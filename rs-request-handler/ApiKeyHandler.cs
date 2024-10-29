using System.Data;
using Microsoft.Data.SqlClient;

namespace rs_request_handler {
    public class ApiKeyMiddleware {
        private readonly RequestDelegate _next;
        private const string APIKEY = "Api_Key";
        private IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration) {

            if (context.Request.Path.StartsWithSegments("/swagger")) {
                await _next(context);
                return;
            }
            _configuration = configuration;
            if (!context.Request.Headers.TryGetValue(APIKEY, out
                    var extractedApiKey)) {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            // get valid apikey from db here
            //var apiKey = appSettings.GetValue<string>(APIKEY);
            if (!GetUserByApiKey(new Guid(extractedApiKey))) {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }

        public bool GetUserByApiKey(Guid pApiKey) {
            string action = "BYAPIKEY";
            string storedproc = "GetUser";
            DBConnection dbconnection = new DBConnection(_configuration, "db_connection");
            SqlCommand authcommand = new SqlCommand(storedproc, dbconnection.GetDBConnection());
            authcommand.CommandType = CommandType.StoredProcedure;
            SqlDataReader authuser = null;
            SqlCommandBuilder.DeriveParameters(authcommand);

            authcommand.Parameters["@Action"].Value = action;
            authcommand.Parameters["@ApiKey"].Value = pApiKey;

            if (dbconnection.IsOpen()) {
                authuser = authcommand.ExecuteReader();
                if (authuser.HasRows) {
                    authuser.Read();
                    Guid _ApiKey = !DBNull.Value.Equals(authuser["api_key"]) ? new Guid(authuser["api_key"].ToString()) : Guid.Empty;
                    if (String.Equals(_ApiKey, pApiKey)) {
                        return true;
                    }
                }
            }
            authuser.Close();
            authcommand.Dispose();
            dbconnection.Close();

            return false;
        }
    }
}
