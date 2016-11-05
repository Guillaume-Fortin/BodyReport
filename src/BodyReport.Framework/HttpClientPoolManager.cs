using BodyReport.Message.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BodyReport.Framework
{
    internal class HttpClientUser
    {
        public int LockCount { get; set; } =  0;
        public string UserId { get; set; } = null;
        public string UserIdentityCookie { get; set; } = null;
        public HttpClient HttpClient { get; set; } = null;
    }

    /// <summary>
    /// Manage Single instance of HttpClient for userId
    /// </summary>
    public class HttpClientPoolManager
    {
        private static ILoggerFactory _loggerFactory = new LoggerFactory().AddConsole();
        private static ILogger _logger = _loggerFactory.CreateLogger(typeof(HttpClientPoolManager));

        private Uri _baseAdress = null;
        private int _maxPoolSize;
        private HttpClientUser[] _clients = null;
        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="baseAdress">base adress of whttp service</param>
        /// <param name="maxPoolSize">max pool size for manager</param>
        public HttpClientPoolManager(Uri baseAdress, int maxPoolSize)
        {
            _baseAdress = baseAdress;
            _maxPoolSize = maxPoolSize;
            if (_maxPoolSize < 100)
                _maxPoolSize = 100;
            _clients = new HttpClientUser[_maxPoolSize];
        }

        /// <summary>
        /// Create or reuse HttpClientUser for userId
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>HttpClientUser</returns>
        private HttpClientUser CreateOrReuseClient(string userId)
        {
            HttpClientUser client = null;
            client = _clients.Where(c => c != null && c.UserId == userId).FirstOrDefault();
            if (client == null)
            {
                client = _clients.Where(c => c != null && c.LockCount == 0).FirstOrDefault();
                if (client != null)
                {
                    if(client.HttpClient != null) // Must be diposed
                    {
                        client.HttpClient.Dispose();
                        client.HttpClient = null;
                    }
                }
                else
                {
                    for (int i = 0; i < _clients.Length; i++)
                    {
                        if (_clients[i] == null)
                        {
                            _clients[i] = client = new HttpClientUser() { UserId = userId };
                        }
                    }
                }
            }
            if (client == null)
            {
                _logger.LogCritical("Unable to create HttpClient in HttpClientPool, max client : " + _maxPoolSize);
            }
            return client;
        }

        /// <summary>
        /// Create or Reuse HttpClient
        /// </summary>
        /// <param name="client">HttpClientUser</param>
        /// <param name="userIdentityCookie">user Cookie on server</param>
        private void CreateOrReuseHttpClient(HttpClientUser client, Cookie userIdentityCookie)
        {
            if (client.HttpClient != null && client.UserIdentityCookie != userIdentityCookie.Value)
            {
                client.UserIdentityCookie = null;
                client.HttpClient.Dispose();
                client.HttpClient = null;
            }

            if (client.HttpClient == null) // Get session cookie for call web api service
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                cookieContainer.Add(_baseAdress, new Cookie(userIdentityCookie.Name, userIdentityCookie.Value));
                client.UserIdentityCookie = userIdentityCookie.Value;
                client.HttpClient = new HttpClient(handler) { BaseAddress = _baseAdress };
            }
        }

        /// <summary>
        /// Http Get Request for one user
        /// </summary>
        /// <typeparam name="T">Data type of result</typeparam>
        /// <param name="userId">user id</param>
        /// <param name="userIdentityCookie">identify user on server</param>
        /// <param name="relativeUrl">relative http url</param>
        /// <param name="datas">datas push on httpget</param>
        /// <returns>http result</returns>
        public async Task<T> GetAsync<T>(string userId, Cookie userIdentityCookie, string relativeUrl, Dictionary<string, string> datas = null)
        {
            T result = default(T);

            HttpClientUser httpClientUser;
            lock (_clients)
            {
                httpClientUser  = CreateOrReuseClient(userId);
            }
            if (httpClientUser == null)
                return result;

            try
            {
                lock (httpClientUser)
                {
                    httpClientUser.LockCount++;
                    CreateOrReuseHttpClient(httpClientUser, userIdentityCookie);
                }

                HttpResponseMessage httpResponse;
                if (datas != null && datas.Count > 0)
                {
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.AddRange(datas);
                    HttpContent content = new FormUrlEncodedContent(postData);
                    string urlDatas = await content.ReadAsStringAsync();
                    httpResponse = await httpClientUser.HttpClient.GetAsync(string.Format("{0}?{1}", relativeUrl, urlDatas));
                }
                else
                    httpResponse = await httpClientUser.HttpClient.GetAsync(relativeUrl);

                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(jsonStringResult))
                        {
                            result = JsonConvert.DeserializeObject<T>(jsonStringResult);
                        }
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        result = default(T);
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new HttpException((int)httpResponse.StatusCode, "Ressource not found");
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new HttpException((int)httpResponse.StatusCode, "Ressource forbidden");
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
                        WebApiException webApiException = ConvertJsonExceptionToWebApiException(jsonStringResult);
                        if (webApiException != null)
                            throw webApiException;
                        throw new HttpException("Bad HTTP request");
                    }
                    else
                    {
                        throw new HttpException((int)httpResponse.StatusCode, "error");
                    }
                }
            }
            catch (TaskCanceledException timeoutException)
            {
                throw new HttpException("Timeout", timeoutException);
            }
            catch (Exception exception)
            {
                if (exception is HttpException || exception is WebApiException)
                    throw exception;
                throw new HttpException("Can't connect to server", exception);
            }
            finally
            {
                if (httpClientUser != null)
                {
                    lock(httpClientUser)
                    {
                        if(httpClientUser.LockCount > 0)
                            httpClientUser.LockCount--;
                    }
                }
            }

            return result;
        }
        
        public async Task<TResultData> PostAsync<TData, TResultData>(string userId, Cookie userIdentityCookie, string relativeUrl, TData postData, bool isAnonymousRequest = false)
        {
            TResultData result = default(TResultData);
            HttpClientUser httpClientUser;
            lock (_clients)
            {
                httpClientUser = CreateOrReuseClient(userId);
            }
            if (httpClientUser == null)
                return result;

            try
            {
                lock (httpClientUser)
                {
                    httpClientUser.LockCount++;
                    CreateOrReuseHttpClient(httpClientUser, userIdentityCookie);
                }
                
                string postBody = JsonConvert.SerializeObject(postData, new JsonSerializerSettings { Formatting = Formatting.None });
                var httpResponse = await httpClientUser.HttpClient.PostAsync(relativeUrl, new StringContent(postBody, Encoding.UTF8, "application/json"));

                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(jsonStringResult))
                        {
                            result = JsonConvert.DeserializeObject<TResultData>(jsonStringResult);
                        }
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        result = default(TResultData);
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new HttpException((int)httpResponse.StatusCode, "Ressource not found");
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new HttpException((int)httpResponse.StatusCode, "Ressource forbidden");
                    }
                    else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var jsonStringResult = httpResponse.Content.ReadAsStringAsync().Result;
                        WebApiException webApiException = ConvertJsonExceptionToWebApiException(jsonStringResult);
                        if (webApiException != null)
                            throw webApiException;
                        throw new HttpException((int)httpResponse.StatusCode, "Bad request");
                    }
                    else
                    {
                        throw new HttpException((int)httpResponse.StatusCode, "error");
                    }
                }
            }
            catch (TaskCanceledException timeoutException)
            {
                throw new HttpException("Timeout", timeoutException);
            }
            catch (Exception exception)
            {
                if (exception is HttpException || exception is WebApiException)
                    throw exception;
                throw new HttpException("Can't connect to server", exception);
            }
            finally
            {
                if (httpClientUser != null)
                {
                    lock (httpClientUser)
                    {
                        if (httpClientUser.LockCount > 0)
                            httpClientUser.LockCount--;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Convert json exception to .Net exception 
        /// </summary>
        /// <param name="jsonException">json exception string</param>
        /// <returns></returns>
        private WebApiException ConvertJsonExceptionToWebApiException(string jsonException)
        {
            WebApiException result = null;
            try
            {
                if (!string.IsNullOrEmpty(jsonException))
                {
                    result = JsonConvert.DeserializeObject<WebApiException>(jsonException);
                    if (result != null)
                    { // BUG with my exception, json parse bad field Message and InnerException
                        result = new WebApiException(result.Code, result.Message, result.InnerException);
                    }
                }
            }
            catch (Exception except)
            {
                _logger.LogError("Unable to convert http webapi error", except);
            }
            return result;
        }
    }
}
