using AutomaticController.Function;
using AutomaticController.UI;
using AutomaticController.Windows.Demos.测试机通用界面.Datas;
using AutomaticController.Windows.Demos.测试机通用界面.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;

namespace AutomaticController.Windows.Demos.测试机通用界面
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Grid MainGrid { get; set; }
        public static PageManage Pages { get; set; }
        public bool Started;
        public MainWindow()
        {
            InitializeComponent();
            MainGrid = this.mainGrid;
            //WindowStyle = WindowStyle.None;
            //WindowState = WindowState.Maximized;
            switch (Setting.Instance.WindowStartupState)
            {
                case WindowStartupState.None:
                    break;
                case WindowStartupState.Full:
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    break;
                case WindowStartupState.Max:
                    WindowState = WindowState.Maximized;
                    break;
                default:
                    break;
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            

            Pages = new PageManage(UserFrame);
            //添加页面
            Pages.SetKeyPage(new 口令验证());
            Pages.Add("运行监控", new 运行监控());
            Pages.Add("IO监控", new IO监控());
            Pages.Add("参数设置", new 参数设置());
            Pages.Add("封面", new 封面());
            Pages.Add("手动界面", new 手动界面());
            Pages.Add("数据查询", new 数据查询());
            Pages.Add("系统设置", new 系统设置());
            Task.Delay(3000).ContinueWith(t => App.Current.Dispatcher.Invoke(() =>
            {
                if (Pages.IsNext == false)
                    Pages.Next("运行监控");
            }));
            Started = true;
            this.Closed += (s, e) =>
            {
                Started = false;
            };
            PLCStart();
            bool 记录1 = false;
            bool 记录2 = false;
            bool 记录3 = false;
            PLC1.记录1.ReadFinishEvent += u => {
                bool b = PLC1.记录1.Value;
                if (b && 记录1 == false)
                {
                    Task.Delay(500).ContinueWith(t => {
                        string rt = "";
                        if (PLC1.状态1.Value == 1) rt = "OK";
                        if (PLC1.状态1.Value == 2) rt = "NG";
                        UserData.Add(new string[] { DateTime.Now.ToString(), "工位1", PLC1.结果流量1.Value.ToString(), rt });
                        PLC1.记录1.Value = false;
                    });
                }
                记录1 = b;
            };
            PLC1.记录2.ReadFinishEvent += u => {
                bool b = PLC1.记录2.Value;
                if (b && 记录2 == false)
                {
                    Task.Delay(500).ContinueWith(t => {
                        string rt = "";
                        if (PLC1.状态2.Value == 1) rt = "OK";
                        if (PLC1.状态2.Value == 2) rt = "NG";
                        UserData.Add(new string[] { DateTime.Now.ToString(), "工位2", PLC1.结果流量2.Value.ToString(), rt }); 
                        PLC1.记录2.Value = false;
                    });
                }
                记录2 = b;
            };
            PLC1.记录3.ReadFinishEvent += u => {
                bool b = PLC1.记录3.Value;
                if (b && 记录3 == false)
                {
                    Task.Delay(500).ContinueWith(t => {
                        string rt = "";
                        if (PLC1.状态3.Value == 1) rt = "OK";
                        if (PLC1.状态3.Value == 2) rt = "NG";
                        UserData.Add(new string[] { DateTime.Now.ToString(), "工位3", PLC1.结果流量3.Value.ToString(), rt });
                        PLC1.记录3.Value = false;
                    });
                }
                记录3 = b;
            };
        }

        public async void PLCStart()
        {
           await Task.Run(async () => {
                while (Started)
                {
                   try
                   {
                       if (PLC1.PLC.Started == false)
                       {
                           PLC1.PLC.Serial = new System.IO.Ports.SerialPort();
                           PLC1.PLC.Serial.PortName = Setting.Instance.PLC1_Name;
                           PLC1.PLC.Serial.BaudRate = Setting.Instance.PLC1_Baud;
                           PLC1.PLC.Serial.Parity = Setting.Instance.PLC1_Parity;
                           PLC1.PLC.Serial.DataBits = Setting.Instance.PLC1_Databit;
                           PLC1.PLC.Serial.StopBits = Setting.Instance.PLC1_Stopbit;
                           PLC1.PLC.Serial.WriteTimeout = 1000;
                           PLC1.PLC.Serial.ReadTimeout = 1000;
                           PLC1.PLC.Start();
                           //等待通讯连接
                           for (int i = 0; i < 100; i++)
                           {
                               if (PLC1.PLC.Connected == true) break;
                               await Task.Delay(100);
                           }
                       }
                       else
                       {
                           //PLC连接正常，PLC参数未曾改变
                           if(PLC1.PLC.Connected == false || PLC1.PLC.Serial.PortName != Setting.Instance.PLC1_Name || PLC1.PLC.Serial.BaudRate != Setting.Instance.PLC1_Baud ||
                           PLC1.PLC.Serial.Parity != Setting.Instance.PLC1_Parity || PLC1.PLC.Serial.DataBits != Setting.Instance.PLC1_Databit || PLC1.PLC.Serial.StopBits != Setting.Instance.PLC1_Stopbit)
                           {
                               PLC1.PLC.Close();
                               await Task.Delay(500);
                           }
                       }
                   }
                   catch
                   {
                       PLC1.PLC.Close();
                       await Task.Delay(500);
                   }
                   await Task.Delay(100);
               }
               await Task.Delay(100);
           });
        }




        /// <summary>
        /// 实时渲染事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //DatetimeText.Text = DateTime.Now.ToString();
            //实时显示PLC通讯状态
            if (PLC1.PLC.Connected)
            {
                plcState.Text = $"PLC:{PLC1.PLC.CommunicationDelay.ToString("0.00")}ms";
                plcState.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                if (PLC1.PLC.Started)
                {
                    plcState.Text = "PLC连接中";
                    plcState.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    plcState.Text = "PLC关闭";
                    plcState.Foreground = new SolidColorBrush(Colors.Red);
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //切换窗口
            Button button = (Button)sender;
            string name = button.Content.ToString();
            switch (name)
            {
                case "退出系统":
                    App.Current.Shutdown();
                    break;
                case "参数设置":
                case "系统设置":
                    Pages.NextKey(Setting.Instance.UserKey, name);
                    break;
                default:
                    Pages.Next(name);
                    break;
            }
        }


        /// <summary>
        /// 当该窗口内的输入框被按下时，如果是在触摸界面按下，则弹出键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            bool open = false;
            if (e.OriginalSource is TextBox || (e.OriginalSource is PasswordBox))
            {
                open = (e.OriginalSource as Control).TouchesOver.Count() > 0;
            }
            if (open)
            {
                string p = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";
                if (File.Exists(p))
                    Process.Start(p);
            }
        }
    }
}
