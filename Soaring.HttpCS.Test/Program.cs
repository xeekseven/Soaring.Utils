using Soaring.HttpCS.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Soaring.HttpCS.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var listenUrls = new string[1] { "http://*:5000/" };
            var task = new Task(async () =>
            {
                var getRouteHandlers = new Dictionary<string, Func<HttpListenerRequest, ResponseResult>>();
                getRouteHandlers.Add("api/getuser", (r) =>
                {
                    return new ResponseResult() { ResponseString = "get user 11" };
                });
                await HttpServerUtil.Instance.StartHttpServer(listenUrls);
                //(url, queryStrings) =>
                //{
                //    var r = new ResponseResult();
                //    r.ResponseString = $"Get url:{url}";
                //    foreach (var key in queryStrings.AllKeys)
                //    {
                //        r.ResponseString += $" {key}:{queryStrings[key]} ";
                //    }
                //    return r;
                //},
                //(url, data) =>
                //{
                //    var r = new ResponseResult();
                //    r.ResponseString = $"GET POST DATA {data}";
                //    return r;
                //});

            });
            task.Start();
            task.Wait();
            if (Console.ReadKey().Key == ConsoleKey.A)
            {
                HttpServerUtil.Instance.AddListenUrl("http://*:5001/");
            }
            Console.ReadKey();
        }
    }
}
