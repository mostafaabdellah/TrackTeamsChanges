using System.Collections.Generic;

namespace WebhookReceiver.Models
{
    public class SPWebhookContent
    {
        public List<SPWebhookNotification> Value { get; set; }
    }
    public class ContentSubscriptions
    {
        public List<Subscription> Value { get; set; }
    }
    public class TeamsContent
    {
        public List<TeamsNotification> value { get; set; }
    }

}