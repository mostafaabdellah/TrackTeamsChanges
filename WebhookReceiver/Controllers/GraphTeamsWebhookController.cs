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
    public class GraphTeamsWebhookController : ApiController
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

            var responseContent = Request.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrEmpty(responseContent))
                return httpResponse;

            try
            {
                Task.Factory.StartNew(() =>
                {
                    DbOperations.AddLog(new LogInfo()
                    {
                        CreatedOn = DateTime.UtcNow,
                        ResponseContent = responseContent
                    });

                    //var objNotification = JsonConvert.DeserializeObject<TeamsContent>(responseContent);
                    //var notifications = objNotification.value;
                    //notifications.ForEach(notification =>
                    //{
                    var idIndx = responseContent.IndexOf("\"id\"");
                    var teamId = responseContent.Substring(idIndx + 6, 36);
                        var teams =new Teams()
                        {
                            TeamId = teamId,
                            CreatedOn = DateTime.UtcNow
                        };
                        DbOperations.AddTeams(teams);
                    //});
                });
                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                return httpResponse;
            }
            catch (JsonException ex)
            {
                DbOperations.AddLog(new LogInfo()
                {
                    CreatedOn = DateTime.UtcNow,
                    ResponseContent = responseContent,
                    Exception = ex.InnerException.ToString()
                });
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