namespace RestApi
{
    public class SubscriptionPost 
    {
        public string Resource { get; set; }
        public string NotificationUrl { get; set; } = "https://55be959674aa.ngrok.io/api/spwebhook/handlerequest";
        public string ExpirationDateTime { get; set; } = "2021-10-27T16:17:57+00:00";
        public string ClientState { get; set; } = "A0A354EC-97D4-4D83-9DDB-144077ADB449";

    }
}
