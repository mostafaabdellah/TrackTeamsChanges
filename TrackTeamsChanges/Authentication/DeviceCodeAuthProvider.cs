// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TrackTeamsChanges.Authentication
{
    public class DeviceCodeAuthProvider : IAuthenticationProvider
    {
        private IConfidentialClientApplication _msalClient;
        private string _token;
        private AuthenticationConfig config;

        public DeviceCodeAuthProvider()
        {
            config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
            
            _msalClient = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                                                      .WithClientSecret(config.ClientSecret)
                                                      .WithAuthority(new Uri(config.Authority))
                                                      .Build();
        }

        public async Task<string> GetAccessToken()
        {
                string[] scopes = new string[] { $"{config.ApiUrl}.default" };
            // If there is no saved user account, the user must sign-in
            //if (!string.IsNullOrEmpty(_token))
            //    return _token;

            try
            {
                // Invoke device code flow so user can sign-in with a browser
                var result = await _msalClient.AcquireTokenForClient(scopes)
                .ExecuteAsync();
                
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("Token acquired");
                //Console.ResetColor();

                //_token = result.AccessToken;
                return result.AccessToken;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error getting access token: {exception.Message}");
                return null;
            }
        }

        // This is the required method to implement IAuthenticationProvider
        // The Graph SDK will call this method each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}