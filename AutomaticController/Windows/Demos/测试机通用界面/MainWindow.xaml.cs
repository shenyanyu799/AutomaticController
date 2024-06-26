#define CCDEnable
using AutomaticController.Function;
using AutomaticController.UI;
using AutomaticController.Windows.Demos.测试机通用界面.Datas;
using AutomaticController.Windows.Demos.测试机通用界面.Pages;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
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
using VM.Core;

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
            //处理窗口中被点击的Uri
            UserFrame.Navigating += (s, e) =>
            {
                if(e.Uri != null)
                {
                    if (e.Uri.IsAbsoluteUri && e.Uri.IsFile)
                    {
                        e.Cancel = true;
                        if (File.Exists(e.Uri.LocalPath) == false)
                        {
                            MessageBox.Show("文件不存在");
                            return;
                        }
                        try
                        {
                            Process.Start(new ProcessStartInfo(e.Uri.LocalPath));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }               
            };
            //WindowStyle = WindowStyle.None;
            //WindowState = WindowState.Maximized;
            //窗口初始状态
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

            //用户参数初始化
            if (Parameters_XMLFile.SelectItem == null)
            {
                Parameters_XMLFile.Select("Default");
            }

            Pages = new PageManage(UserFrame);
            //添加页面
            Pages.SetKeyPage(new 口令验证());
#if CCDEnable
            Pages.Add("CCD编辑", new CCD编辑());
            Pages.Add("运行监控", new 运行监控_CCD());
            this.Closed += (s, e) =>
            {
                CCD编辑.CloseCCD();
            };
            Parameters_XMLFile.Instance.LoadParamEvent += param => {
                CCD编辑.LoadCCD();
            };
            
#else
            Pages.Add("运行监控", new 运行监控());
#endif
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
                Process.GetCurrentProcess().WaitForExit(3000);//防止应用卡死
            };
            PLCStart();
            BarcodeOpen();
            bool 数据记录 = false;
            PLC1.数据记录.ReadFinishEvent += u =>
            {
                if (PLC1.PLC.Connected == false) return;
                PLC1.检测结果.RequestRead = true;
                PLC1.检测重量.RequestRead = true;
                bool r = PLC1.数据记录.Value;
               
                if (r && 数据记录 == false)
                {
                    string sn = SNCode;
                    SNCode = "";
                    PLC1.扫码完成.Value = false;
                    Task.Delay(500).ContinueWith(t =>
                    {
                        int n = (int)PLC1.检测结果.Value;
                        string rs = "";
                        if (n == 1) rs = "OK";
                        if (n == 2) rs = "扫码超时";
                        if (n == 3) rs = "重量NG";
                        if (n == 4) rs = "拍照NG";
                        测试机通用界面.Pages.UserData.Add(new string[] { DateTime.Now.ToString(), sn, PLC1.检测重量.Value.ToString(), rs, CCD编辑.OutImgPath });
                        CCD编辑.OutImgPath = "";
                        PLC1.数据记录.Value = false;
                    });
                }
                  
                数据记录 = r;
            };
            //扫码完成
            SNCodeReadEvent += t =>
            {
                var par = Parameters_XMLFile.SelectItem;
                bool ok = true;
                if (par.SN码前缀?.Length > 0)
                {
                    if (t.StartsWith(par.SN码前缀) == false)
                    {
                        ok = false;
                        PLC1.检测结果.Value = 5;

                    }
                }
                
                if (par.重码检测)
                {
                    using (LiteDatabase lite = new LiteDatabase(测试机通用界面.Pages.UserData.DBPath))
                    {
                        var lst = lite.GetCollection<测试机通用界面.Pages.UserData>(测试机通用界面.Pages.UserData.TableName);
                        //SN搜索
                        var list = lst.Find(x => x.SN码 == t);
                        if (list.Count() > 0)
                        {
                            ok = false;
                            PLC1.检测结果.Value = 6;
                        }
                    }
                }
                PLC1.扫码完成.Value = ok;
            };
            bool startCCD = false;
            //开始拍照
            PLC1.产品拍照.ReadFinishEvent += t =>
            {
                bool r = PLC1.产品拍照.Value;

                if (r == true && startCCD == false) 
                {
                    Task.Run(CCD编辑.ExecuteCCD);
                }
                if(r == false)
                {
                    PLC1.拍照OK.Value = false;
                    PLC1.拍照NG.Value = false;
                }
                startCCD = r;
            };
            //拍照完成
            CCD编辑.CCDExecutedEvent += () =>
            {
                if(CCD编辑.CCDResult == 1)
                {
                    PLC1.拍照OK.Value = true;
                }
                if (CCD编辑.CCDResult == 2)
                {
                    PLC1.拍照NG.Value = true;
                }
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

        #region 扫码枪通讯
        public static string SNCode { get; set; }

        /// <summary>
        /// SN码读取完成
        /// </summary>
        public static event Action<string> SNCodeReadEvent;
        bool IsBarcodeOpen;
        System.IO.Ports.SerialPort Barcode = new System.IO.Ports.SerialPort();

        public void BarcodeOpen()
        {
            if (IsBarcodeOpen) return;
            IsBarcodeOpen = true;
            this.Closed += (s, e) => {
                BarcodeClose();
            };
            Task.Run(async () =>
            {
                while (IsBarcodeOpen)
                {
                    try
                    {
                        if (Barcode.IsOpen == false)
                        {
                            Barcode = new System.IO.Ports.SerialPort();
                            Barcode.PortName = Setting.Instance.Sweep1_Name;
                            Barcode.Parity = Setting.Instance.Sweep1_Parity;
                            Barcode.BaudRate = Setting.Instance.Sweep1_Baud;
                            Barcode.StopBits = Setting.Instance.Sweep1_Stopbit;
                            Barcode.DataBits = Setting.Instance.Sweep1_Databit;
                            Barcode.ReadTimeout = 100;
                            Barcode.Open();
                            App.Current.Dispatcher.InvokeAsync(() =>
                            {
                                sweep1State.Text = "已连接";
                                sweep1State.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x08, 0xFF, 0x00));

                            });
                        }
                        else
                        {
                            if (Barcode.BytesToRead > 0)
                            {
                                await Task.Delay(50);
                                SNCode = Barcode.ReadExisting().Trim();
                                App.Current.Dispatcher.Invoke(() => { SNCodeReadEvent?.Invoke(SNCode); });
                                
                            }
                        }

                    }
                    catch
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            sweep1State.Text = "未连接";
                            sweep1State.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));

                        });
                        Barcode?.Close();
                        await Task.Delay(500);
                    }
                    await Task.Delay(100);
                }
            });
        }
        public void BarcodeClose()
        {
            IsBarcodeOpen = false;
            try
            {
                Barcode.Close();
            }
            catch { }
        }
        #endregion

        bool plcConnected;
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
                //首次连接后自动载入一次参数
                if (plcConnected == false)
                {
                    Task.Delay(1000).ContinueWith(t=> Parameters_XMLFile.Instance.LoadParam());
                }
                plcState.Text = $"PLC:{PLC1.PLC.CommunicationDelay.ToString("F2")}ms";
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
            if (Pages.IsUnlock(Setting.Instance.UserKey))
            {
                keyState.SetValue(Border.BackgroundProperty, this.Resources["unlockImg"]);
                keyState.ToolTip = $"将在{((int)Pages.RemainingLockTime)}秒后自动登出";
            }
            else
            {
                keyState.SetValue(Border.BackgroundProperty, this.Resources["lockImg"]);
                keyState.ToolTip = $"口令未登录";
            }
            plcConnected = PLC1.PLC.Connected;
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
