using Newtonsoft.Json;
using System;
using System.IO;

namespace RestApi
{
    public class AzureAdSetting
    {
        public string TenantId { get; set; }//"yourtenant.onmicrosoft.com";
        public string ClientId { get; set; }//"your client id";
        public string ResourceId { get; set; }//"https://outlook.office.com/";
        public string ResourceUrl { get; set; }//"https://outlook.office.com/api/v2.0/users/service@contoso.com/sendmail"; //this is your on-behalf user's UPN
        public string CertficatePath { get; set; }//@"c:\test.pfx"; //this is your certficate location.
        public string CertificatePassword { get; set; }//"xxxx"; // this is your certificate password
        public string SendEmail { get; set; } //the email you want to send to.

        public string Authority { get { return String.Format("https://login.windows.net/{0}", TenantId); } }

        public static AzureAdSetting CreateInstance(string jsonFile)
        {
            using (StreamReader sr = new StreamReader(jsonFile))
            {
                string json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<AzureAdSetting>(json);
            }
        }
    }
}
