using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TrackTeamsChanges;

namespace RestApi
{
    public static class Operations
    {
        public static async Task<string> GetTokenAsync()
        {
            string settingJson = String.Format("{0}\\setting.json", AppDomain.CurrentDomain.BaseDirectory);
            AzureAdSetting setting = AzureAdSetting.CreateInstance(settingJson);

            //if you need to load from certficate store, use different constructors. 
            X509Certificate2 certificate = new X509Certificate2(setting.CertficatePath, setting.CertificatePassword, X509KeyStorageFlags.MachineKeySet);
            AuthenticationContext authenticationContext = new AuthenticationContext(setting.Authority, false);

            ClientAssertionCertificate cac = new ClientAssertionCertificate(setting.ClientId, certificate);

            //get the access token to Outlook using the ClientAssertionCertificate
            var authenticationResult = await authenticationContext.AcquireTokenAsync(setting.ResourceId, cac);
            return authenticationResult.AccessToken;

        }
        public static void CreateSubscriptions(int count)
        {
            List<Teams> teams = DbOperations.GetTeams(count)
                            .ToList();
            teams
                .ForEach(t => { CreateSubscriptionAsync(t).Wait(); });

        }
        private static async Task CreateSubscriptionAsync(Teams team)
        {
            string accessToken = GetTokenAsync().Result;
            var siteUrl = team.SiteUrl.Replace("Shared%20Documents", "");
            var url = $"{siteUrl}_api/web/lists('{team.ListId}')/subscriptions";
            var payload = "";
            SubscriptionPost post = new SubscriptionPost()
            {
                Resource = $"{siteUrl}_api/web/lists('{team.ListId}')",
                NotificationUrl= "https://0284f0573ffb.ngrok.io/api/spwebhook/handlerequest"
            };
            payload = JsonConvert.SerializeObject(post, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(url, content);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                var subscription = JsonConvert.DeserializeObject<Subscription>(resultContent);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Subscription {subscription.SubscriptionId} Added to {siteUrl}");
                subscription.TeamId = team.TeamId;
                DbOperations.AddSubscriptions(subscription);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(resultContent);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }
        private static void TestGet()
        {
            string accessToken = GetTokenAsync().Result;
            var url = "https://mmoustafa.sharepoint.com/_api/web/lists";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            //setup the client get
            HttpResponseMessage result = client.GetAsync(url).Result;
            if (result.IsSuccessStatusCode)
            {
                var resultContent = result.Content.ReadAsStringAsync().Result;
                var obj=JsonConvert.DeserializeObject(resultContent);
                Console.WriteLine(obj.ToString());
            }

            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //webRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
            //webRequest.Method = "GET";
            //HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

        }
    }
}
