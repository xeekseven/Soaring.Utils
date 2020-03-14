using Newtonsoft.Json;
using Soaring.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Soaring.HttpCS.Util
{
    public class HttpClientUtil : SingletonObject<HttpClientUtil>
    {
        private string _defaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11";
        private string _defaultAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
        private HttpClient _httpClient = new HttpClient();
        public async Task<TResult> HttpGetAsync<TResult>(string url, int timeout = 10, string accessToken = null, string proxyUrl = null, string userAgent = null, Dictionary<string, string> headers = null)
        {
            this.BuildHttpClientInstance(timeout, accessToken, proxyUrl, userAgent, headers);
            var response = await this._httpClient.GetAsync(url);
            var responseResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseResult);
        }

        public async Task<TResult> HttpPostFormDataAsync<TResult>(string url, Dictionary<string, string> paramDict, int timeout = 10, string accessToken = null, string proxyUrl = null, string userAgent = null, Dictionary<string, string> headers = null)
        {
            this.BuildHttpClientInstance(timeout, accessToken, proxyUrl, userAgent, headers);
            var response = await this._httpClient.PostAsync(url, new FormUrlEncodedContent(paramDict));
            var responseResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseResult);
        }
        public async Task<TResult> HttpPostJsonAsync<TResult>(string url, Dictionary<string, string> paramDict, int timeout = 10, string accessToken = null, string proxyUrl = null, string userAgent = null, Dictionary<string, string> headers = null)
        {
            this.BuildHttpClientInstance(timeout, accessToken, proxyUrl, userAgent, headers);
            var response = await this._httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(paramDict)));
            var responseResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(responseResult);
        }
        private void BuildHttpClientInstance(int timeout = 10, string accessToken = null, string proxyUrl = null, string userAgent = null, Dictionary<string, string> headers = null)
        {
            if (proxyUrl != null)
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    Proxy = new WebProxy("proxyUrl"),
                    UseProxy = true,
                };
                this._httpClient = new HttpClient(handler);
            }

            this._httpClient.DefaultRequestHeaders.Clear();

            // 默认配置
            this._httpClient.DefaultRequestHeaders.Add("Accept", this._defaultAccept);
            this._httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            this._httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");

            // 设置http头
            if (headers != null)
            {
                foreach (var headerItem in headers)
                {
                    this._httpClient.DefaultRequestHeaders.Add(headerItem.Key, headerItem.Value);
                }
            }
            this._httpClient.Timeout = TimeSpan.FromSeconds(timeout);

            // 设置token
            if (accessToken != null)
            {
                var authenticationHeaderValue = new AuthenticationHeaderValue("bearer", accessToken);
                this._httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            }

            // 设置 userAgent
            this._httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent ?? _defaultUserAgent);
        }
    }
}
