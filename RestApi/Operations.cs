using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
    }
}
