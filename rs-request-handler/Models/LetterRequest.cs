using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.NetworkInformation;
using System.Xml;
using System.Data.SqlTypes;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace rs_request_handler.Models {
    public class LetterRequest {
        private IConfiguration _configuration;
        private ILogger<LetterRequest> _logger;

        private Guid _requestid { get; set; }
        public DateTime _createdDate { get; set; }
        private XmlDocument? _requestXml { get; set; }
        private string _reqeustType { get; set; }
        public int _processing { get; set; } = -1;
        public DateTime? _startDate { get; set; }
        public DateTime? _endDate { get; set; }
        public string? _status { get; set; }
        public string? _errors { get; set; }
        private string? _processingHost { get; set; }
        public string? _outputLocation { get; set; }
        private int _priority { get; set; }
        public Guid _batchId { get; set; }
        public Guid _apiKey { get; set; }
        private XmlDocument? _dataXml { get; set; }

        private JsonSerializerOptions options = new JsonSerializerOptions {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public LetterRequest(IConfiguration configuration, ILogger<LetterRequest> logger) {
            _configuration = configuration;
            _logger = logger;
        }

        public Guid SetRequestXml(XmlDocument xmlDocument) {
            _requestXml = xmlDocument;
            _createdDate = DateTime.Now;
            _status = "new";
            _processing = -1;
            _errors = "";

            if (AddRequest()) {
                return _requestid;
            }
            return Guid.Empty;
        }

        public Guid SetRequestJson(string jsonDocument) {

            JsonRequest jsonRequest = new JsonRequest();

            _createdDate = DateTime.Now;
            _status = "new";
            _processing = -1;
            _errors = "";

            _requestXml = jsonRequest.convertToXml(jsonDocument);

            if (AddRequest()) {
                return _requestid;
            }
            return Guid.Empty;
        }

        public Guid letterRequestId() {
            return _requestid;
        }

        public void setBatchId(Guid batchId) {
            _batchId = batchId;
        }

        public bool AddRequest() {
            try {
                DBConnection dbconnection = new DBConnection(_configuration, "db_connection");
                _requestid = Guid.Empty;
                if (dbconnection.IsOpen()) {
                    string action = "ADDNEWREQUEST";
                    string storedprocname = "dbo.SetLetterRequest";

                    string requesttype = _requestXml.SelectNodes("/DocumentRequest/Header/RequestType").Item(0).InnerText.ToString();

                    try {

                        SqlCommand addrequest = new SqlCommand(storedprocname, dbconnection.GetDBConnection()) {
                            CommandType = CommandType.StoredProcedure
                        };

                        SqlCommandBuilder.DeriveParameters(addrequest);

                        addrequest.Parameters["@Action"].Value = action;
                        addrequest.Parameters["@requesttype"].Value = requesttype;
                        addrequest.Parameters["@requestXml"].Value = _requestXml.InnerXml.ToString();
                        addrequest.Parameters["@createdDate"].Value = _createdDate;
                        addrequest.Parameters["@status"].Value = _status;
                        addrequest.Parameters["@processing"].Value = _processing;
                        addrequest.Parameters["@apikey"].Value = _apiKey;
                        addrequest.Parameters["@batchid"].Value = _batchId;
                        addrequest.Parameters["@errors"].Value = _errors;
                        addrequest.Parameters["@newid"].Direction = ParameterDirection.Output;
                        addrequest.Parameters["@newid"].DbType = DbType.Guid;

                        addrequest.ExecuteNonQuery();

                        _requestid = (Guid)addrequest.Parameters["@newid"].Value;

                        addrequest.Dispose();
                        dbconnection.Close();

                        if (_requestid != Guid.Empty) {
                            return true;
                        }
                    } catch (Exception err) {
                        _logger.Log(LogLevel.Error, err.Message);
                        return false;
                    }
                }
            } catch (Exception ex) {
                _logger.Log(LogLevel.Error, "Database error: " + ex.Message);
            }
            return true;
        }

        public bool GetRequestStatus(Guid requestId) {
            DBConnection dbconnection = new DBConnection(_configuration, "db_connection");
            if (dbconnection.IsOpen() && requestId != Guid.Empty) {
                string action = "GETREQUESTSTATUS";
                string storedprocname = "dbo.GetLetterRequest";

                try {
                    SqlCommand getrequest = new SqlCommand(storedprocname, dbconnection.GetDBConnection()) {
                        CommandType = CommandType.StoredProcedure
                    };

                    SqlCommandBuilder.DeriveParameters(getrequest);

                    getrequest.Parameters["@Action"].Value = action;
                    getrequest.Parameters["@requestId"].Value = requestId;

                    SqlDataReader request = getrequest.ExecuteReader();

                    while (request.Read()) {
                        _reqeustType = request["requesttype"].ToString();
                        _createdDate = request["createdDate"] != DBNull.Value ? (DateTime)request["createdDate"] : DateTime.MinValue;
                        _startDate = request["startdate"] != DBNull.Value ? (DateTime)request["startdate"] : null;
                        _endDate = request["endDate"] != DBNull.Value ? (DateTime)request["createdDate"] : null;
                        _status = request["status"].ToString();
                        _processing = (int)request["processing"];
                        _errors = request["errors"].ToString();
                        _batchId = Guid.Parse(request["batchid"].ToString());
                        _apiKey = Guid.Parse(request["apikey"].ToString());
                        _outputLocation = request["outputlocation"].ToString();
                    }

                    request.Dispose();
                    dbconnection.Close();
                } catch {
                    return false;
                }
            }
            return true;
        }

        public bool FlipReadyforProcessing() {
            DBConnection dbconnection = new DBConnection(_configuration, "db_connection");
            if (dbconnection.IsOpen()) {
                string sql = "update dbo.letterqueue set processing = 0 where requestid = '{requestid}'";
                try {

                    SqlCommand setartwork = new SqlCommand(sql.Replace("{requestid}", _requestid.ToString()), dbconnection.GetDBConnection()) {
                        CommandType = CommandType.Text
                    };

                    setartwork.ExecuteNonQuery();

                    setartwork.Dispose();
                    dbconnection.Close();
                } catch {
                    return false;
                }
            }
            return true;
        }

        public string StatusAsJson() {
            return JsonSerializer.Serialize(this, options);
        }
    }
}
