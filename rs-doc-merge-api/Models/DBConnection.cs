using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace rs_doc_merge_api.Models {
    public class DBConnection {

        private SqlConnection _letterdb;
        private IConfiguration _SystemConfiguration;
        private string _ConnectionString = "";

        public DBConnection(IConfiguration pSystemConfiguration, string pConnectionType) {
            _SystemConfiguration = pSystemConfiguration;
            _ConnectionString = _SystemConfiguration.GetConnectionString(pConnectionType);
            try {
                if (_ConnectionString != "") {
                    _letterdb = new SqlConnection();
                    _letterdb.ConnectionString = _ConnectionString;
                    if (_letterdb.State != ConnectionState.Open) {
                        _letterdb.Open();
                    }
                }
            } catch (Exception e) {
                throw new Exception(e.Message, e);
            }
        }

        public bool IsOpen() {
            if (_letterdb.State == ConnectionState.Open) {
                return true;
            } else {
                return false;
            }
        }

        public SqlConnection GetDBConnection() {
            return _letterdb;
        }

        public void Close() {
            if (IsOpen()) {
                _letterdb.Close();
            }
        }

    }
}