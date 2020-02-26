using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Soaring.Utils
{
    // 进程管理类，主要用于启动子进程，kill -9 pid 可能会产生孤儿进程
    public class ProcessUtil : IDisposable
    {
        private static ProcessUtil _instacne = null;
        private List<Process> processList = new List<Process>();
        public static ProcessUtil Instance
        {
            get
            {
                if (_instacne == null)
                {
                    _instacne = new ProcessUtil();
                }
                return _instacne;
            }
        }
        private ProcessUtil()
        {
            Console.CancelKeyPress += (s, e) =>
            {
                ProcessUtil.Instance.Dispose();
            };
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                ProcessUtil.Instance.Dispose();
            };
        }
        
        public Process RunShell(string command,
                                string args,
                                string workDir = null,
                                DataReceivedEventHandler outputHandler = null,
                                DataReceivedEventHandler errorHandler = null,
                                EventHandler exitHandler = null)
        {
            ProcessStartInfo stdInfo = new ProcessStartInfo(command, args) { RedirectStandardOutput = false };
            // 是否使用操作系统shell启动程序
            stdInfo.UseShellExecute = true;
            if (outputHandler != null)
            {
                stdInfo.RedirectStandardOutput = true;
            }
            // 设置工作目录
            if (workDir != null)
            {
                stdInfo.WorkingDirectory = workDir;
            }

            // unix 平台下没什么用
            stdInfo.CreateNoWindow = true;
            var process = Process.Start(stdInfo);
            process.EnableRaisingEvents = true;
            if (outputHandler != null)
            {
                process.BeginOutputReadLine();
                process.OutputDataReceived += outputHandler;
            }
            if (errorHandler != null)
            {
                process.BeginErrorReadLine();
                process.ErrorDataReceived += errorHandler;
            }
            if (exitHandler != null) process.Exited += exitHandler;
            processList.Add(process);
            return process;
            //proc.WaitForExit();
        }

        public void Close(Process process)
        {
            process.Kill();
            // Close process by sending a close message to its main window.
            process.CloseMainWindow();
            // Free resources associated with process.
            process.Close();
        }

        public void Dispose()
        {
            this.processList.ForEach(p =>
            {
                if (p != null)
                {
                    try
                    {
                        p.Kill();
                        p.CloseMainWindow();
                        p.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }
    }
}