﻿using Newtonsoft.Json;
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
    public class GraphWebhookController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage HandleRequest()
        {
            DateTime currentDatetime = DateTime.UtcNow;
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string validationToken = string.Empty;
            IEnumerable<string> clientStateHeader = new List<string>();
            string webhookClientState = ConfigurationManager.AppSettings["webhookclientstate"].ToString();
            var queryStringParams = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            if (queryStringParams.AllKeys.Contains("validationToken"))
            {
                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                validationToken = queryStringParams.GetValues("validationToken")[0].ToString();
                httpResponse.Content = new StringContent(validationToken);
                return httpResponse;
            }

            var requestContent = Request.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(requestContent))
                return httpResponse;

            try
            {
                var objNotification = JsonConvert.DeserializeObject<SPWebhookContent>(requestContent);
                var notifications = objNotification.Value;
                Task.Factory.StartNew(() =>
                {
                    notifications.ForEach(notification =>
                    {
                        notification.NotificationDate = currentDatetime;
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