using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutomaticController.Function;
using AutomaticController.Windows;
using AutomaticController.Windows.Demos.PLC通讯样例;
using AutomaticController.Windows.Demos.测试机通用界面;

namespace AutomaticController
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private bool started;
        /// <summary>
        /// 应用启动，在应用启动时最先执行的是这个程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            //打开应用禁止多开
            Process process = Process.GetCurrentProcess();
            string name = process.ProcessName;
            var ps = Process.GetProcesses();
            foreach (var item in ps)//遍历所有进程，查找重名
            {
                if(item.ProcessName == name)
                {
                    if(item.Id != process.Id)
                    {
                        MessageBox.Show("程序已启动");
                        process.Kill();
                        return;
                    }
                }
            }

            //未处理的异常
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;


            //启动初始窗口...........................................
            //this.MainWindow = new Windows.Demos.测试机通用界面.MainWindow();//创建一个新窗体
            this.MainWindow = new Windows.Demos.吸尘器空气性能测试.StartWindow();//创建一个新窗体
            //this.MainWindow = new Windows.SanSheng.数据库扫码查重项目.MainWindow1();//创建一个新窗体


            LogManager.Log("启动窗口：" + this.MainWindow.Title, LogLevel.Message);
            this.MainWindow.Show();//显示窗体






            //创建新的线程，开始循环
            new Thread(() =>
            {
                started = true;
                DateTime dateTime = DateTime.Now;
                while (started)
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        double dt = (now - dateTime).TotalMilliseconds;
                        dateTime = now;
                        RunTime(dt);
                        Thread.Sleep(1);
                        //Task.Delay(1).Wait();
                    }
                    catch (Exception ex)
                    {
                        LogManager.LogShow(ex.Message, LogLevel.Error);
                    }
                }
                System.Environment.Exit(0);//结束生命周期后自动退出系统
            }).Start();

        }

        /// <summary>
        /// 处理未处理的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogManager.LogShow(e.Exception.Message, LogLevel.Error);
        }

        /// <summary>
        /// 生命周期
        /// </summary>
        public void RunTime(double dt)
        {
            //Console.WriteLine(dt);
        }
        /// <summary>
        /// 退出应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            started = false;//结束应用生命周期
        }
    }
}
