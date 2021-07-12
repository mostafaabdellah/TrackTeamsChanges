using System.Collections.Generic;

namespace WebhookReceiver.Models
{
    public class SPWebhookContent
    {
        public List<SPWebhookNotification> Value { get; set; }
    }
}