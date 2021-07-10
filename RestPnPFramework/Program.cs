using PnP.Framework;
using System;

namespace RestPnPFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var ClientId = "ec30785e-b676-46c9-beba-70a8faf2d5d7";
            var ClientSecret = "2kcm8HeK05ytW4nGOM~.ERb242vPDa2~J~";
            string siteUrl = "https://mmoustafa.sharepoint.com/sites/Public853";
            using (var cc = new AuthenticationManager().GetContext(GetAzureADLoginEndPoint(AzureEnvironment.Production))
            {
                cc.Load(cc.Web, p => p.Title);
                cc.ExecuteQuery();
                Console.WriteLine(cc.Web.Title);
            };
        }
    }
}
