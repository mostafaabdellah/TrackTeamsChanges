using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;

public class Change
{
    [Key]
    public long Id { get; set; }
    public string TeamId { get; set; }
    public string RestSubscriptionId { get; set; }
    public string GraphSubscriptionId { get; set; }
    public int Iteration { get; set; }
    public string ChangeType { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? RestNotificationOn { get; set; }
    public DateTime? GraphNotificationOn { get; set; }
    public int? RestLatency { get; set; }
    public int? GraphLatency { get; set; }


}
