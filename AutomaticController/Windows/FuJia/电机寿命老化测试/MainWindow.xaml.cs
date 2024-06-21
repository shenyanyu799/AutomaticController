using AutomaticController.Device;
using AutomaticController.Windows.FuJia.电机寿命老化测试.Datas;
using AutomaticController.Windows.FuJia.电机寿命老化测试.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Grid MainGrid { get; private set; }
        public static MainWindow Window { get; private set; }
        public static PageManage Pages { get; private set; }
        public bool Started;
        public MainWindow()
        {
            InitializeComponent();
            Window = this;
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
            DOStart();
            AIStart();
            bool 记录1 = false;
            bool 记录2 = false;
            bool 记录3 = false;
            PLC1.记录1.ReadFinishEvent += u =>
            {
                bool b = PLC1.记录1.Value;
                if (b && 记录1 == false)
                {
                    Task.Delay(500).ContinueWith(t =>
                    {
                        string rt = "";
                        if (PLC1.状态1.Value == 1) rt = "OK";
                        if (PLC1.状态1.Value == 2) rt = "NG";
                        UserData.Add(new string[] { DateTime.Now.ToString(), "工位1", PLC1.结果流量1.Value.ToString(), rt });
                        PLC1.记录1.Value = false;
                    });
                }
                记录1 = b;
            };
            PLC1.记录2.ReadFinishEvent += u =>
            {
                bool b = PLC1.记录2.Value;
                if (b && 记录2 == false)
                {
                    Task.Delay(500).ContinueWith(t =>
                    {
                        string rt = "";
                        if (PLC1.状态2.Value == 1) rt = "OK";
                        if (PLC1.状态2.Value == 2) rt = "NG";
                        UserData.Add(new string[] { DateTime.Now.ToString(), "工位2", PLC1.结果流量2.Value.ToString(), rt });
                        PLC1.记录2.Value = false;
                    });
                }
                记录2 = b;
            };
            PLC1.记录3.ReadFinishEvent += u =>
            {
                bool b = PLC1.记录3.Value;
                if (b && 记录3 == false)
                {
                    Task.Delay(500).ContinueWith(t =>
                    {
                        string rt = "";
                        if (PLC1.状态3.Value == 1) rt = "OK";
                        if (PLC1.状态3.Value == 2) rt = "NG";
                        UserData.Add(new string[] { DateTime.Now.ToString(), "工位3", PLC1.结果流量3.Value.ToString(), rt });
                        PLC1.记录3.Value = false;
                    });
                }
                记录3 = b;
            };

            //初始化参数
            RunningData.Load();
            //Parameters_XMLFile.Load();


            //开启循环
            Task.Run(() =>
            {
                DateTime date = DateTime.Now;
                while (Started)
                {
                    try
                    {
                        if ((DateTime.Now - date).TotalSeconds >= 1)
                        {
                            Loop(true);
                            date = DateTime.Now;
                        }
                        else
                        {
                            Loop(false);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    Task.Delay(10).Wait();
                }
            });

        }

        public async void DOStart()
        {
            Modbus_RTU modbus = Devices.DODevice;
            await Task.Run(async () =>
            {
                while (Started)
                {
                    try
                    {
                        if (modbus.Started == false)
                        {
                            modbus.Serial = new System.IO.Ports.SerialPort();
                            modbus.Serial.PortName = Setting.Instance.DO_Name;
                            modbus.Serial.BaudRate = Setting.Instance.DO_Baud;
                            modbus.Serial.Parity = Setting.Instance.DO_Parity;
                            modbus.Serial.DataBits = Setting.Instance.DO_Databit;
                            modbus.Serial.StopBits = Setting.Instance.DO_Stopbit;
                            //modbus.Serial.WriteTimeout = 1000;
                            modbus.Serial.ReadTimeout = 1000;
                            modbus.Start();
                            //等待通讯连接
                            for (int i = 0; i < 100; i++)
                            {
                                if (modbus.Connected == true) break;
                                await Task.Delay(100);
                            }
                        }
                        else
                        {
                            //PLC连接正常，PLC参数未曾改变
                            if (modbus.Connected == false || modbus.Serial.PortName != Setting.Instance.DO_Name || modbus.Serial.BaudRate != Setting.Instance.DO_Baud ||
                            modbus.Serial.Parity != Setting.Instance.DO_Parity || modbus.Serial.DataBits != Setting.Instance.DO_Databit || modbus.Serial.StopBits != Setting.Instance.DO_Stopbit)
                            {
                                modbus.Close();
                                await Task.Delay(500);
                            }
                        }
                    }
                    catch
                    {
                        modbus.Close();
                        await Task.Delay(500);
                    }
                    await Task.Delay(100);
                }
                await Task.Delay(100);
            });
        }
        bool AIAIDevice_Connected;
        bool AIAIDevice_Started;
        DateTime AIAIDevice_LastTime;
        double AIAIDevice_CommunicationDelay;
        public async void AIStart()
        {
            await Task.Run(async () =>
            {
                while (Started)
                {
                    try
                    {
                        if (Devices.AIDevice == null)
                        {
                            try
                            {
                                AIAIDevice_Started = false;
                                AIAIDevice_Connected = false;
                                var sp = new System.IO.Ports.SerialPort();
                                sp = new System.IO.Ports.SerialPort();
                                sp.PortName = Setting.Instance.AI_Name;
                                sp.BaudRate = Setting.Instance.AI_Baud;
                                sp.Parity = Setting.Instance.AI_Parity;
                                sp.DataBits = Setting.Instance.AI_Databit;
                                sp.StopBits = Setting.Instance.AI_Stopbit;
                                //sp.WriteTimeout = 1000;
                                sp.ReadTimeout = 500;
                                sp.Encoding = Encoding.ASCII;
                                sp.NewLine = "\r";
                                Devices.AIDevice = sp;
                                Devices.AIDevice.Open();
                                AIAIDevice_LastTime = DateTime.Now;
                                AIAIDevice_Started = true;
                            }
                            catch
                            {
                                Devices.AIDevice.Dispose();
                                Devices.AIDevice = null;
                                await Task.Delay(500);
                                continue;
                            }
                        }


                        //PLC连接正常，PLC参数未曾改变
                        if (Devices.AIDevice.PortName != Setting.Instance.AI_Name || Devices.AIDevice.BaudRate != Setting.Instance.AI_Baud ||
                        Devices.AIDevice.Parity != Setting.Instance.AI_Parity || Devices.AIDevice.DataBits != Setting.Instance.AI_Databit || Devices.AIDevice.StopBits != Setting.Instance.AI_Stopbit)
                        {
                            AIAIDevice_Connected = false;
                            AIAIDevice_Started = false;
                            Devices.AIDevice.Close();
                            Devices.AIDevice = null;
                            await Task.Delay(500);
                            continue;
                        }
                        else
                        {
                            Devices.采集电压1 = GetModuleVoltage(Devices.AIDevice, 1);
                            Devices.采集电压2 = GetModuleVoltage(Devices.AIDevice, 2);
                            Devices.采集电压3 = GetModuleVoltage(Devices.AIDevice, 3);
                            for (int i = 0; i < Devices.采集电流.Length; i++)
                            {
                                Devices.采集电流[i] = GetModuleCurrent(Devices.AIDevice, (byte)(i + 10));
                            }

                            AIAIDevice_CommunicationDelay = (DateTime.Now - AIAIDevice_LastTime).TotalMilliseconds;
                            AIAIDevice_LastTime = DateTime.Now;
                            if (AIAIDevice_CommunicationDelay < 5000)
                            {
                                AIAIDevice_Connected = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AIAIDevice_Connected = false;
                        AIAIDevice_Started = false;
                        Devices.AIDevice?.Close();
                        Devices.AIDevice = null;
                        await Task.Delay(3000);
                    }
                    await Task.Delay(100);
                }
                await Task.Delay(100);
            });
        }
        public static double GetModuleVoltage(SerialPort serialPort, byte address)
        {
            double result = 0;
            try
            {
                string wt = $"#{address.ToString("X2")}A";
                serialPort.WriteLine(wt);
                string s = serialPort.ReadLine();
                if (double.TryParse(s.Replace(">", ""), out result))
                {
                    result *= 400;
                }
            }
            catch { }

            return result;
        }
        public static double GetModuleCurrent(SerialPort serialPort, byte address)
        {
            double result = 0;
            try
            {
                string wt = $"#{address.ToString("X2")}A";
                serialPort.WriteLine(wt);
                string s = serialPort.ReadLine();

                if (double.TryParse(s.Replace(">", ""), out result))
                {
                    result *= 25;
                }
            }
            catch { }

            return result;
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
            if (Devices.DODevice.Connected)
            {
                doState.Text = $"DO模块:{Devices.DODevice.CommunicationDelay.ToString("0.00")}ms";
                doState.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                if (Devices.DODevice.Started)
                {
                    doState.Text = "DO模块连接中";
                    doState.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    doState.Text = "DO模块关闭";
                    doState.Foreground = new SolidColorBrush(Colors.Red);
                }

            }
            if (AIAIDevice_Connected)
            {
                aiState.Text = $"AI模块:{AIAIDevice_CommunicationDelay.ToString("0.00")}ms";
                aiState.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                if (AIAIDevice_Started)
                {
                    aiState.Text = "AI模块连接中";
                    aiState.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    aiState.Text = "AI模块关闭";
                    aiState.Foreground = new SolidColorBrush(Colors.Red);
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
                case "手动界面":
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






        bool work_starting = false;
        DateTime work_starting_time;
        /// <summary>
        /// 10ms循环执行,secondPulse一秒触发一次
        /// </summary>
        /// <param name="secondPulse"></param>
        private void Loop(bool secondPulse)
        {
            //延时启动功能
            if ((DateTime.Now - work_starting_time).TotalMilliseconds >= 1000)
                work_starting = false;
            //第一组
            for (int i = 0; i < 30; i++)
            {
                double current = Devices.采集电流[i];

                RunningData data = RunningData.Get(i);
                if (data.Name == null)
                {
                    //提示未设定测试规则
                    if (RunningState.States[i].State == 1)
                    {
                        RunningState.States[i].State = 4;
                    }
                    RunningState.States[i].RequestStart = false;
                    RunningState.States[i].RequestStop = false;
                    continue;
                }
                var param = Parameters_XMLFile.Instance[data.Name];
                if (RunningState.States[i].RequestStop) RunningState.States[i].State = 0;
                switch (RunningState.States[i].State)
                {
                    case 0://待机中
                        if (Pages.IsPage("手动界面") == false) Devices.继电器组[i].Value = false;
                        if (data.TestCount > 0) RunningState.States[i].State = 5;
                        if (RunningState.States[i].RequestStart) RunningState.States[i].State = 1;
                        break;
                    case 3://测试完成
                    case 5://测试暂停中
                        if (Pages.IsPage("手动界面") == false) Devices.继电器组[i].Value = false;
                        if (data.TestCount == 0) RunningState.States[i].State = 0;
                        if (RunningState.States[i].RequestStart) RunningState.States[i].State = 1;
                        break;
                    case 1://准备启动
                        RunningState.States[i].ElectriCurrent = 0;
                        RunningState.States[i].AverageCurrent = 0;
                        RunningState.States[i].MaxCurrent = 0;
                        RunningState.States[i].MinCurrent = 0;

                        if (data.TestCount >= param.TotalCount)
                        {
                            RunningState.States[i].State = 3;
                            break;
                        }
                        if (work_starting == false)
                        {
                            Devices.继电器组[i].Value = true;
                            RunningState.States[i].State = 10;

                            //if (data.Tag == 0)
                            //    RunningState.States[i].State = 10;
                            //if (data.Tag == 1)
                            //    RunningState.States[i].State = 11;
                            work_starting = true;
                            RunningState.States[i].RuningTime = DateTime.Now;
                            work_starting_time = DateTime.Now;
                            //表示新的开始
                            if (data.TestCount == 0)
                            {
                                data.StartTime = DateTime.Now;
                                RunningData.Set(i, data);
                            }

                        }
                        break;
                    case 2://电流异常
                        Devices.继电器组[i].Value = false;
                        break;
                    case 4://请选择测试规则
                        RunningState.States[i].State = 0;
                        break;
                    case 10:
                        {
                            RunningState.States[i].ElectriCurrent = current;
                            double rt = (DateTime.Now - RunningState.States[i].RuningTime).TotalSeconds;
                            //double st = (DateTime.Now - RunningState.States[i].StartTime).TotalSeconds;
                            //data.Runtime = (int)rt;
                            RunningState.States[i].Runtime_s = (int)rt;
                            //开始异常检测NG
                            if (rt >= param.AlarmDelay)
                            {
                                RunningState.States[i].AverageCurrent = (current + RunningState.States[i].AverageCurrent) / 2;
                                if (current > RunningState.States[i].MaxCurrent) RunningState.States[i].MaxCurrent = current;
                                if (current < RunningState.States[i].MinCurrent) RunningState.States[i].MinCurrent = current;
                                if (current > param.ElectriCurrentUpper || current < param.ElectriCurrentLower)
                                {
                                    RunningState.States[i].State = 2;
                                    RunningData.Set(i, data);
                                    Devices.继电器组[i].Value = false;
                                    AddRecord(i, data.StartTime, data.TestCount, "FAIL");
                                }
                            }
                            else
                            {
                                RunningState.States[i].AverageCurrent = current;
                                RunningState.States[i].MaxCurrent = current;
                                RunningState.States[i].MinCurrent = current;
                            }
                            //如果时间大于开启时间OK
                            if (rt >= param.PowerOnTime)
                            {
                                RunningState.States[i].RuningTime = DateTime.Now;
                                //RunningState.States[i].StartTime = DateTime.Now;
                                RunningState.States[i].State = 11;
                                //data.Runtime = 0;
                                //data.Tag = 1;
                                RunningData.Set(i, data);
                            }
                            else
                            {
                                Devices.继电器组[i].Value = true;
                            }
                        }
                        break;
                    case 11:
                        {
                            RunningState.States[i].ElectriCurrent = Devices.采集电流[i];
                            double rt = (DateTime.Now - RunningState.States[i].RuningTime).TotalSeconds;
                            //data.Runtime = (int)rt;
                            RunningState.States[i].Runtime_s = (int)rt;
                            //如果时间大于关闭时间
                            if (rt >= param.PowerOffTime)
                            {
                                data.TestCount++;
                                RunningState.States[i].RuningTime = DateTime.Now;
                                //RunningState.States[i].StartTime = DateTime.Now;
                                //data.Tag = 0;
                                //data.Runtime = 0;
                                if (data.TestCount >= param.TotalCount)
                                {
                                    RunningState.States[i].State = 3;
                                    AddRecord(i, data.StartTime, data.TestCount, "PASS");
                                }
                                else
                                {
                                    RunningState.States[i].State = 10;
                                }
                                RunningData.Set(i, data);
                            }
                            else
                            {
                                Devices.继电器组[i].Value = false;
                            }
                        }
                        break;
                }

                RunningState.States[i].RequestStart = false;
                RunningState.States[i].RequestStop = false;

            }
        }
        private void AddRecord(int index, DateTime startTime, int testCount, string result)
        {
            var rs = RunningState.States[index];
            double v = 0;
            if (index < 10)
            {
                v = Devices.采集电压1;
            }
            if (index >= 10 && index < 20)
            {
                v = Devices.采集电压2;
            }
            if (index >= 20 && index < 30)
            {
                v = Devices.采集电压3;
            }
            UserData.Add(new string[] {startTime.ToString(), DateTime.Now.ToString(), (index + 1).ToString("D2"), v.ToString(), rs.AverageCurrent.ToString("F2"),
            rs.MaxCurrent.ToString("F2"),rs.MinCurrent.ToString("F2"), testCount.ToString(), result});
        }
    }
}
