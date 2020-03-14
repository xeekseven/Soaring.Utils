using Soaring.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Soaring.HttpCS.Util
{
    public class HttpServerUtil : SingletonObject<HttpServerUtil>
    {
        private HttpListener _listener = new HttpListener();
        private Dictionary<string, Func<HttpListenerRequest, ResponseResult>> _getRouteHandlerDict = null;
        private Dictionary<string, Func<HttpListenerRequest, ResponseResult>> _postRouteHandlerDict = null;

        public async Task StartHttpServer(string[] hostUrls, Func<string, NameValueCollection, ResponseResult> defaultGetHandler = null, Func<string, string, ResponseResult> defaultPostHandler = null)
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (hostUrls == null || hostUrls.Length == 0)
                throw new ArgumentException("hostUrls");
            foreach (string s in hostUrls)
            {
                this._listener.Prefixes.Add(s);
            }
            this._listener.Start();
            Console.WriteLine("...listening");
            while (true)
            {
                try
                {
                    HttpListenerContext context = await this._listener.GetContextAsync();
                    HttpListenerRequest request = context.Request;
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;

                    ResponseResult responseResult = null;
                    var requestRoute = request.Url.AbsolutePath;
                    switch (request.HttpMethod.ToUpper())
                    {
                        case "GET":
                            if (this._getRouteHandlerDict != null && this._getRouteHandlerDict.ContainsKey(requestRoute))
                            {
                                responseResult = this._getRouteHandlerDict[requestRoute](request);
                            }
                            else if (defaultGetHandler != null)
                            {
                                responseResult = HttpGetRequest(request, defaultGetHandler);
                            }
                            else
                            {
                                response.StatusCode = 404;
                                responseResult = new ResponseResult() { RESCode = 404, ResponseString = "Can not Found This Get Method" };
                            }
                            break;
                        case "POST":
                            if (this._postRouteHandlerDict != null && this._postRouteHandlerDict.ContainsKey(requestRoute))
                            {
                                responseResult = this._postRouteHandlerDict[requestRoute](request);
                            }
                            else if (defaultGetHandler != null)
                            {
                                responseResult = HttpPostRequest(request, defaultPostHandler);
                            }
                            else
                            {
                                response.StatusCode = 404;
                                responseResult = new ResponseResult() { RESCode = 404, ResponseString = "Can not Found This Post Method" };
                            }

                            break;
                        default:
                            responseResult = new ResponseResult() { ResponseString = $"Can not handle HttpMethod{request.HttpMethod}" };
                            break;
                    }
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(responseResult));
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Flush();
                    output.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                
            }
        }

        public void AddGetRouteHandlers(Dictionary<string, Func<HttpListenerRequest, ResponseResult>> routeHandlerDict)
        {
            foreach(var item in routeHandlerDict)
            {
                string routePath = item.Key;
                var handler = item.Value;
                if (this._getRouteHandlerDict == null) this._getRouteHandlerDict = new Dictionary<string, Func<HttpListenerRequest, ResponseResult>>();
                if (!routePath.StartsWith("/")) routePath = "/" + routePath;
                this._getRouteHandlerDict[routePath] = handler;
            }
            
        }
        public void AddPostRouteHandlers(Dictionary<string, Func<HttpListenerRequest, ResponseResult>> routeHandlerDict)
        {
            foreach (var item in routeHandlerDict)
            {
                string routePath = item.Key;
                var handler = item.Value;
                if (this._postRouteHandlerDict == null) this._postRouteHandlerDict = new Dictionary<string, Func<HttpListenerRequest, ResponseResult>>();
                if (!routePath.StartsWith("/")) routePath = "/" + routePath;
                this._postRouteHandlerDict[routePath] = handler;
            }    
        }

        public void AddListenUrl(string hostUrl)
        {
            if (!this._listener.Prefixes.Contains(hostUrl))
            {
                this._listener.Prefixes.Add(hostUrl);
            }
        }
        private ResponseResult HttpGetRequest(HttpListenerRequest request, Func<string, NameValueCollection, ResponseResult> requestHandler)
        {
            // Get a Get data
            ResponseResult responseResult = requestHandler(request.Url.AbsoluteUri, request.QueryString);
            return responseResult;
        }
        private ResponseResult HttpPostRequest(HttpListenerRequest request, Func<string, string, ResponseResult> requestHandler)
        {
            // Get a request data
            var reader = new System.IO.StreamReader(request.InputStream, Encoding.UTF8);
            string postData = reader.ReadToEnd();
            ResponseResult responseResult = requestHandler(request.Url.AbsoluteUri, postData);
            return responseResult;
        }
    }
}
