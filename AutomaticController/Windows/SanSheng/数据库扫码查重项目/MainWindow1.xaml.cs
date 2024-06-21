using AutomaticController.Windows.SanSheng.数据库扫码查重项目.Pages;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AutomaticController.Windows.SanSheng.数据库扫码查重项目
{
    /// <summary>
    /// MainWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow1 : Window
    {
        public static string SNCode { get; set; }
        public static string Module { get; set; }//当前选择的型号
        private static int testCount = 3;
        public static int TestCount { get => testCount; set {
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    UserArgument user = new UserArgument();
                    user.TestCount = value;
                    var lst = lite.GetCollection<UserArgument>("args");
                    lst.DeleteAll();
                    lst.Insert(user);
                    testCount = value;
                    WorkState = 0;
                }
            } }
        /// <summary>
        /// 工作状态机
        /// </summary>
        public static int WorkState = 0;
        /// <summary>
        /// 单次流程显示状态
        /// </summary>
        public static int[] PartState = new int[3];
        public static string[] Codes = new string[3];

        public static List<UserData> SelectData { get; set; }
        public static Dictionary<string, List<UserData>> DicData = new Dictionary<string, List<UserData>>();


        public static UserPermissions Logined { get; set; } = UserPermissions.Normal;//登入权限

        List<Page> pages = new List<Page>();
        public MainWindow1()
        {
            if (System.IO.Directory.Exists("data") == false)
            {
                System.IO.Directory.CreateDirectory("data");
            }
            Setting.Load();
            BarcodeOpen();

            InitializeComponent();
            LoadModule(Setting.Instance.Name);//加载产品型号
            ListView1.MouseDoubleClick += ListView1_MouseDoubleClick;

            //创建并加载页面
            pages.Add(new Monitor());
            pages.Add(new DataManager()); 
            pages.Add(new UsetSetting());
            UserFrame.Navigate(pages[0]);
            Button1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xED, 0xF3));

            bool start = true;
            this.Closed += (s, e) => start = false;
            Task.Run(async () =>
            {
                while (start)
                {
                    try
                    {
                        Loop();
                    }
                    catch { }
                    await Task.Delay(10);
                }
            });

        }


        #region 扫码枪通讯
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
                            Barcode.PortName = Setting.Instance.BarcodeName;
                            Barcode.Parity = Setting.Instance.BarcodeVerify;
                            Barcode.BaudRate = Setting.Instance.BarcodeBaud;
                            Barcode.StopBits = Setting.Instance.BarcodeStopbit;
                            Barcode.DataBits = Setting.Instance.BarcodeDatabit;
                            Barcode.ReadTimeout = 100;
                            Barcode.Open();
                            App.Current.Dispatcher.InvokeAsync(() =>
                            {
                                Barcodestate.Text = "已连接";
                                Barcodestate.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x08, 0xFF, 0x00));

                            });
                        }
                        else
                        {
                            if (Barcode.BytesToRead > 0)
                            {
                                await Task.Delay(50);
                                SNCode = Barcode.ReadExisting().Trim();
                            }
                        }

                    }
                    catch
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Barcodestate.Text = "未连接";
                            Barcodestate.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));

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
        /// <summary>
        /// 界面控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button1.Background = new SolidColorBrush(Color.FromArgb(0xFF,0xDD,0xDD,0xDD));
            Button2.Background = new SolidColorBrush(Color.FromArgb(0xFF,0xDD,0xDD,0xDD));
            Button3.Background = new SolidColorBrush(Color.FromArgb(0xFF,0xDD,0xDD,0xDD));
            string name = (sender as Button).Content.ToString();

            switch(name)
            {
                case "运行监控":
                    UserFrame.Navigate(pages[0]);
                    Button1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xED, 0xF3));
                    break;
                case "数据管理":
                    if (Logined == UserPermissions.Normal)
                    {
                        Login login = new Login();
                        login.Owner = this;
                        login.Show();
                        login.Closed += (s, _e) => {
                            if (Logined != UserPermissions.Normal)
                            {
                                UserFrame.Navigate(pages[1]);
                                Button2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xED, 0xF3));
                            }
                        };
                        break;
                    }
                    UserFrame.Navigate(pages[1]);
                    Button2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xED, 0xF3));
                    break;
                case "设置":
                    if (Logined == UserPermissions.Normal)
                    {
                        Login login = new Login();
                        login.Owner = this;
                        login.Show();
                        login.Closed += (s, _e) => {
                            if (Logined != UserPermissions.Normal)
                            {
                                UserFrame.Navigate(pages[2]);
                                Button3.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xED, 0xF3));
                            }
                        };
                        break;
                    }
                    UserFrame.Navigate(pages[2]);
                    Button3.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x91, 0xED, 0xF3));
                    break;
            }
        }

        /// <summary>
        /// 加载参数模块
        /// </summary>
        public void LoadModule(string module = "default")
        {

            DirectoryInfo dir = new DirectoryInfo("data");
            
            ListView1?.Items?.Clear();
            var files = dir.GetFiles();
            Array.Sort(files, (FileInfo x, FileInfo y) => x.CreationTime.CompareTo(y.CreationTime));//时间顺序排序
            foreach (var item in files)
            {
                ListView1.Items.Add(item.Name);
            }
            ////判断文件是否存在，如果不存在则选择默认
            //bool b = false;
            //foreach (var file in dir.EnumerateFiles())
            //{
            //    if(file.Name == module)
            //    {
            //        b = true;
            //    }
            //}
            //Module = b ? module : "default";
            Module = module;
            if (File.Exists(UserData.DBPath) == false)
            {
                ListView1.Items.Add(module);
            }
            if (DicData.ContainsKey(Module) == false)
            {
                DicData.Add(Module, new List<UserData>());
            }
            SelectData = DicData[Module];

            Task.Run(() =>
            {
                //加载用户参数
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    var lst = lite.GetCollection<UserArgument>("args");
                    var rt = lst.FindAll()?.GetEnumerator();
                    rt.MoveNext();
                    if (rt.Current != null)
                    {
                        testCount = rt.Current.TestCount;
                    }
                }
            });
            Setting.Instance.Name = module;
            Setting.Save();
        }
        //双击切换产品型号时确认
        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string type = ListView1.SelectedValue.ToString();
            if (type == Module) return;
            if (MessageBox.Show($"是否更换产品型号为【{type}】", "型号选择", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                LoadModule(type);
            }
            WorkState = 0;
        }
        /// <summary>
        /// 右击新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {            
            int i = 1;
            while (true)
            {
                string name = "data/Module" + i;

                if (File.Exists(name) == false)
                {
                    Module = "Module" + i;
                    TestCount = TestCount;
                    LoadModule(Module);
                    return;
                }
                i++;
            }
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var obj = ListView1.SelectedItem;
            int i = ListView1.SelectedIndex;
            ListView1.Items.RemoveAt(i);
            TextBox textBox = new TextBox();
            ListView1.Items.Insert(i, textBox);
            textBox.Text = obj.ToString();
            textBox.LostFocus += (s, _e) =>
            {
                try
                {
                    File.Move("data/" + obj, "data/" + textBox.Text);
                    ListView1.Items.RemoveAt(i);
                    ListView1.Items.Insert(i, textBox.Text);
                    Module = textBox.Text;
                    LoadModule(Module);
                }
                catch
                {
                    ListView1.Items.RemoveAt(i);
                    ListView1.Items.Insert(i, obj);
                }

                ListView1.UpdateLayout();
                ListView1.MouseDoubleClick += ListView1_MouseDoubleClick;
            };
            textBox.KeyDown += (s, _e) =>
            {
                if(_e.Key == Key.Enter)
                {
                    textBox.Focusable = false;
                }
                
            };
            ListView1.MouseDoubleClick -= ListView1_MouseDoubleClick;
            //延时触发
            Task.Delay(10).ContinueWith(t => {
                App.Current.Dispatcher.Invoke(() =>
                {
                    textBox.Focus();
                    textBox.SelectAll();
                });
            });
        }
        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="userData"></param>
        public static void Add(UserData userData, LiteDatabase lite)
        {
            try
            {
                if (DicData.ContainsKey(Module) == false)
                {
                    DicData.Add(Module, new List<UserData>());
                }
                var list = DicData[Module];
                list.Insert(0, userData);
                //if (list.Count > 100)
                //{
                //    list.RemoveAt(list.Count - 1);
                //}
                var lst = lite.GetCollection<UserData>("生产记录");
                lst.Insert(userData.ID, userData);
                Monitor.Add();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        int lstCount = 0;
        /// <summary>
        /// 循环流程
        /// </summary>
        public void Loop()
        {
            switch (WorkState)
            {
                case 0://等待扫码状态
                    if (TestCount == 1)
                    {
                        PartState[0] = 1;//等待扫码
                        PartState[1] = 0;//等待扫码
                        PartState[2] = 0;//等待扫码
                    }
                    if (TestCount == 2)
                    {
                        PartState[0] = 1;
                        PartState[1] = 1;
                        PartState[2] = 0;
                    }
                    if (TestCount == 3)
                    {
                        PartState[0] = 1;
                        PartState[1] = 1;
                        PartState[2] = 1;
                    }
                    if(SNCode?.Length > 0)//扫码完成
                    {
                        WorkState = 1;
                    }
                    break;
                case 10://重码NG
                    if (SNCode?.Length > 0)//扫码完成
                    {
                        WorkState = 1;
                        PartState[0] = 1;//等待扫码
                        PartState[1] = 1;//等待扫码
                        PartState[2] = 1;//等待扫码
                    }
                    break;
                case 1://扫码1查重
                    Codes[0] = SNCode;
                    using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                    {
                        var lst = lite.GetCollection<UserData>("生产记录");
                        
                        //SN搜索
                        var list = lst.Find(x => x.SN码 == SNCode);
                        lstCount = lst.Count();

                        if (list.Count() > 0)
                        {
                            WorkState = 10;
                            PartState[0] = 3;
                        }
                        else
                        {
                            PartState[0] = 2;
                            if (TestCount > 1)
                            {
                                WorkState = 2;
                            }
                            else
                            {
                                MainWindow1.Add(new UserData(new string[] { lstCount.ToString(), DateTime.Now.ToString(), MainWindow1.Module, MainWindow1.SNCode }), lite);
                                Codes[0] = "";
                                Codes[1] = "";
                                Codes[2] = "";
                                WorkState = 0;
                            }
                        }
                    }
                    SNCode = "";
                    break;
                case 2:
                    if (SNCode?.Length > 0)//扫码完成
                    {
                        WorkState = 3;
                    }
                    break;
                case 3://扫码2查重
                    Codes[1] = SNCode;
                    if (Codes[1] == Codes[0])
                    {
                        PartState[1] = 2;
                        if (TestCount > 2)
                        {
                            WorkState = 4;
                        }
                        else
                        {
                            using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                            {
                                MainWindow1.Add(new UserData(new string[] { lstCount.ToString(), DateTime.Now.ToString(), MainWindow1.Module, MainWindow1.SNCode }), lite);
                                Codes[0] = "";
                                Codes[1] = "";
                                Codes[2] = "";
                            }
                            WorkState = 0;
                        }
                    }
                    else
                    {
                        WorkState = 10;
                        PartState[1] = 3;
                    }
                    SNCode = "";
                    break;
                case 4:
                    if (SNCode?.Length > 0)//扫码完成
                    {
                        WorkState = 5;
                    }
                    break;
                case 5://扫码3查重
                    Codes[2] = SNCode;
                    if (Codes[2] == Codes[0])
                    {
                        PartState[2] = 2;
                        using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                        {
                            MainWindow1.Add(new UserData(new string[] { lstCount.ToString(), DateTime.Now.ToString(), MainWindow1.Module, MainWindow1.SNCode }), lite);
                            Codes[0] = "";
                            Codes[1] = "";
                            Codes[2] = "";
                        }
                        WorkState = 0;
                    }
                    else
                    {
                        WorkState = 10;
                        PartState[2] = 3;
                    }
                    SNCode = "";
                    break;


            }
        }


    }
    public enum UserPermissions
    {
        Normal = 1,
        Operator = 2,
        Administrator = 9,
    }
    /// <summary>
    /// 用户参数
    /// </summary>
    public class UserArgument
    {
        public int TestCount { get; set; }
    }
    public class Setting
    {
        public string Name { get; set; } = "default";
        public string UserPassword { get; set; } = "123";


        public string BarcodeName { get; set; }
        public int BarcodeBaud { get; set; } = 9600;
        public System.IO.Ports.Parity BarcodeVerify { get; set; } = System.IO.Ports.Parity.None;
        public int BarcodeDatabit { get; set; } = 8;
        public System.IO.Ports.StopBits BarcodeStopbit { get; set; } = System.IO.Ports.StopBits.One;

        public static Setting Instance { get; set; }
        public static void Load()
        {
            if (File.Exists("AutomaticController"))
            {
                string json = File.ReadAllText("AutomaticController");
                try
                {
                    Instance = JsonConvert.DeserializeObject<Setting>(json);
                }
                catch { }
            }

            if (Instance == null) Instance = new Setting();
        }
        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Instance);
            File.WriteAllText("AutomaticController", json);
            //SetData("AutomaticController.db", "setting", JsonConvert.SerializeObject(Instance));
        }
    }
}
