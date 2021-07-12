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
            if (Request.Headers.TryGetValues("ClientState", out clientStateHeader))
            {
                string clientStateHeaderValue = clientStateHeader.FirstOrDefault() ?? string.Empty;

                if (!string.IsNullOrEmpty(clientStateHeaderValue) && clientStateHeaderValue.Equals(webhookClientState))
                {
                    var queryStringParams = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                    if (queryStringParams.AllKeys.Contains("validationtoken"))
                    {
                        httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                        validationToken = queryStringParams.GetValues("validationtoken")[0].ToString();
                        httpResponse.Content = new StringContent(validationToken);
                        return httpResponse;
                    }
                    else
                    {
                        var requestContent = Request.Content.ReadAsStringAsync().Result;

                        if (!string.IsNullOrEmpty(requestContent))
                        {
                            SPWebhookNotification notification = null;

                            try
                            {
                                var objNotification = JsonConvert.DeserializeObject<SPWebhookContent>(requestContent);
                                notification = objNotification.Value[0];
                                notification.NotificationDate = currentDatetime;
                            }
                            catch (JsonException ex)
                            {
                                var traceWriter = Configuration.Services.GetTraceWriter();
                                traceWriter.Trace(Request, "SPWebhooks",
                                    TraceLevel.Error,
                                    string.Format("JSON deserialization error: {0}", ex.InnerException));
                                return httpResponse;
                            }

                            if (notification != null)
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    DbOperations.AddNotifications(notification);
                                });

                                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                            }
                        }
                    }
                }
                else
                {
                    httpResponse = new HttpResponseMessage(HttpStatusCode.Forbidden);
                }
            }

            return httpResponse;
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
