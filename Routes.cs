using System;

namespace AllbotHttp
{
    public class Routes
    {
        public Routes(String session_route = "api/start_session",
                String status_route = "api/status_session",
               String send_message_route = "api/send_message",
               String testconnection_route = "api/testconnection",
               String login_route = "api/login"
               )
        {

            this.session_route = session_route;
            this.status_route = status_route;
            this.send_message_route = send_message_route;
            this.testconnection_route = testconnection_route;
            this.login_route = login_route;
            //this.test_route = test_route;

        }
        public string session_route;
        public string status_route;
        public string send_message_route;
        public string testconnection_route;
        //public string test_route;
        public string login_route;

        public override string ToString()
        {
            return $"{session_route} \n{send_message_route} \n{testconnection_route} \n{login_route}\n";
        }
    }
}
