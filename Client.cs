//using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AllbotHttp
{

    public class Client
    {
        private string _email, _password;
        public string token = null;
        private bool _debug = false, uselognet = false;

        private log4net.ILog _lognet = null;

        private const string baseUrl = "https://allbot.top/";
        private Routes currentroutes = new Routes();

        HttpClient client;


        private DateTime tokenTime;

        public Client(string email, string password, bool debug = false , TextWriterTraceListener traceListner = null, log4net.ILog lognet = null)
        {
            //
            // Summary:
            //     Create Http Client to send sms and whatsapp messages
            //
            //
            // Parameters:
            //   email:
            //     login email.
            //   password:
            //     login password.
            //   logger:
            //     opitional Iloger instance .
            //

            client = new HttpClient();

            _email = email;
            _password = password;
            _debug = debug;
            _lognet = lognet;
            uselognet = _lognet != null;

            Trace.AutoFlush = true;


            if (traceListner != null)
            {
                Trace.Listeners.Add(traceListner);
            }


            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(120);

        }

        private void Log(string msg)
        {
            if (_debug)
            {
                string logmsg = $"AllbotHTTP: {DateTime.Now} : {msg} ";//"AllbotHTTP " + " " + msg;

                if (uselognet)
                {
                    _lognet.Info(logmsg);
                }
                else
                {


                    Trace.Indent();
                    Trace.WriteLine(logmsg);
                    //Trace.TraceError(logmsg);

                    Trace.Unindent();

                }

            }
        }

        private void LogError(string msg)
        {
            if (_debug)
            {
                string logmsg = $"AllbotHTTP: {DateTime.Now} : {msg} ";//"AllbotHTTP " + " " + msg;

                if (uselognet)
                {
                    _lognet.Error(logmsg);
                }
                else
                {
                    //string logmsg = "AllbotHTTP " + " " + msg;
                    //Log(logmsg);

                    Trace.Indent();
                    //Trace.WriteLine(logmsg);
                    Trace.TraceError(logmsg);

                    Trace.Unindent();
                }
            }
        }

        private async Task<Boolean> setToken()
        {
            if (!string.IsNullOrEmpty(token) && tokenTime < DateTime.Now.AddMinutes(-60))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Log("token is set");

                return true;
            }
            else
            {
                Log("no token or expired trying to log in");
                token = await LoginAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Log("token is set");
                    return true;
                }
            }
            return false;
        }


        public async Task<SessionInfo> CreateSessionAsync(SessionInfo sessionInfo)
        {
            SessionInfo session = null;

            try
            {
                if (!await setToken())
                {
                    LogError("can not set token");
                    return session;
                }

                HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.session_route, sessionInfo);


                Log("response " + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    token = null;
                }

                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Log("Internal server error");
                }


                else if (response.IsSuccessStatusCode)
                {
                    session = await response.Content.ReadAsAsync<SessionInfo>();
                }

                return session;
            }
            catch (HttpRequestException err)
            {
                Log(err.ToString());
                return session;
            }


        }


        public async Task<SessionInfo> StatusSessionAsync(SessionInfo sessionInfo)
        {
            SessionInfo session = null;

            try
            {
                if (!await setToken())
                {
                    LogError("can not set token");
                    return session;
                }

                HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.status_route, sessionInfo);


                Log("response " + response.StatusCode.ToString());
                //Log("error " + response.ReasonPhrase);

                //remove token if stale or removed or unauthorized
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    token = null;
                }

                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Log("Internal server error");
                }


                else if (response.IsSuccessStatusCode)
                {
                    session = await response.Content.ReadAsAsync<SessionInfo>();
                }

                return session;
            }
            catch (HttpRequestException err)
            {
                Log(err.ToString());
                return session;
            }


        }

        public async Task<string> LoginAsync()
        {
            TokenInfo token = null;


            Log("start logging in");
            var httpContent = new StringContent("{}", Encoding.UTF8, "application/json");
            try
            {
                var url = $"{baseUrl}{currentroutes.login_route}?email={_email}&password={_password}";
                Log(url);
                HttpResponseMessage response = await client.PostAsync(url, httpContent);

                Log("login in response code :" + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    token = null;
                    LogError("Unauthorized login in");
                }

                if (response.IsSuccessStatusCode)
                {
                    token = await response.Content.ReadAsAsync<TokenInfo>();

                    SaveToken(token.access_token);
                    //setToken();
                    return token.access_token;
                }

                return null;
            }
            catch (HttpRequestException err)
            {
                LogError(err.ToString());
                return null;
            }


        }

        public async Task<bool> TestConnection()
        {

            try
            {
                if (!await setToken())
                {
                    LogError("can not set token");
                    return false;
                }

                Log("testing connection");
                var response = await client.GetAsync(currentroutes.testconnection_route);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    token = null;
                }

                Log("response " + response.StatusCode.ToString());

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException err)
            {
                LogError(err.ToString());
                return false;
            }


        }

        private async Task<MessageInfo> SendMessageAsync(MessageInfo messageInfo)
        {
            MessageInfo mInfo = null;

            try
            {
                if (!await setToken())
                {
                    LogError("can not set token");
                    return mInfo;
                }

                HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.send_message_route, messageInfo);

                Log("response " + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    token = null;
                    LogError("unauthorized token");
                    return mInfo;
                }

                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Log("Internal server error");
                }


                else if (response.IsSuccessStatusCode)
                {
                    mInfo = await response.Content.ReadAsAsync<MessageInfo>();
                    mInfo.message = messageInfo.message;
                    mInfo.mobile = messageInfo.mobile;
                    /*if (mInfo.status != "ok")
                    {
                        //MessagesForm.AddMessage(mInfo);
                    }*/

                }

                return mInfo;
            }
            catch (HttpRequestException err)
            {
                LogError(err.ToString());
                messageInfo.error = "can not connect";
                //MessagesForm.AddMessage(messageInfo);
                return mInfo;
            }

        }


        public async Task<MessageInfo> SendMessageAsync(string mobile , string msg , bool sendWhats , bool sendSms , bool hide_message = false)
        {

            var messageInfo = new MessageInfo { mobile = mobile, message = msg, whats = sendWhats, sms = sendSms, hide_message = hide_message };
            return await SendMessageAsync(messageInfo);
        }

        public async Task<MessageInfo> SendSMSAsync(string mobile, string msg, bool hide_message = false)
        {

            var messageInfo = new MessageInfo { mobile = mobile, message = msg, whats = false, sms = true, hide_message = hide_message };
            return await SendMessageAsync(messageInfo);
        }

        public async Task<MessageInfo> SendWhatsAsync(string mobile, string msg, bool hide_message = false)
        {

            var messageInfo = new MessageInfo { mobile = mobile, message = msg, whats = true, sms = false , hide_message= hide_message };
            return await SendMessageAsync(messageInfo);
        }
        private void SaveToken(String t)
        {
            token = t;
            tokenTime = DateTime.Now;
        }

    }
}

