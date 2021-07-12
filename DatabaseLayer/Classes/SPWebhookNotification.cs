using System;
using System.ComponentModel.DataAnnotations;

public class SPWebhookNotification
{
    [Key]
    public int Id { get; set; }
    public string SubscriptionId { get; set; }

    public string ClientState { get; set; }

    public string ExpirationDateTime { get; set; }

    public string Resource { get; set; }

    public string TenantId { get; set; }

    public string SiteUrl { get; set; }

    public string WebId { get; set; }
    public DateTime NotificationDate { get; set; }
}