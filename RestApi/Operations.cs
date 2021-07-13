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
using WebhookReceiver.Models;

namespace RestApi
{
    public static class Operations
    {
        private const string ClientState = "A0A354EC-97D4-4D83-9DDB-144077ADB449";
        private const string NotificationUrl = "https://0284f0573ffb.ngrok.io/api/spwebhook/handlerequest";
        private static string AccessToken = string.Empty;
        public static async Task<string> GetTokenAsync()
        {
            string settingJson = String.Format("{0}\\setting.json", AppDomain.CurrentDomain.BaseDirectory);
            AzureAdSetting setting = AzureAdSetting.CreateInstance(settingJson);
            X509Certificate2 certificate = new X509Certificate2(setting.CertficatePath, setting.CertificatePassword, X509KeyStorageFlags.MachineKeySet);
            AuthenticationContext authenticationContext = new AuthenticationContext(setting.Authority, false);
            ClientAssertionCertificate cac = new ClientAssertionCertificate(setting.ClientId, certificate);
            var authenticationResult = await authenticationContext.AcquireTokenAsync(setting.ResourceId, cac);
            return authenticationResult.AccessToken;

        }
        public static void CreateSubscriptions(int count)
        {
            AccessToken = GetTokenAsync().Result;

            List<Teams> teams = DbOperations.GetTeams(count)
                            .ToList();
            teams
                .ForEach(t => { CreateSubscriptionAsync(t).Wait(); });

        }
        public static void GetSubscriptions(int count)
        {
            AccessToken = GetTokenAsync().Result;

            List<Teams> teams = DbOperations.GetTeams(count)
                            .ToList();
            teams
                .ForEach(team => { 
                    var subscriptions = GetSubscriptionsAsync(team);
                    subscriptions.Result
                    .Where(w => w.ClientState == ClientState)
                    .ToList().ForEach(subscription =>
                    {
                        Console.WriteLine($"Subscription {subscription.SubscriptionId} - {team.SiteUrl}");
                    }
                );
                });
        }
        public static void DeleteSubscriptions(int count)
        {
            AccessToken = GetTokenAsync().Result;

            DbOperations.GetTeams(count)
                .ToList()
                .ForEach(team => {
                    var subscriptions = GetSubscriptionsAsync(team);
                    subscriptions.Result
                    .Where(w => w.ClientState == ClientState)
                    .ToList().ForEach(async subscription =>
                    {
                        var r=await DeleteSubscriptionAsync(subscription, team);
                        DbOperations.DeleteSubscription(subscription);
                        Console.WriteLine($"Subscription Deleted {subscription.SubscriptionId} - {team.SiteUrl}");
                    }
                );
                });
        }
        private static async Task CreateSubscriptionAsync(Teams team)
        {
            var siteUrl = team.SiteUrl.Replace("Shared%20Documents", "");
            var url = $"{siteUrl}_api/web/lists('{team.ListId}')/subscriptions";
            var payload = "";
            SubscriptionPost post = new SubscriptionPost()
            {
                Resource = $"{siteUrl}_api/web/lists('{team.ListId}')",
                NotificationUrl= NotificationUrl,
                ClientState= ClientState
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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
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
                //if (resultContent.Contains("Access token has expired") 
                //    || resultContent.Contains("temporary issue, so try again in a few minutes"))
                {
                    Console.WriteLine("Renew Tokean");
                    AccessToken = GetTokenAsync().Result;
                    await CreateSubscriptionAsync(team);
                }
            }

        }

        private static async Task<List<Subscription>> GetSubscriptionsAsync(Teams team)
        {
            var siteUrl = team.SiteUrl.Replace("Shared%20Documents", "");
            var url = $"{siteUrl}_api/web/lists('{team.ListId}')/subscriptions";
            
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage result = await client.GetAsync(url);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                var subscriptions = JsonConvert.DeserializeObject<ContentSubscriptions>(resultContent);
                Console.ForegroundColor = ConsoleColor.Green;
                return subscriptions.Value;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(resultContent);
                Console.ForegroundColor = ConsoleColor.Gray;
                //if (resultContent.Contains("Access token has expired"))
                {
                    Console.WriteLine("Renew Tokean");
                    AccessToken = GetTokenAsync().Result;
                    await GetSubscriptionsAsync(team);
                }
                return null;
            }
        }

        private static async Task<bool> DeleteSubscriptionAsync(Subscription subscription, Teams team)
        {
            var siteUrl = team.SiteUrl.Replace("Shared%20Documents", "");
            var url = $"{siteUrl}_api/web/lists('{team.ListId}')/subscriptions('{subscription.SubscriptionId}')";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage result = await client.DeleteAsync(url);
            var resultContent = result.Content.ReadAsStringAsync().Result;
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                //if (resultContent.Contains("Access token has expired"))
                {
                    Console.WriteLine("Renew Tokean");
                    AccessToken = GetTokenAsync().Result;
                    await DeleteSubscriptionAsync(subscription,team);
                }
                return false;
            }
        }

        private static void TestGet()
        {
            string AccessToken = GetTokenAsync().Result;
            var url = "https://mmoustafa.sharepoint.com/_api/web/lists";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
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
            //webRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + AccessToken);
            //webRequest.Method = "GET";
            //HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

        }
    }
}
