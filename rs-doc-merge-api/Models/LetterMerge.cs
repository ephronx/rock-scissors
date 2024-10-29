using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Docentric.Documents.Reporting;
using System.IO;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using rs_doc_merge_api.Models;
using Docentric.Documents.ObjectModel;
using Microsoft.ApplicationInsights;

namespace rs_doc_merge_api.Models {
    public class LetterMerge {

        private readonly string _SampleConnString = "server=<servername>;database=<db_name>;trusted_connection=True;Encrypt=False";
        private readonly ILogger<LetterMerge> _logger;

        public partial class Key {
            [JsonPropertyName("keyName")]
            public string _keyName { get; set; }
            [JsonPropertyName("keyType")]
            public string _keyType { get; set; }
            [JsonPropertyName("keyValue")]
            public string _keyValue { get; set; }

            public Key(string keyName, string keyType, string keyValue) {
                _keyName = keyName;
                _keyType = keyType;
                _keyValue = keyValue;
            }
        }

        private XElement _sourceXml;
        [JsonPropertyName("requestId")]
        public Guid _requestId { get; set; } = Guid.Empty;
        [JsonPropertyName("requestType")]
        public string _requestType { get; set; } = "";
        [JsonPropertyName("outputFileName")]
        public string _outputfilename { get; set; } = "";
        [JsonPropertyName("requestDate")]
        public DateTime? _requestDate { get; set; }
        [JsonPropertyName("errors")]
        public string _errors { get; set; } = "";

        private string _outputLocation = "";

        [JsonPropertyName("keys")]
        public List<Key> _keys { get; set; }

        private Template _template { get; set; }

        private string _connectionString = "";

        private JsonSerializerOptions options = new JsonSerializerOptions {
            WriteIndented = true,
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        IConfiguration _configuration;

        public LetterMerge(IConfiguration configuration, Guid requestId, string outputLocation, ILogger<LetterMerge> logger, string manual = "NO") {
            _logger = logger;
            _requestId = requestId;
            _outputLocation = outputLocation;

            _configuration = configuration;

            DBConnection letterDb = new DBConnection(configuration, "db_connection");

            Docentric.Licensing.LicenseSource.SetPhysicalFilePath(AppDomain.CurrentDomain.BaseDirectory + "Docentric.lic");

            try {
                string sSql = "";
                string _docDate = "";

                //if (manual == "YES") {
                //    using (var command = new SqlCommand("update letterqueue set status='ready' where requestid = @requestid and status='new' and processing=0", letterDb.GetConnection())) {
                //        command.Parameters.AddWithValue("@requestid", requestId);
                //        command.ExecuteNonQuery();
                //    }
                //}
                //using (var command = new SqlCommand("update letterqueue set processing=1, startdate=getdate() where requestid = @requestid and processing=0 and status='ready'", letterDb.GetConnection())) {
                //    command.Parameters.AddWithValue("@requestid", requestId);
                //    command.ExecuteNonQuery();
                //}

                sSql = @"select top 1 requestid, requestxml from dbo.letterqueue where processing = 1 and status = 'new' and requestId = '<requestId>' order by priority, createddate desc";
                sSql = sSql.Replace("<requestId>", requestId.ToString());

                SqlCommand myCommand = new SqlCommand(sSql, letterDb.GetDBConnection());
                SqlDataReader mySqlRS = myCommand.ExecuteReader();

                if (mySqlRS.HasRows) {
                    mySqlRS.Read();
                    XmlDocument _letterXml = new XmlDocument();

                    _letterXml.LoadXml(mySqlRS["requestxml"].ToString());
                    _requestType = _letterXml.SelectNodes("/DocumentRequest/Header/RequestType").Item(0).InnerText.ToString();
                    _docDate = _letterXml.SelectNodes("/DocumentRequest/Header/LetterDate").Item(0).InnerText.ToString();
                    _outputfilename = _letterXml.SelectNodes("/DocumentRequest/Header/OutputFileName").Item(0).InnerText.ToString();

                    if (_letterXml.SelectSingleNode("/DocumentRequest/Header/Keys") != null) {
                        if (_letterXml.SelectNodes("/DocumentRequest/Header/Keys").Item(0).ChildNodes.Count > 0) {
                            _keys = new List<Key>();
                            foreach (XmlNode xmlNode in _letterXml.SelectNodes("/DocumentRequest/Header/Keys").Item(0).ChildNodes) {
                                _keys.Add(new Key(xmlNode.Name.ToString().Trim(),
                                              xmlNode.Attributes["type"] != null ? xmlNode.Attributes["type"].Value.ToString().Trim() : "string",
                                              xmlNode.InnerText.Trim()));
                            }
                        }
                    }
                    MergeDocument(_letterXml);

                    mySqlRS.Close();

                    using (var command = new SqlCommand("update letterqueue set processing=0, status='done', enddate=getdate() where requestid = @requestid", letterDb.GetDBConnection())) {
                        command.Parameters.AddWithValue("@requestid", requestId);
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception err) {
                using (var command = new SqlCommand("update letterqueue set processing=0, status='errored', enddate=getedate(), errors='" + err.InnerException.ToString() + "' where requestid = @requestid", letterDb.GetDBConnection())) {
                    command.Parameters.AddWithValue("@requestid", requestId);
                    command.ExecuteNonQuery();
                }
                _errors = err.InnerException.ToString();
            } finally {
                letterDb.Close();
            }
        }

        private bool MergeDocument(XmlDocument xmlDocument) {

            // if full xml then execute merge
            // if SQL then get XML from SQL SP and then execute merge

            _template = new Template(_configuration, _requestType);

            if (_outputfilename == "") {
                _outputfilename = AppDomain.CurrentDomain.BaseDirectory + "/" + _template._apiKey.ToString() + "/" + _requestId + ".docx";
            }
            try {
                if (_template._hostType == "SQL") {
                    //DBConnection sourcedata = new DBConnection(GetSourceConnectionString(_template._dataHost, _template._db_Name));

                    //SqlCommand sqlCmd = new SqlCommand(_template._sp_Name, sourcedata.GetDBConnection());
                    //sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    //foreach (Key key in _keys) {
                    //    System.Data.SqlDbType paramtype = System.Data.SqlDbType.VarChar;
                    //    switch (key._keyType) {
                    //        case "string": paramtype = System.Data.SqlDbType.VarChar; break;
                    //        case "Date": paramtype = System.Data.SqlDbType.Date; break;
                    //        case "DateTime": paramtype = System.Data.SqlDbType.DateTime; break;
                    //        case "int": paramtype = System.Data.SqlDbType.Int; break;
                    //    }
                    //    sqlCmd.Parameters.Add("@" + key._keyName, paramtype);
                    //    sqlCmd.Parameters["@" + key._keyName].Value = key._keyValue;
                    //}

                    //SqlDataReader srcXml = sqlCmd.ExecuteReader();

                    //if (srcXml.HasRows) {
                    //    while (srcXml.Read()) {
                    //        // this is the correct code for processing non SQL data
                    //        MemoryStream xml = new MemoryStream(Encoding.UTF8.GetBytes(srcXml[0].ToString() ?? ""));
                    //        _sourceXml = XElement.Load(xml);
                    //    }
                    //}

                    //sourcedata.Close();
                } else {
                    XmlNode dataPart = xmlDocument.SelectSingleNode("/DocumentRequest/Data");
                    XmlReader reader = new XmlNodeReader(dataPart);
                    _sourceXml = XElement.Load(reader);
                }
            } catch (Exception err) {
                throw new Exception(err.Message);
            }

            CreateDirectory(_outputfilename);

            try {
                using (Stream templateStream = new MemoryStream(_template._templateDoc))
                using (Stream reportDocumentStream = new MemoryStream()) {

                    var dg = new DocumentGenerator(_sourceXml);

                    dg.WriteErrorsAndWarningsToDocument = false;

                    //generate Docentric document
                    var result = dg.GenerateDocument(templateStream, reportDocumentStream);

                    if (result.HasErrors) {
                        IEnumerable<string> errorMessages = result.Errors.Select(x => $"Severity: {x.Severity.ToString()}; Message: {x.Message}");
                        throw new Exception(string.Join(Environment.NewLine, errorMessages));
                    } else {
                        using (var fileStream = File.Create(_outputfilename)) {
                            reportDocumentStream.Seek(0, SeekOrigin.Begin);
                            reportDocumentStream.CopyTo(fileStream);
                        }
                    }
                }
            } catch (Exception err) {
                throw new Exception(err.Message);
            }

            byte[] pdfdoc = ConvertWordToPDF(_outputfilename);
            File.WriteAllBytes(_outputfilename.Replace(".docx", "") + ".pdf", pdfdoc);

            return true;
        }

        private static byte[] ConvertWordToPDF(string FileName) {
            Aspose.Words.License lic = new Aspose.Words.License();
            try {
                lic.SetLicense(AppDomain.CurrentDomain.BaseDirectory + "Aspose.Words.NET.lic");
            } catch (Exception err) {
                Console.WriteLine(err.Message);
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(FileName);
            System.IO.MemoryStream mstream = new MemoryStream();
            doc.Save(mstream, Aspose.Words.SaveFormat.Pdf);

            return mstream.ToArray();
        }

        private static void CreateDirectory(string path) {
            string dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        public string GetRequestAsJson() {
            return JsonSerializer.Serialize(this, options);
        }

        public string GetSourceConnectionString(string serverName, string dbName) {
            return _SampleConnString.Replace("<servername>", serverName).Replace("<db_name>", dbName);
        }

    }
}
