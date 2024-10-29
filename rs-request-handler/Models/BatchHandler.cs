using Microsoft.Data.SqlClient;

namespace rs_request_handler.Models {
    public class BatchHandler {

        public Guid batchId { get; set; }

        private readonly ILogger<BatchHandler> _logger;

        public BatchHandler() {

        }

        public Guid newBatch(IConfiguration configuration, Guid apiKey, ILogger<BatchHandler> batchLogger) {

            string spName = "dbo.SetBatch";
            string action = "NEWBATCH";

            batchId = Guid.NewGuid();

            DBConnection dbconnection = new DBConnection(configuration, "db_connection");

            if (dbconnection.IsOpen()) {
                try {
                    SqlCommand newBatch = new SqlCommand(spName, dbconnection.GetDBConnection());
                    newBatch.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlCommandBuilder.DeriveParameters(newBatch);

                    newBatch.Parameters["@Action"].Value = action;
                    newBatch.Parameters["@batchId"].Value = batchId;
                    newBatch.Parameters["@apiKey"].Value = apiKey;

                    newBatch.ExecuteNonQuery();

                    newBatch.Dispose();
                    dbconnection.Close();

                } catch (Exception err) {
                    _logger.Log(LogLevel.Error, err.Message);
                }
            }

            return batchId;
        }
    }
}
