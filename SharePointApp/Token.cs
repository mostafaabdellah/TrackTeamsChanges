using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace SPTokenService
{
    public class RequestState
    {
        // This class stores the request state of the request.
        public WebRequest request;
        public RequestState()
        {
            request = null;
        }
    }
    public class ResponseState
    {
        // This class stores the response state of the request.
        public WebResponse response;
        public ResponseState()
        {
            response = null;
        }
    }
    class RetrieveSPListData
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("  ###################################  ");
                Console.WriteLine("  Read SP online Data   ");
                Console.WriteLine("  ###################################  ");


                // To create a new app go to url :  https://< Jump ;sitename>.sharepoint.com/_layouts/15/appregnew.aspx
                // To set the permission https://< Jump ;sitename>.sharepoint.com/_layouts/15/appinv.aspx
                // To list : http://< Jump ;SharePointWebsite> /_layouts/15/AppPrincipals.aspx
                // XML : <AppPermissionRequests AllowAppOnlyPolicy="true">
                //  <AppPermissionRequest Scope="http://sharepoint/content/sitecollection/web Jump " Right="Read" />
                //  </AppPermissionRequests>


                #region URL
                WebRequest myWebRequest;
                //string stGetTenantDetailsUrl = "https://mmoustafa.sharepoint.com/_vti_bin/client.svc/" +"0.3";
                string stGetTenantDetailsUrl = "https://mmoustafa-admin.sharepoint.com";
                string stGetAccessTokenUrl = "https://accounts.accesscontrol.windows.net/{0}/tokens/OAuth/2 ";

                string tenantID = string.Empty;
                string resourceID = string.Empty;
                string accessToken = string.Empty;

                //string stClientID = "7c1d7a84-1d0e-4851-b034-0da1d90e18ed"; // pass the client id which is created in Step 1, https://< Jump ;sitename>.sharepoint.com/_layouts/15/appregnew.aspx
                //string stClientSecret = "mp4W1atuvcIdXkchghhJigArwsaF6mac/6wrFaQHvU4="; // pass the client secret code which is created in step 1

                string stClientID = "3e16ec1a-e298-44b6-82b0-bb5fc5847a1c"; // pass the client id which is created in Step 1, https://< Jump ;sitename>.sharepoint.com/_layouts/15/appregnew.aspx
                string stClientSecret = "vhRPs/WQ3MJjeuZQJqcZfF+UiiqLNuu9cG0i4inookY="; // pass the client secret code which is created in step 1

                string stSiteDomain = "mmoustafa.sharepoint.com";
                string stSiteDetailsUrl = "https://mmoustafa.sharepoint.com/sites/CustomerTeamBWS/_api/Web/Lists";

                #endregion


                #region Get Tenant ID by HttpRequest

                //read the url https://< Jump ;sitename>/sharepoint.com/_vti_bin/client.svc
                // get the http header of WWW-Authenticate
                // and get the Realm value
                // and the resource value (which will come in Client id attribute

                myWebRequest = WebRequest.Create(stGetTenantDetailsUrl);
                myWebRequest.Method = "GET";
                myWebRequest.Headers.Add("Authorization", "Bearer");


                WebResponse myWebResponse = null; ;
                try
                {

                    myWebResponse = myWebRequest.GetResponse();

                }
                catch (System.Net.WebException ex)
                {
                    //get the Web exception and read the headers
                    #region Parse the WebException and read the WWW-Authenticate

                    string[] headerAuthenticateValue = ex.Response.Headers.GetValues("WWW-Authenticate");
                    if (headerAuthenticateValue != null)
                    {
                        //get the array separated by comma
                        //Console.WriteLine(" Value => " + headerAuthenticateValue.Length);

                        foreach (string stHeader in headerAuthenticateValue)
                        {
                            string[] stArrHeaders = stHeader.Split(',');
                            //loop all the key value pair of WWW-Authenticate

                            foreach (string stValues in stArrHeaders)
                            {
                                // Console.WriteLine(" Value =>" + stValues);
                                if (stValues.StartsWith("Bearer realm="))
                                {
                                    tenantID = stValues.Substring(14);
                                    tenantID = tenantID.Substring(0, tenantID.Length - 1);
                                }
                                if (stValues.StartsWith("client_id="))
                                {
                                    //this value is consider as resourceid which is required for getting the access token
                                    resourceID = stValues.Substring(11);
                                     resourceID = resourceID.Substring(0, resourceID.Length - 1);
                                }
                            }

                        }

                    }

                    #endregion

                }
                Console.WriteLine(" Tenant ID " + tenantID);
                Console.WriteLine(" Resource ID " + resourceID);


                #endregion

                #region Get Access Token using TenantID and App secret ID & Password
                // URL Format
                //https://accounts.accesscontrol.windows.net/tenant_ID/tokens/OAuth/2 Jump

                stGetAccessTokenUrl = string.Format(stGetAccessTokenUrl, tenantID);
                myWebRequest = WebRequest.Create(stGetAccessTokenUrl);
                myWebRequest.ContentType = "application/x-www-form-urlencoded";
                myWebRequest.Method = "POST";


                // Add the below body attributes to the request
                /*
                 *  grant_type  client_credentials  client_credentials
                 client_id  ClientID@TenantID 
                 client_secret  ClientSecret 
                 resource  resource/SiteDomain@TenantID  resourceid/abc.sharepoint.com@tenantID
                 */


                var postData = "grant_type=client_credentials";
                postData += "&client_id=" + stClientID + "@" + tenantID;
                postData += "&client_secret=" + stClientSecret;
                postData += "&resource=" + resourceID + "/" + stSiteDomain + "@" + tenantID;
                var data = Encoding.ASCII.GetBytes(postData);

                using (var stream = myWebRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)myWebRequest.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                string[] stArrResponse = responseString.Split(',');

                //get the access token and expiry time ,etc

                foreach (var stValues in stArrResponse)
                {

                    if (stValues.StartsWith("\"access_token\":"))
                    {
                        //Console.WriteLine(" Result => " + stValues);
                        accessToken = stValues.Substring(16);
                        //Console.WriteLine(" Result => " + accessToken);
                        accessToken = accessToken.Substring(0, accessToken.Length - 2);
                        // Console.WriteLine(" Result => " + accessToken);
                    }
                }






                #endregion

                #region Call REST Service

                //https://< Jump ;sitename>.sharepoint.com/_api/web?$select=Title
                myWebRequest = null;
                myWebRequest = WebRequest.Create(stSiteDetailsUrl);




                // Add the below headers attributes to the request
                /*
                 *  Accept  application/json;odata=verbose  application/json;odata=verbose
                 Authorization  <token_type> <access_token> Bearer eyJ0eX….JQWQ
                 */

                // myWebRequest.Headers.Add("Accept", "application/json;odata=verbose");
                myWebRequest.ContentType = "application/json;odata=verbose";
                myWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
                myWebRequest.ContentLength = 0;
                myWebRequest.Method = "GET";
                
                //call Asyncronously
                //RequestState objRequestState = new RequestState();
                //objRequestState.request = myWebRequest;
                // myWebRequest.BeginGetResponse(GetResponseCallBack, objRequestState);

                var responseREST = (HttpWebResponse)myWebRequest.GetResponse();
                ResponseState objResponseState = new ResponseState();
                objResponseState.response = responseREST;
                myWebRequest.BeginGetResponse(GetResponseCallBack, objResponseState);

                //var responseRestString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                // Console.WriteLine(" Site Details " + responseRestString);

                #endregion

                Console.ReadLine();


            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error occured " + ex.Message);
                Console.ReadLine();
            }
        }
        static void GetResponseCallBack(IAsyncResult asynchronousResult)
        {
            try
            {
                //RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                //WebRequest myWebRequest = myRequestState.request;
                ResponseState myResponseState = (ResponseState)asynchronousResult.AsyncState;
                Console.WriteLine(" Site Info " + new StreamReader(myResponseState.response.GetResponseStream()).ReadToEnd());

            }
            catch (Exception ex)
            {

            }

        }

    }
}