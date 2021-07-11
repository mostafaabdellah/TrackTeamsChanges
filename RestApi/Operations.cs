using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
            var url = "https://devx9.sharepoint.com/_api/web/lists";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            //setup the client get
            HttpResponseMessage result = await client.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine("Email sent successfully. ");
            }

            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //webRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
            //webRequest.Method = "GET";
            //HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

        }
    }
}
