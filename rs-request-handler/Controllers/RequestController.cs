using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using rs_request_handler.Models;
using System.Net.Mime;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Primitives;

namespace rs_request_handler.Controllers {
    [ApiController]
    [Route("api/v1/[action]")]
    public class RequestController : ControllerBase {
        private IConfiguration _SystemConfig;

        private readonly ILogger<RequestController> _logger;
        private readonly ILogger<LetterRequest> _letterLogger;
        private readonly ILogger<BatchHandler> _batchLogger;

        private JsonSerializerOptions options = new JsonSerializerOptions {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RequestController(ILogger<RequestController> reqLogger, ILogger<LetterRequest> letterLogger, IConfiguration configuration, ILogger<BatchHandler> batchLogger) {
            _SystemConfig = configuration;
            _logger = reqLogger;
            _letterLogger = letterLogger;
            _batchLogger = batchLogger;
        }

        //private bool AuthenticateUser(string userName) {
        //    string useraccount = userName.Substring(userName.IndexOf("\\") + 1, userName.Length - (userName.IndexOf("\\") + 1));
        //    string userdomain = userName.Substring(0, userName.IndexOf("\\"));
        //    PrincipalContext ctx = new PrincipalContext(ContextType.Domain, userdomain);
        //    UserPrincipal user = UserPrincipal.FindByIdentity(ctx, useraccount);
        //    PrincipalSearchResult<Principal> groups = user.GetGroups();
        //    foreach (Principal group in groups) {
        //        if (group.Name == "Domain Users") {
        //            // Note: Change this when you create a group for the API
        //            //if (group.Name == "some random ad group") {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        [HttpGet]
        public ActionResult<bool> Index() {
            return true;
        }

        [HttpPost]
        [Consumes("application/xml")]
        [Produces("application/json")]
        public ActionResult<SvcResponse> AddRequest([FromBody] XmlDocument reqXml) {
            SvcResponse svcResponse;
            try {
                Guid apikey = Guid.Empty;
                Request.Headers.TryGetValue("Api_Key", out var sApiKey);
                if (sApiKey != StringValues.Empty) {
                    apikey = Guid.Parse(sApiKey);
                }
                Guid requestid = Guid.Empty;
                LetterRequest letterRequest = new LetterRequest(_SystemConfig, _letterLogger);
                letterRequest._apiKey = apikey;
                requestid = letterRequest.SetRequestXml(reqXml);
                svcResponse = new SvcResponse(true, "LetterId", requestid.ToString(), "");
                _logger.Log(LogLevel.Information, "Request " + requestid.ToString() + " created at: " + DateTime.Now.ToString());
                letterRequest.FlipReadyforProcessing();
            } catch (Exception ex) {
                _logger.Log(LogLevel.Error, ex.Message);
                svcResponse = new SvcResponse(false,"LetterId", "-1", ex.Message);
            }
            return svcResponse;
        }

        [HttpPost]
        [Consumes("application/xml")]
        [Produces("application/json")]
        public ActionResult<SvcResponse> AddBatchRequest([FromBody] XmlDocument reqXml, Guid batchId) {
            SvcResponse svcResponse;
            try {
                Guid apikey = Guid.Empty;
                Request.Headers.TryGetValue("Api_Key", out var sApiKey);
                if (sApiKey != StringValues.Empty) {
                    apikey = Guid.Parse(sApiKey);
                }
                Guid requestid = Guid.Empty;
                LetterRequest letterRequest = new LetterRequest(_SystemConfig, _letterLogger);
                letterRequest.setBatchId(batchId);
                requestid = letterRequest.SetRequestXml(reqXml);
                svcResponse = new SvcResponse(true, "LetterId", requestid.ToString(), "");
                _logger.Log(LogLevel.Information, "Request " + requestid.ToString() + " created at: " + DateTime.Now.ToString());
            } catch (Exception ex) {
                _logger.Log(LogLevel.Error, ex.Message);
                svcResponse = new SvcResponse(false, "LetterId", "-1", ex.Message);
            }
            return svcResponse;
        }

        [HttpPost]
        public ActionResult<SvcResponse> OpenNewBatch() {
            Guid apikey = Guid.Empty;
            Request.Headers.TryGetValue("Api_Key", out var sApiKey);
            if (sApiKey != StringValues.Empty) {
                apikey = Guid.Parse(sApiKey);
            }
            BatchHandler batchHandler = new BatchHandler();
            batchHandler.newBatch(_SystemConfig, apikey, _batchLogger);
            SvcResponse svcResponse;
            if (batchHandler.batchId != Guid.Empty) {
                svcResponse = new SvcResponse(true, "BatchId", batchHandler.batchId.ToString(), "");
            } else {
                svcResponse = new SvcResponse(false, "BatchId", Guid.Empty.ToString(), "Could not generate a new batch id");
            }
            return svcResponse;
        }

        [HttpPost]
        public ActionResult<SvcResponse> CloseAndExecuteBatch(Guid batchId) {
            return new SvcResponse();
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<QueryResponse> QueryRequest(Guid requestId) {
            if (requestId == Guid.Empty) {
                return BadRequest("No requestId were provided.");
            }
            LetterRequest letterRequest = new LetterRequest(_SystemConfig, _letterLogger);
            bool status = letterRequest.GetRequestStatus(requestId);
            string statusStr = letterRequest.StatusAsJson();
            QueryResponse queryResponse = new QueryResponse(status, requestId.ToString(), "", statusStr);
            return queryResponse;
        }

        [HttpGet]
        [Produces("application/json")]
        public ActionResult<IEnumerable<QueryResponse>> QueryRequests([FromQuery] IEnumerable<Guid> requestIds) {
            if (requestIds == null || !requestIds.Any()) {
                return BadRequest("No requestIds were provided.");
            }
            var letterRequest = new LetterRequest(_SystemConfig, _letterLogger);
            var queryResponses = requestIds
                .Select(requestId => {
                    var status = letterRequest.GetRequestStatus(requestId);
                    var statusStr = letterRequest.StatusAsJson();
                    return new QueryResponse(status, requestId.ToString(), "", statusStr);
                })
                .ToList();

            return queryResponses;
        }
    }
}