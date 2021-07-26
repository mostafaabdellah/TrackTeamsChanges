using Newtonsoft.Json;

namespace WebhookReceiver.Models
{
    public class TeamsNotification
    {
        public string changeType { get; set; }
        [JsonProperty("id")]
        public string TeamId { get; set; }
        public string clientState { get; set; }
        public string resource { get; set; }
        public string resourceData { get; set; }
        public string organizationId { get; set; }
        [JsonProperty("odata.id")]
        public string Oid { get; set; }
        [JsonProperty("members@delta")]
        public string members { get; set; }
        public string eventTime { get; set; }
        public string sequenceNumber { get; set; }
        public string subscriptionExpirationDateTime { get; set; }
        public string subscriptionId { get; set; }
        public string tenantId { get; set; }


        //{"value":[{
        //"changeType":"updated",
        //"clientState":"A0A354EC-97D4-4D83-9DDB-144077ADB449",
        //"resource":"Groups/14b0fed8-8714-4140-b0da-f05759d10b5d"
        //,"resourceData":{"@odata.type":"#Microsoft.Graph.Group",
        //"@odata.id":"Groups/14b0fed8-8714-4140-b0da-f05759d10b5d",
        //"id":"14b0fed8-8714-4140-b0da-f05759d10b5d",
        //"organizationId":"0d4ca527-dc44-43d1-84c1-b63d1b1e024d",
        //"eventTime":"2021-07-26T02:40:12.0385565Z",
        //"sequenceNumber":637628640120385565,
        //members@delta":[{"id":"636b5365-803f-4703-8cd8-a39d473fe31a"}]},
        //"subscriptionExpirationDateTime":"2021-08-05T04:00:00-07:00",
        //"subscriptionId":"26a5aadf-b10d-425c-98a4-aa9c2e0b0b45",
        //"tenantId":"0d4ca527-dc44-43d1-84c1-b63d1b1e024d"}]}
    }
}