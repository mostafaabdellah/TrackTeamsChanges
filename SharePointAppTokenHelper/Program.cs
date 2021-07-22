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
            string stClientID = "3e16ec1a-e298-44b6-82b0-bb5fc5847a1c";
            string stClientSecret = "vhRPs/WQ3MJjeuZQJqcZfF+UiiqLNuu9cG0i4inookY=";

            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
        }
    }
}
