using System;
using System.Diagnostics;
using System.Threading;
using Soaring.Utils;

namespace Soaring.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = ProcessUtil.Instance.RunShell("/usr/local/share/dotnet/dotnet", "/Users/xeek/Documents/codes/tmp/bin/Debug/netcoreapp3.1/tmp.dll");
            
            //ProcessUtil.Instance.Close(p);
            Console.WriteLine("Hello World!");
            var currentProcess = Process.GetCurrentProcess();
            currentProcess.Exited += (s, e) =>
            {
                Console.WriteLine("exit");
            };
            currentProcess.Disposed += (s, e) =>
            {
                Console.WriteLine("dispose");
            };
            currentProcess.EnableRaisingEvents = true;
            Console.WriteLine("Hello World!");
            Thread.Sleep(100000);
        }
    }
}
