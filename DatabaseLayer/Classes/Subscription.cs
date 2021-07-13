using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class Subscription
    {
        [Key]
        [JsonProperty("id")]
        public string SubscriptionId { get; set; }
        public string TeamId { get; set; }
        public string NotificationUrl { get; set; }
        public string ClientState { get; set; }
        public string ExpirationDateTime { get; set; }
        public string Resource { get; set; }
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        [JsonProperty("odata.type")]
        public string Type { get; set; }
        [JsonProperty("odata.id")]
        public string Oid { get; set; }
        [JsonProperty("odata.editLink")]
        public string EditLink { get; set; }

    }
public class SubscriptionGraph
{
    [Key]
    [JsonProperty("id")]
    public string SubscriptionId { get; set; }
    public string TeamId { get; set; }
    public string NotificationUrl { get; set; }
    public string ClientState { get; set; }
    public string ExpirationDateTime { get; set; }
    public string Resource { get; set; }
    [JsonProperty("odata.metadata")]
    public string Metadata { get; set; }
    [JsonProperty("odata.type")]
    public string Type { get; set; }
    [JsonProperty("odata.id")]
    public string Oid { get; set; }
    [JsonProperty("odata.editLink")]
    public string EditLink { get; set; }
    public string ChangeType { get; set; }
    public string ApplicationId { get; set; }

}
