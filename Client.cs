using AllbotHttp.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Diagnostics;
using System.Net.Http;
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

        RestClient client;
        JwtAuthenticator authenticator;
        private DateTime tokenTime;

        public Client(string email, string password, bool debug = false, TextWriterTraceListener traceListner = null, log4net.ILog lognet = null)
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

            //client = new HttpClient();
            authenticator = new JwtAuthenticator("asdasdasdasd");
            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = authenticator
            };
            client = new RestClient(options);

            client.AddDefaultHeader("Accept", "application/json");
            client.AddDefaultHeader("Content-Type", "application/json");
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


        }

        public void UpdateCred(string email, string password)
        {
            _email = email;
            _password = password;
        }

        private void Log(string msg)
        {
            if (_debug)
            {
                string logmsg = $"AllbotHTTP: {msg}";

                if (uselognet)
                {
                    _lognet.Debug(logmsg);
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
                string logmsg = $"AllbotHTTP: {msg}";

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
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                authenticator.SetBearerToken(token);

                Log("token is set");

                return true;
            }
            else
            {
                Log("no token or expired trying to log in");
                token = await LoginAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    authenticator.SetBearerToken(token);
                    Log("token is set");
                    return true;
                }
            }
            return false;
        }


        public async Task<SubscriptionInfo> SubscriptionInfo(string deviceid)
        {
            SubscriptionInfo subscriptionInfo = null;
            var deviceInfo = new DeviceInfo() { deviceuid = deviceid };
            try
            {
                if (!await setToken())
                {
                    LogError("can not set token");
                    return subscriptionInfo;
                }

                //HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.subscription_route, deviceInfo);
                var request = new RestRequest(currentroutes.subscription_route, Method.Post);
                request.AddJsonBody(deviceInfo);

                var response = await client.PostAsync<SubscriptionInfo>(request);


                 return subscriptionInfo;
                return response;
            }
            catch (HttpRequestException err)
            {
                Log(err.ToString());
                return subscriptionInfo;
            }
            catch (Exception err)
            {
                Log(err.ToString());
                return subscriptionInfo;
            }

        }

        public async Task<string> LoginAsync(string email = null, string password = null)
        {
            //TokenInfo token = null;

            bool isTest = true;
            if (email is null)
            {
                isTest = false;
                email = _email;
                password = _password;
            }

            Log("start logging in");
            var httpContent = new StringContent("{}", Encoding.UTF8, "application/json");
            try
            {


                Debug.WriteLine("login *********************************");
                var request = new RestRequest(currentroutes.login_route, Method.Post);
                request.AddQueryParameter("email", email);
                request.AddQueryParameter("password", password);

                var response = await client.PostAsync<TokenInfo>(request);



                if (!isTest)
                {

                    SaveToken(response.access_token);
                    //setToken();
                    return response.access_token;
                }
                return "OK";
            }
            catch (HttpRequestException err)
            {
                LogError(err.ToString());
                return null;
            }
            catch (Exception err)
            {
                LogError(err.ToString());
                return null;
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


                var request = new RestRequest(currentroutes.send_message_route, Method.Post);
                request.AddJsonBody(messageInfo);

                var response = await client.PostAsync<MessageInfo>(request);


                return response;
            }
            catch (HttpRequestException err)
            {
                LogError(err.ToString());
                messageInfo.error = "can not connect";
                //MessagesForm.AddMessage(messageInfo);
                return mInfo;
            }
            catch (Exception err)
            {
                Log(err.ToString());
                messageInfo.error = "can not connect";
                return mInfo;
            }
        }


        public async Task<MessageInfo> SendSMSAsync(string mobile, string msg, bool hide_message = false)
        {

            var messageInfo = new MessageInfo { mobile = mobile, message = msg, whats = false, sms = true, hide_message = hide_message };
            return await SendMessageAsync(messageInfo);
        }

        private void SaveToken(String t)
        {
            token = t;
            tokenTime = DateTime.Now;
        }

    }
}

