using System;

namespace AllbotHttp.Models
{
    public class SubscriptionInfo
    {
        public string status { get; set; }

        public bool whats_subscription { get; set; }

        /* "whats_subscription_end_date": "2024-09-11 14:54:45",
     "sms_subscription": false,
     "sms_subscription_end_date": "2024-09-11 14:54:45",
     "error"*/
        public DateTime whats_subscription_end_date { get; set; }
        public bool sms_subscription { get; set; }

        public DateTime sms_subscription_end_date { get; set; }

        public string error { get; set; }

        public override string ToString()
        {
            return $"{status} ,\n whats: {whats_subscription} ,{whats_subscription_end_date}" +
                $"\n SMS: {sms_subscription} ,{sms_subscription_end_date}\n  " +
                $"{error}";
        }
    }

    internal class DeviceInfo
    {
        public string deviceuid { get; set; }
    }
}
