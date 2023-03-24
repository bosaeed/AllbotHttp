using System;

namespace AllbotHttp
{
    public class MessageInfo
    {
        public Guid id { get; set; }
        public string mobile { get; set; }

        public string message { get; set; }

        public string status { get; set; }

        public string error { get; set; }

        public bool whats { get; set; }

        public bool sms { get; set; }

        public bool hide_message { get; set; } = false;

        public override string ToString()
        {
            return $"{mobile} ,\n{message} ,\n{status} ,\n{error}";
        }

    }
}
