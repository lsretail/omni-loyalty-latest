using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using LSRetail.Omni.Domain.DataModel.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Base
{
    public abstract class BaseRepository
    {
        /// <summary>
        /// Gets or sets the URL. http://mobiledemo.lsretail.com/LSOmniService/json.svc
        /// </summary>
        /// <value>
        /// The URL. http://mobiledemo.lsretail.com/LSOmniService/json.svc
        /// </value>
        protected string Url { get; set; }
        //public string Method { get; set; } 
        /// <summary>
        /// Stores the response from the last request. Good for debugging.
        /// </summary>
        public string JsonRepsonse { get; set; }
        /// <summary>
        /// Stores the latest request. Good for debugging.
        /// </summary>
        public string JsonRequest { get; set; }
        protected string UniqueDeviceId { get; set; } //unique id of device

        /// <summary>
        /// Timeout in seconds
        /// </summary>
        protected int Timeout { get; set; }
        public string SecurityToken = string.Empty;
        protected string version = string.Empty; //this version is sent to server
        protected string languageCode = string.Empty; //"en-US";

        private string baseUrl { get; set; }   //http://mobiledemo.lsretail.com
        private string resource { get; set; }  //LSOmniService/json.svc

        //basic auth
        private string authUser = string.Empty; //"LSOmniServiceUser";
        private string authPwd = string.Empty; //"XYZ.LSOmniServiceUser.321";

        protected BaseRepository()
        {
            //cant do much in base since config values can change until call to web service
        }

        //you can either set the ConfigSettings or use the ConfigStatic
        private void SetConfigProperties()
        {
            //get values from config 
            this.SecurityToken = ConfigStatic.SecurityToken; //null not allowed
            this.Timeout = ConfigStatic.Timeout; //default sec
            this.Url = ConfigStatic.Url;
            this.version = ConfigStatic.Version;
            this.languageCode = ConfigStatic.LanguageCode;
            this.UniqueDeviceId = ConfigStatic.UniqueDeviceId;
            this.authUser = ConfigStatic.AuthUser;
            this.authPwd = ConfigStatic.AuthPassword;
        }

        private HttpClient BuildHttpClient(int timeout = 0)
        {
            SetConfigProperties();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //client.DefaultRequestHeaders.Add("X-Auth-Token", "lsomni"); //Auth-Token
            client.DefaultRequestHeaders.Add("LSRETAIL-TOKEN", (string.IsNullOrEmpty(this.SecurityToken) ? "x" : this.SecurityToken));
            client.DefaultRequestHeaders.Add("LSRETAIL-VERSION", this.version);
            client.DefaultRequestHeaders.Add("LSRETAIL-LANGCODE", this.languageCode);
            client.DefaultRequestHeaders.Add("LSRETAIL-DEVICEID", this.UniqueDeviceId);

            if (timeout == 0)
            {
                client.Timeout = TimeSpan.FromMilliseconds(this.Timeout * 1000);// new TimeSpan(0, 0, 0, 0, this.Timeout * 1000); //millisecs
                client.DefaultRequestHeaders.Add("LSRETAIL-TIMEOUT", this.Timeout.ToString()); //timeout in seconds
            }
            else
            {
                client.Timeout = TimeSpan.FromMilliseconds(timeout * 1000);// new TimeSpan(0, 0, 0, 0, this.Timeout * 1000); //millisecs
                client.DefaultRequestHeaders.Add("LSRETAIL-TIMEOUT", timeout.ToString()); //timeout in seconds
            }
            client.DefaultRequestHeaders.Add("user-agent", "LSOmniServicePCL-" + this.version); //  

            //basic auth
            //string basicauth = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.authUser, this.authPwd)));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicauth);

            return client;
        }

        protected T PostData<T>(object jObject, string method, int timeout = 0)
        {
            T DTOOut = default(T);
            try
            {
                using (var client = BuildHttpClient(timeout))
                {
                    method = method.Replace("/", ""); // strip 
                    string requestUri = this.Url + "/" + method; //or this.baseUrl + this.resource + "/" + method; //works in PostAsync


                    this.JsonRequest = JsonConvert.SerializeObject(jObject, GetSerSettings()); //store the last request in JsonRequest
                    var stringContent = new StringContent(this.JsonRequest, Encoding.UTF8, "application/json");

                    //HttpResponseMessage response = PostSafe.PostAsyncSafe(client, requestUri, stringContent); //handles exceptions
                    using (HttpResponseMessage response = client.PostAsync(requestUri, stringContent).Result)
                    {
                        //response.EnsureSuccessStatusCode(); //throws exception also calls dispose so not good
                        if (response.IsSuccessStatusCode)
                        {
                            // by calling .Result you are synchronously reading the result
                            JsonRepsonse = response.Content.ReadAsStringAsync().Result;
                            if (string.IsNullOrWhiteSpace(JsonRepsonse))
                                throw new LSOmniException(StatusCode.Error, "JsonRepsonse IsNullOrWhiteSpace");

                            // trick to get rid of the rootname  ex."CouponsGetByContactIdResult"
                            JObject jo = JObject.Parse(JsonRepsonse);
                            if (jo != null)
                            {
                                var theFirst = jo.First;
                                if (theFirst != null)
                                {
                                    var secondToken = theFirst.First;
                                    if (secondToken != null)
                                    {
                                        JsonRepsonse = secondToken.ToString();
                                        //string xJsonRepsonse = secondToken.ToString();
                                        //handle true false differently
                                        if (typeof(T) == typeof(bool))
                                            return (T)((object)Convert.ToBoolean(JsonRepsonse));
                                        else if (typeof(T) == typeof(int))
                                            return (T)((object)Convert.ToInt32(JsonRepsonse));
                                        else if (typeof(T) == typeof(byte))
                                            return (T)((object)Convert.ToByte(JsonRepsonse));
                                        else if (typeof(T) == typeof(long))
                                            return (T)((object)Convert.ToInt64(JsonRepsonse));
                                        else if (typeof(T) == typeof(string))
                                            return (T)((object)Convert.ToString(JsonRepsonse));
                                        else if (typeof(T) == typeof(decimal))
                                            return (T)((object)Convert.ToDecimal(JsonRepsonse));

                                        //DTOOut = (T)secondToken.ToObject<T>();
                                        //int idx = JsonRepsonse.IndexOf(":") + 1;
                                        //JsonRepsonse = JsonRepsonse.Substring(idx, JsonRepsonse.Length - idx - 1);
                                        DTOOut = (T)JsonConvert.DeserializeObject<T>(JsonRepsonse, GetSerSettings());
                                    }
                                    else
                                        throw new LSOmniException(StatusCode.Error, "secondToken is null");
                                }
                                else
                                    throw new LSOmniException(StatusCode.Error, "theFirst is null");

                                jo = null;
                            }
                            else
                                throw new LSOmniException(StatusCode.Error, "JObject parse returned null");
                        }
                        else if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                        {
                            //json errors comes back with HttpStatusCode.RequestedRangeNotSatisfiable
                            JsonRepsonse = response.Content.ReadAsStringAsync().Result.Replace("\\\"", "\"").Trim('\"');

                            WebServiceFault fault = JsonConvert.DeserializeObject<WebServiceFault>(JsonRepsonse, GetSerSettings());
                            string msg = string.Format("{0} - [Method: {1}] - RequestedRangeNotSatisfiable", fault.FaultMessage, method);
                            throw new LSOmniException((StatusCode)fault.FaultCode, msg);
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            string msg = string.Format("HTTP Status: {0} - Reason: {1}   Uri:{2}",
                                response.StatusCode, response.ReasonPhrase, response.RequestMessage.RequestUri.AbsoluteUri);
                            throw new LSOmniException(StatusCode.CommunicationFailure, msg);
                        }
                        else
                        {
                            JsonRepsonse = response.Content.ReadAsStringAsync().Result;

                            //response.RequestMessage //shows full url of request
                            string msg = string.Format("HTTP Status: {0} - Reason: {1} ", response.StatusCode, response.ReasonPhrase);
                            throw new LSOmniException(StatusCode.Error, msg);
                        }
                    }
                }
            }
            catch (LSOmniException lex)
            {
                throw lex;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return DTOOut;
        }

        private JsonSerializerSettings GetSerSettings()
        {
            //Note, the server returns Utc without any timezone info in it
            // otherwise we get datetime with offset /Date(1417195800000-0600)\/
            //that the deserializer cannot deal with!! never take into account the offset arrggg.. DateTimeZoneHandling not working
            //if server sends on Utc /Date(1417195800000)\/, only then can the client deal with it, without the -0600 

            //Note: Server is not converting anything into UTC, just specifies it as such with same date
            // DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);    so json dates come correctly over

            JsonSerializerSettings sett = new JsonSerializerSettings();
            sett.Formatting = Formatting.None;
            sett.DateTimeZoneHandling = DateTimeZoneHandling.Utc;//.Unspecified;// RoundtripKind;//Utc works(none,msdate)
            sett.DateParseHandling = DateParseHandling.None;
            sett.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            sett.FloatParseHandling = FloatParseHandling.Decimal;
            sett.DefaultValueHandling = DefaultValueHandling.Populate;
            sett.FloatFormatHandling = FloatFormatHandling.DefaultValue;
            //PCL does not have a way to set culture!  Must be done in calling thread
            sett.Culture = CultureInfo.InvariantCulture; // need the CultureInfo so . dont change to , for double
            return sett;
        }

        private void HandleException(Exception ex)
        {
            if (ex.GetType() == typeof(AggregateException))
            {
                AggregateException agex = (AggregateException)ex;
                if (agex.InnerException != null && agex.InnerException.GetType() == typeof(System.Net.Http.HttpRequestException))
                {
                    HttpRequestException hrex = (HttpRequestException)agex.InnerException;
                    string msg = string.Format("HTTP Status: {0} - Reason: {1}   HResult: {2}",
                        "AggregateException-HttpRequestException 1", hrex.Message, hrex.HResult);
                    throw new LSOmniException(StatusCode.CommunicationFailure, msg, hrex);
                }
                else if (agex.InnerException != null && agex.InnerException.GetType() == typeof(System.Threading.Tasks.TaskCanceledException))
                {
                    //usually get this when we timeout
                    System.Threading.Tasks.TaskCanceledException te = (System.Threading.Tasks.TaskCanceledException)agex.InnerException;
                    string msg = string.Format("HTTP Status: {0} - Reason: {1} AggregateException-TaskCanceledException 1", te.Task.Status, "Timeout " + te.Message);
                    throw new LSOmniException(StatusCode.CommunicationFailure, msg, te);
                }
                else if (agex.InnerException != null)
                {
                    int i = 1; //just in case 
                    string msg = msg = string.Format("HTTP Status: {0} - Reason: {1} ",
                        "AggregateException unknown 1 " + agex.InnerException.GetType().ToString(), agex.InnerException.Message);
                    foreach (Exception exInnerException in agex.Flatten().InnerExceptions)
                    {
                        //Get message from all aggregated exceptions
                        Exception exNestedInnerException = exInnerException;
                        do
                        {
                            if (exNestedInnerException != null && !string.IsNullOrEmpty(exNestedInnerException.Message))
                            {
                                msg += " *" + i.ToString() + "* " + exNestedInnerException.Message;
                            }
                            exNestedInnerException = exNestedInnerException.InnerException;
                            i++;
                        }
                        while (exNestedInnerException != null && i <= 10);
                    }

                    throw new LSOmniException(StatusCode.CommunicationFailure, msg, agex);
                }              
                else
                {
                    string msg = string.Format("HTTP Status: {0} - Reason: {1} ", "AggregateException unknown 2 " + agex.InnerException.GetType().ToString(), agex.Message);
                    throw new LSOmniException(StatusCode.CommunicationFailure, msg, agex);
                }
            }
            else if (ex.GetType() == typeof(HttpRequestException))
            {
                HttpRequestException hrex = (HttpRequestException)ex;
                string msg = string.Format("HTTP Status: {0} - Reason: {1}   HResult: {2}", "HttpRequestException-unknown", hrex.Message, hrex.HResult);
                throw new LSOmniException(StatusCode.CommunicationFailure, msg, hrex);
            }
            else throw new LSOmniException(StatusCode.Error, ex.Message, ex);
        }
    }
}

