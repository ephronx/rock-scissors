using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace rs_doc_event_handler {
    public class CheckForNewDocRequests {
        private readonly ILogger _logger;

        public CheckForNewDocRequests(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<CheckForNewDocRequests>();
        }

        [Function("CheckForNewDocRequests")]
        public async Task<string> Run([TimerTrigger("0 */1 * * * *")] ScheduleInfo myTimer) {
            string result = "";
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            try {
                result = await GetApiResponseAsync("https://your-merge-api.example/api/Merge");
            } catch (Exception ex) {
                result = ex.Message;
            }
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            _logger.LogInformation("result :" +  result);
            return result;
        }
        private async Task<string> GetApiResponseAsync(string url) {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }

    public class ScheduleInfo {
        public DocCheckScheduleStatus ScheduleStatus { get; set; }
        public bool IsPastDue { get; set; }
    }

    public class DocCheckScheduleStatus {
        public DateTime Last { get; set; }
        public DateTime Next { get; set; }
        public DateTime LastUpdated { get; set; }
    }



}
