//using Microsoft.Extensions.Logging;
using AllbotHttp.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Diagnostics;
using System.Net.Http;
//using System.Net.Http.Json;
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

        //HttpClient client;
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


            //client.BaseAddress = new Uri(baseUrl);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Accept", "application/json");
            //client.Timeout = TimeSpan.FromSeconds(120);


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

                //HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.session_route, sessionInfo);
                var request = new RestRequest(currentroutes.session_route, Method.Post);
                request.AddJsonBody(sessionInfo);
                //request.RequestFormat = DataFormat.Json;
                //request.AddHeader("Content-Type", "application/json");
                // The cancellation token comes from the caller. You can still make a call without it.
                var response = await client.PostAsync<SessionInfo>(request);

                /*Log("response " + response.StatusCode.ToString());

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

                return session;*/
                return response;
            }
            catch (HttpRequestException err)
            {
                Log(err.ToString());
                return session;
            }
            catch (Exception err)
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

                //HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.status_route, sessionInfo);
                var request = new RestRequest(currentroutes.status_route, Method.Post);
                request.AddJsonBody(sessionInfo);

                var response = await client.PostAsync<SessionInfo>(request);


                /* Log("response " + response.StatusCode.ToString());
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

                 return session;*/
                return response;
            }
            catch (HttpRequestException err)
            {
                Log(err.ToString());
                return session;
            }
            catch (Exception err)
            {
                Log(err.ToString());
                return session;
            }

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

                /* Log("response " + response.StatusCode.ToString());

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
                     subscriptionInfo = await response.Content.ReadAsAsync<SubscriptionInfo>();
                 }

                 return subscriptionInfo;*/
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
                //var url = $"{baseUrl}{currentroutes.login_route}?email={email}&password={password}";
                //Log(url);
                //HttpResponseMessage response = await client.PostAsync(url, httpContent);

                Debug.WriteLine("login *********************************");
                var request = new RestRequest(currentroutes.login_route, Method.Post);
                request.AddQueryParameter("email", email);
                request.AddQueryParameter("password", password);

                var response = await client.PostAsync<TokenInfo>(request);


                /*Log("login in response code :" + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (!isTest)
                        token = null;
                    LogError("Unauthorized login in");
                }

                if (response.IsSuccessStatusCode)
                {
                    if (!isTest)
                    {

                        token = await response.Content.ReadAsAsync<TokenInfo>();

                        SaveToken(token.access_token);
                        //setToken();
                        return token.access_token;
                    }
                    return "OK";
                }*/

                //return null;

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

                //HttpResponseMessage response = await client.PostAsJsonAsync(currentroutes.send_message_route, messageInfo);

                var request = new RestRequest(currentroutes.send_message_route, Method.Post);
                request.AddJsonBody(messageInfo);

                var response = await client.PostAsync<MessageInfo>(request);

                /*Log("response " + response.StatusCode.ToString());

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
          

                }

                return mInfo;*/
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


        public async Task<MessageInfo> SendMessageAsync(string mobile, string msg, bool sendWhats, bool sendSms, bool hide_message = false)
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

            var messageInfo = new MessageInfo { mobile = mobile, message = msg, whats = true, sms = false, hide_message = hide_message };
            return await SendMessageAsync(messageInfo);
        }
        private void SaveToken(String t)
        {
            token = t;
            tokenTime = DateTime.Now;
        }

    }
}

