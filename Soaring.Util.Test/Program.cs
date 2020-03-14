using Soaring.HttpCS.Util;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Soaring.Util.Test
{
    class Program
    {
        static Socket ClientSocket;
        static void Main(string[] args)
        {
            var listenUrls = new string[1] { "http://*:5000/" };
            var task = new Task(async () =>
            {
                var getHandlerDict = new Dictionary<string, Func<HttpListenerRequest, ResponseResult>>();
                getHandlerDict.Add("api/getuser",(r) =>
                {
                    return new ResponseResult() { ResponseString = "get user 11" };
                });
                    
                HttpServerUtil.Instance.AddGetRouteHandlers(getHandlerDict);
                await HttpServerUtil.Instance.StartHttpServer(listenUrls);
                // default get post处理方式
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
            if(Console.ReadKey().Key == ConsoleKey.A)
            {
                HttpServerUtil.Instance.AddListenUrl("http://*:5001/");
            }
            Console.ReadKey();
        }

        static void Main3(string[] args)
        {
            string IP = "127.0.0.1";
            int port = 6700;

            IPAddress ip = IPAddress.Parse(IP);  //将IP地址字符串转换成IPAddress实例
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//使用指定的地址簇协议、套接字类型和通信协议
            IPEndPoint endPoint = new IPEndPoint(ip, port); // 用指定的ip和端口号初始化IPEndPoint实例
            ClientSocket.Connect(endPoint);  //与远程主机建立连接


            Console.WriteLine("开始发送消息");
            byte[] message = Encoding.ASCII.GetBytes("Connect the Server");  //通信时实际发送的是字节数组，所以要将发送消息转换字节
            ClientSocket.Send(message);
            Console.WriteLine("发送消息为:" + Encoding.UTF8.GetString(message));
            byte[] receive = new byte[1024];
            int length = ClientSocket.Receive(receive);  // length 接收字节数组长度
            Console.WriteLine("接收消息为：" + Encoding.UTF8.GetString(receive));
            ClientSocket.Close();  //关闭连接
            Console.ReadLine();
        }
        static void Main2()
        {
            Console.WriteLine("Press any key to start the server");
            Console.ReadKey();
            var appServer = new AppServer() { };
            if (!appServer.Setup(2012))
            {
                Console.WriteLine("Failed to setup");
                Console.ReadKey();
                return;
            }
            Console.WriteLine();

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

            appServer.NewSessionConnected += new SessionHandler<AppSession>(appServer_NewSessionConnected);
            //appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(appServer_NewRequestReceived);
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }
            //Stop the appServer
            appServer.Stop();

            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }

        private static void appServer_NewSessionConnected(AppSession session)
        {
            session.Send("Welcome to SuperSocket Telnet Server");
        }

        private static void appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            switch (requestInfo.Key.ToUpper())
            {
                case "ECHO":
                    session.Send(requestInfo.Body);
                    break;
                case "ADD":
                    session.Send(requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
                    break;
                case "MULT":
                    var result = 1;
                    foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
                    {
                        result *= factor;
                    }
                    session.Send(result.ToString());
                    break;
                default:
                    break;
            }
        }
    }
}
