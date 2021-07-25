using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class RemoteEventReceiver
{
    [Key]
    public int Id { get; set; }
    [JsonProperty("ReceiverId")]
    public string ReceiverId { get; set; }
    public string TeamId { get; set; }
    public string ReceiverUrl { get; set; }
    public int EventType { get; set; }
    [JsonProperty("odata.id")]
    public string Oid { get; set; }
    [JsonProperty("odata.metadata")]
    public string Metadata { get; set; }
    [JsonProperty("odata.type")]
    public string Type { get; set; }

}
