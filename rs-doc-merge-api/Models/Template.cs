using Microsoft.Data.SqlClient;
using System;

namespace rs_doc_merge_api.Models {
    public class Template {

        public Guid _templateId { get; set; }
        public Guid _apiKey { get; set; }
        public string _internalTemplateName { get; set; }
        public string _templateCode { get; set; }
        public string _category { get; set; }
        public bool _active { get; set; }
        public DateTime _createdDate { get; set; }
        public DateTime _endDate { get; set; }
        public int _defaultPriority { get; set; }
        public byte[] _templateDoc { get; set; }
        public int _version { get; set; }
        public string _dataHost { get; set; }
        public string _hostType { get; set; }
        public string _db_Name { get; set; }
        public string _sp_Name { get; set; }
        public string _params { get; set; }

        public Template(IConfiguration configuration, string templateCode) {
            DBConnection letterDb = new DBConnection(configuration, "db_connection");

            try {
                string sSql = "";
                string _docDate = "";

                sSql = @"SELECT templateId
                              ,apikey
                              ,internaltemplatename
                              ,templatecode
                              ,category
                              ,active
                              ,createddate
                              ,enddate
                              ,defaultpriority
                              ,templatedoc
                              ,version
                              ,datahost
                              ,hosttype
                              ,db_name
                              ,sp_name
                              ,params
                          FROM sf_docengine.dbo.template
                        where templatecode = '<templateCode>' and active = 1";
                sSql = sSql.Replace("<templateCode>", templateCode.ToString());

                SqlCommand myCommand = new SqlCommand(sSql, letterDb.GetDBConnection());
                SqlDataReader mySqlRS = myCommand.ExecuteReader();

                if (mySqlRS.HasRows) {
                    while (mySqlRS.Read()) {
                        _templateId = new Guid(mySqlRS["templateId"].ToString());
                        _apiKey = new Guid(mySqlRS["apiKey"].ToString());
                        _internalTemplateName = mySqlRS["internalTemplateName"].ToString();
                        _templateCode = mySqlRS["templateCode"].ToString();
                        _category = mySqlRS["category"].ToString();
                        _active = (bool)mySqlRS["active"];
                        _createdDate = !DBNull.Value.Equals(mySqlRS["createddate"]) ? (DateTime)mySqlRS["createddate"] : new DateTime();
                        _endDate = !DBNull.Value.Equals(mySqlRS["enddate"]) ? (DateTime)mySqlRS["enddate"] : new DateTime();
                        _defaultPriority = (int)mySqlRS["defaultPriority"];
                        _templateDoc = !DBNull.Value.Equals(mySqlRS["templateDoc"]) ? (byte[])mySqlRS["templateDoc"] : null;
                        _version = (int)mySqlRS["version"];
                        _hostType = mySqlRS["hosttype"].ToString();
                        _dataHost = mySqlRS["dataHost"].ToString();
                        _db_Name = mySqlRS["db_name"].ToString();
                        _sp_Name = mySqlRS["sp_name"].ToString();
                        _params = mySqlRS["params"].ToString();
                    }
                }
                mySqlRS.Close();
                myCommand.Dispose();
            }
            catch (Exception err) {

            }
            finally {
                letterDb.Close();
            }
        }
    }
}
