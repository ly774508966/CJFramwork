using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Threading;
using System.IO;

public class ysBatchMode
{
    public class CMD
    {
        public static List<string> commandList = new List<string>();
        public static Stack<System.Diagnostics.Process> cmdStack = new Stack<System.Diagnostics.Process>();
        [MenuItem("Test/test cmd")]
        public static void testaaa()
        {
            CMD.Start();
            CMD.AddCommand(@"ipconfig/all");
            CMD.Close();
        }
        public static void Start()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = false;//不显示程序窗口
            cmdStack.Push(p);
            System.Console.InputEncoding = System.Text.Encoding.UTF8;
            p.Start();//启动程序
            p.StandardInput.WriteLine("@echo off");
            p.StandardInput.WriteLine("cls");
            p.StandardInput.AutoFlush = true;
        }
        public static void AddCommand(string commandStr)
        {
            //向cmd窗口发送输入信息
            System.Diagnostics.Process p = cmdStack.Peek();
            p.StandardInput.WriteLine(commandStr);
            p.StandardInput.AutoFlush = true;
            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();
            Debug.Log(output);
            p.WaitForExit();//等待程序执行完退出进程
        }
        public static void Close()
        {
            System.Diagnostics.Process p = cmdStack.Pop();
            p.Close();
        }
    }
}
