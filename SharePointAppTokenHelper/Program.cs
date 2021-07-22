using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointAppTokenHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var tenantStr = "mmoustafa";
            var tenantAdminUri = new Uri(string.Format("https://{0}.sharepoint.com", tenantStr));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);

            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
        }
    }
}
