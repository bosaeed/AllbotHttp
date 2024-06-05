using System;
using System.Collections.Generic;

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

        public bool is_managment { get; set; } = false;

        public Dictionary<string,object> keyValuePairs { get; set; }

        public string file64 { get; set; } = "";

        public string doctype { get; set; } = "";



        public string buttonText { get; set; } = "";

        public string buttonDescription { get; set; } = "";
        public string buttonTitle { get; set; } = "";
        
        public string buttonFooter { get; set; } = "";

        public ListSection[] sections { get; set; } = null;



        public override string ToString()
        {
            return $"{mobile} ,\n{message} ,\n{status} ,\n{error}";
        }

    }



    public class ListRow
    {
        public string rowId { get; set; }
        public string title { get; set; }
        public string description { get; set; }

    }

    public class ListSection
    {
        public string title { get; set; }
        public ListRow[] rows { get; set; }


    }
}
