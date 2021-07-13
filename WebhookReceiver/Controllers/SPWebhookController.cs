using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;
using TrackTeamsChanges;
using WebhookReceiver.Models;

namespace WebhookReceiver.Controllers
{
    public class SPWebhookController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage HandleRequest()
        {
            DateTime currentDatetime = DateTime.UtcNow;
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string validationToken = string.Empty;
            IEnumerable<string> clientStateHeader = new List<string>();
            string webhookClientState = ConfigurationManager.AppSettings["webhookclientstate"].ToString();
            Request.Headers.TryGetValues("clientState", out clientStateHeader);
            if (!Request.Headers.TryGetValues("ClientState", out clientStateHeader))
                return httpResponse;

            string clientStateHeaderValue = clientStateHeader.FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(clientStateHeaderValue) || !clientStateHeaderValue.Equals(webhookClientState))
            {
                httpResponse = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return httpResponse;
            }
            var queryStringParams = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            if (queryStringParams.AllKeys.Contains("validationtoken"))
            {
                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                validationToken = queryStringParams.GetValues("validationtoken")[0].ToString();
                httpResponse.Content = new StringContent(validationToken);
                return httpResponse;
            }
            var responseContent = Request.Content.ReadAsStringAsync().Result;
            //var traceWriter = Configuration.Services.GetTraceWriter();
            //traceWriter.Trace(Request, "SPWebhooks", TraceLevel.Info,string.Format("Notification Response={0}", responseContent));
            if (string.IsNullOrEmpty(responseContent))
                return httpResponse;

            try
            {
                var objNotification = JsonConvert.DeserializeObject<SPWebhookContent>(responseContent);
                var notifications = objNotification.Value;
                Task.Factory.StartNew(() =>
                {
                    notifications.ForEach(notification =>
                    {
                        notification.NotificationDate = currentDatetime;
                        notification.Content = responseContent;
                        DbOperations.AddNotifications(notification);
                    });
                });
                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                return httpResponse;
            }
            catch (JsonException ex)
            {
                var traceWriter = Configuration.Services.GetTraceWriter();
                traceWriter.Trace(Request, "SPWebhooks",
                    TraceLevel.Error,
                    string.Format("JSON deserialization error: {0}", ex.InnerException));
                return httpResponse;
            }
        }

        [HttpGet]
        public HttpResponseMessage IsLive()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Live")
            };
        }
    }
}
