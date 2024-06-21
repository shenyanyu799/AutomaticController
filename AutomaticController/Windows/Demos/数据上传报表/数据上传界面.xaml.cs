using AutomaticController.Device;
using AutomaticController.Function;
using LiteDB;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.Demos.数据上传报表
{
    /// <summary>
    /// 数据上传界面.xaml 的交互逻辑
    /// </summary>
    public partial class 数据上传界面 : System.Windows.Window
    {
        //modbus寄存器，存储为字节，当作modbus地址使用是需要除2。
        UnitData Data = new UnitData(256);
        float 空转转速 => (float)Math.Round(Data.GetFloat_hlHL(0), 3);
        float 空转扭矩 => (float)Math.Round(Data.GetFloat_hlHL(4), 3);
        float 空转电流 => (float)Math.Round(Data.GetFloat_hlHL(8), 3);
        float 堵转转速 => (float)Math.Round(Data.GetFloat_hlHL(12), 3);
        float 堵转扭矩 => (float)Math.Round(Data.GetFloat_hlHL(16), 3);
        float 堵转电流 => (float)Math.Round(Data.GetFloat_hlHL(20), 3);
        short 检测结果 => Data.GetInt16_HL(24);








        bool DataWrite = false;
        //地址H50
        short SNCodeLength { get => Data.GetInt16_HL(160); set => Data.SetInt16_HL(160, value); }


        //扫码枪状态
        bool IsSweepOpen = false;
        public string SNCode
        {
            get
            {
                string r = "";
                App.Current.Dispatcher.Invoke(() =>
                {
                    r = snCodeBox.Text;
                });
                return r;
            }
            set
            {
                App.Current.Dispatcher.Invoke(() => snCodeBox.Text = value);
            }
        }
        public 数据上传界面()
        {
            Setting.Load();
            InitializeComponent();
            DataGrid1.ItemsSource = new List<UserData>();
            //添加首行序号
            DataGrid1.LoadingRow += (object sender, DataGridRowEventArgs e) => e.Row.Header = e.Row.GetIndex() + 1;

            //文本框改变后设置SN码长度
            snCodeBox.TextChanged += (o, e) =>
            {
                SNCodeLength = (short)snCodeBox.Text.Length;
            };
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                Task.Run(OpenSweep);
                Task.Run(OpenModBusServer);
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
            this.Closed += (s, e) => { IsClose = true; };
            startTimeText.DateTime = DateTime.Now;
            endTimeText.DateTime = DateTime.Now;


        }
        /// <summary>
        /// 应用关闭
        /// </summary>
        bool IsClose;
        /// <summary>
        /// 打开扫码枪
        /// </summary>
        private async void OpenSweep()
        {
            System.IO.Ports.SerialPort serialPort = new SerialPort();
            while (IsClose == false)
            {
                try
                {
                    if (serialPort.IsOpen == false)
                    {
                        serialPort.Close();
                        serialPort = new SerialPort();
                        serialPort.PortName = Setting.Example.SweepComName;
                        serialPort.DataBits = 8;
                        serialPort.StopBits = StopBits.One;
                        serialPort.BaudRate = 9600;
                        serialPort.Open();
                        IsSweepOpen = true;
                    }
                    else
                    {
                        if (serialPort.PortName != Setting.Example.SweepComName)
                        {
                            serialPort.Close();
                            IsSweepOpen = false;
                            continue;
                        }
                        //如果扫到码
                        if (serialPort.BytesToRead > 0)
                        {
                            await Task.Delay(100);
                            SNCode = serialPort.ReadExisting().Trim();
                        }


                    }
                }
                catch (Exception e)
                {
                    IsSweepOpen = false;
                    Console.WriteLine(e);
                    await Task.Delay(1000);
                }
                await Task.Delay(100);
            }
        }
        /// <summary>
        /// 打开Modbus服务器
        /// </summary>
        private async void OpenModBusServer()
        {
            System.IO.Ports.SerialPort serialPort = new SerialPort();
            serialPort.ReadTimeout = 100;
            bool aWrite = false;
            while (IsClose == false)
            {
                try
                {
                    byte adds = Setting.Example.Address;
                    if (serialPort.IsOpen == false)
                    {
                        serialPort.Close();
                        serialPort = new SerialPort();
                        serialPort.PortName = Setting.Example.ComName;
                        serialPort.DataBits = Setting.Example.Databit;
                        serialPort.StopBits = Setting.Example.Stopbit;
                        serialPort.BaudRate = Setting.Example.Baud;
                        serialPort.Parity = Setting.Example.Verify;
                        serialPort.Open();
                    }
                    else
                    {
                        //如果参数被改变则重新开启服务器
                        bool close = false;
                        if (serialPort.PortName != Setting.Example.ComName) close = true;
                        if (serialPort.DataBits != Setting.Example.Databit) close = true;
                        if (serialPort.StopBits != Setting.Example.Stopbit) close = true;
                        if (serialPort.Parity != Setting.Example.Verify) close = true;
                        if (serialPort.BaudRate != Setting.Example.Baud) close = true;
                        if (close)
                        {
                            serialPort.Close();
                            continue;
                        }

                        //如果接收到数据
                        byte[] bytes = new byte[255];
                        bytes[0] = (byte)serialPort.ReadByte();

                        if (bytes[0] == Setting.Example.Address)
                        {
                            bytes[1] = (byte)serialPort.ReadByte();
                            if (bytes[1] == 0x10)//写地址
                            {
                                try
                                {
                                    bytes[2] = (byte)serialPort.ReadByte();
                                    bytes[3] = (byte)serialPort.ReadByte();

                                    bytes[4] = (byte)serialPort.ReadByte();
                                    bytes[5] = (byte)serialPort.ReadByte();

                                    short s = 0;
                                    short len = 0;
                                    byte count = 0;
                                    s = BitConverter.ToInt16(bytes, 2).SwapHL();
                                    len = BitConverter.ToInt16(bytes, 4).SwapHL();
                                    count = (byte)serialPort.ReadByte();
                                    bytes[6] = count;

                                    int n = 0;
                                    for (int i = 0; i < count; i++)
                                    {
                                        n = i + 7;
                                        bytes[n] = (byte)serialPort.ReadByte();
                                    }
                                    var crc = Modbus_RTU.CRC16(bytes, n + 1);
                                    bytes[n + 1] = (byte)serialPort.ReadByte();
                                    bytes[n + 2] = (byte)serialPort.ReadByte();
                                    //await Task.Delay(2);
                                    if (bytes[n + 1] == crc[1] && bytes[n + 2] == crc[0])
                                    {
                                        var crc2 = Modbus_RTU.CRC16(bytes, 6);
                                        serialPort.Write(bytes, 0, 6);
                                        byte r = crc2[0];
                                        crc2[0] = crc2[1];
                                        crc2[1] = r;
                                        serialPort.Write(crc2, 0, 2);
                                        for (int i = 0; i < count; i++)
                                        {
                                            n = i + 7;
                                            Data[i] = bytes[n];
                                        }
                                        if (aWrite == false)//写入完后不能立即写入
                                        {
                                            DataWrite = true;
                                            Task.Delay(1000).ContinueWith(t => aWrite = false);
                                        }
                                        aWrite = true;
                                    }
                                    continue;
                                }
                                catch { continue; }

                            }
                            if (bytes[1] == 0x03)//读地址
                            {
                                try
                                {
                                    bytes[2] = (byte)serialPort.ReadByte();
                                    bytes[3] = (byte)serialPort.ReadByte();

                                    bytes[4] = (byte)serialPort.ReadByte();
                                    bytes[5] = (byte)serialPort.ReadByte();

                                    short s = 0;
                                    short len = 0;
                                    s = BitConverter.ToInt16(bytes, 2).SwapHL();
                                    len = BitConverter.ToInt16(bytes, 4).SwapHL();

                                    var crc = Modbus_RTU.CRC16(bytes, 6);
                                    bytes[6] = (byte)serialPort.ReadByte();
                                    bytes[7] = (byte)serialPort.ReadByte();
                                    //await Task.Delay(2);
                                    if (bytes[6] == crc[1] && bytes[7] == crc[0])
                                    {
                                        byte[] bytes1 = new byte[len * 2 + 5];
                                        bytes1[0] = Setting.Example.Address;
                                        bytes1[1] = 0x03;
                                        bytes1[2] = (byte)(len * 2);

                                        int index = s * 2;
                                        int length = len * 2;
                                        for (int i = 0; i < length; i++)
                                        {
                                            bytes1[3 + i] = Data[index + i];
                                        }
                                        var crc1 = Modbus_RTU.CRC16(bytes1, bytes1.Length - 2);
                                        bytes1[bytes1.Length - 1] = crc1[0];
                                        bytes1[bytes1.Length - 2] = crc1[1];
                                        serialPort.Write(bytes1, 0, bytes1.Length);
                                    }
                                    continue;
                                }
                                catch { continue; }
                            }

                        }

                    }
                }
                catch (Exception e)
                {
                    IsSweepOpen = false;
                    Console.WriteLine(e);
                    await Task.Delay(1000);
                }
            }

        }
        /// <summary>
        ///添加数据到表格，同时写入本地数据库
        /// </summary>
        /// <param name="data"></param>
        public void Add(UserData data)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    List<UserData> ls = new List<UserData>((DataGrid1.ItemsSource as List<UserData>));
                    if (ls == null) ls = new List<UserData>();
                    ls.Insert(0, data);
                    DataGrid1.ItemsSource = ls;
                    (DataGrid1.Columns[0] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                    UserData.DBAdd(data);
                }
                catch { }
            });
        }
        Brush green = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        Brush red = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        /// <summary>
        /// 循环事件，选择写入表格的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (IsSweepOpen)
            {
                smqZT.Text = Setting.Example.SweepComName + " 已连接";
                smqZT.Foreground = green;
            }
            else
            {
                smqZT.Text = Setting.Example.SweepComName + " 未连接";
                smqZT.Foreground = red;
            }

            comName.Text = Setting.Example.ComName;
            baudText.Text = Setting.Example.Baud.ToString();
            baudText.Text = Setting.Example.Baud.ToString();
            verifyText.Text = Setting.Example.Verify.ToString();
            AddsText.Text = Setting.Example.Address.ToString();

            //数据写入
            if (DataWrite)
            {
                DataWrite = false;
                Task.Run(() =>
                {
                    string jg = "Test";
                    if (检测结果 == 1) jg = "OK";
                    if (检测结果 == 2) jg = "NG";
                    //Console.WriteLine($"{空转转速},{空转扭矩},{空转电流},{堵转转速},{堵转扭矩},{堵转电流},{jg}");
                    Add(new UserData(new string[] { DateTime.Now.ToString(), SNCode,
                    空转转速.ToString(), 空转扭矩.ToString(), 空转电流.ToString(), 堵转转速.ToString(), 堵转扭矩.ToString(), 堵转电流.ToString(), jg }));
                    SNCode = "";
                });
            }
        }

        #region 按钮事件

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow setting = new SettingWindow();
            setting.Owner = this;
            setting.Show();
        }
        //查询状态
        bool finding = false;


        /// <summary>
        /// SN码查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            finding = true;
            DataGrid1.ItemsSource = null;
            var sd = startTimeText.DateTime;
            var ed = endTimeText.DateTime;
            string code = SNCode;
            Task.Run(() =>
            {
                List<UserData> users = new List<UserData>();
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    var lst = lite.GetCollection<UserData>("生产记录");
                    //时间范围搜索
                    users.AddRange(lst.Find(x => x.SN码 == code));
                }
                //转换并显示
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            //users.Reverse();//反转序列
                            DataGrid1.ItemsSource = users;
                            (DataGrid1.Columns[0] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                        }
                        catch { }
                    });
                finding = false;
            });
        }
        /// <summary>
        /// 时间查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            finding = true;
            DataGrid1.ItemsSource = null;
            var sd = startTimeText.DateTime;
            var ed = endTimeText.DateTime;
            Task.Run(() =>
            {
                List<UserData> users = new List<UserData>();
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    var lst = lite.GetCollection<UserData>("生产记录");
                    //时间范围搜索
                    users.AddRange(lst.Find(x => sd <= x.检测时间 && x.检测时间 < ed));
                }
                //转换并显示
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            //users.Reverse();//反转序列
                            DataGrid1.ItemsSource = users;
                            (DataGrid1.Columns[0] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                        }
                        catch { }
                    });
                finding = false;
            });
        }
        /// <summary>
        /// 全部查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            finding = true;
            DataGrid1.ItemsSource = null;
            var sd = startTimeText.DateTime;
            var ed = endTimeText.DateTime;
            Task.Run(() =>
            {
                List<UserData> users = new List<UserData>();
                users = UserData.DBFindAll();
                //转换并显示
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            //users.Reverse();//反转序列
                            DataGrid1.ItemsSource = users;
                            (DataGrid1.Columns[0] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                        }
                        catch { }
                    });
                finding = false;
            });
        }
        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "表格(*.csv)|*.csv";
            saveFile.Title = "导出数据";
            try
            {
                if (saveFile.ShowDialog() == true)
                {
                    using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                    {
                        List<UserData> datas = (List<UserData>)DataGrid1.ItemsSource;
                        string s = "";
                        if (datas != null)
                        {
                            var ns = UserData.GetNames();
                            foreach (var item in ns)
                            {
                                s += $"\"{item}\",";
                            }
                        }
                        s = s.TrimEnd(',') + "\n";
                        byte[] bytes = Encoding.UTF8.GetBytes(s);
                        fs.Write(bytes, 0, bytes.Length);
                        foreach (var item in datas)
                        {
                            bytes = Encoding.UTF8.GetBytes(item.ToString3() + "\n");
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        fs.Close();
                        MessageBox.Show("导出完成");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        #endregion

    }
    public class Setting
    {
        public static Setting Example { get; set; }
        public string ComName { get; set; } = "Com1";
        public int Baud { get; set; } = 9600;
        public Parity Verify { get; set; } = Parity.None;
        public int Databit { get; set; } = 8;
        public StopBits Stopbit { get; set; } = StopBits.One;
        public byte Address { get; set; } = 1;
        /// <summary>
        /// 扫码枪地址
        /// </summary>
        public string SweepComName { get; set; } = "Com1";

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Example, Formatting.Indented);
            File.WriteAllText("setting.json", json);
        }
        public static void Load()
        {
            if (File.Exists("setting.json") == false)
            {
                Example = new Setting();
                return;
            }
            string json = File.ReadAllText("setting.json");
            var st = JsonConvert.DeserializeObject<Setting>(json);
            if (st != null)
            {
                Example = st;
            }
            else
            {
                Example = new Setting();
            }

        }
    }

    /// <summary>
    /// 表的内容
    /// </summary>
    public class UserData
    {
        #region 功能

        public UserData() { }

        public static string[] GetNames()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string[] names = new string[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                names[i] = ps[i].Name;
            }
            return names;
        }
        public override string ToString()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string r = "";
            foreach (var item in ps)
            {
                r += item.GetValue(this) + ",";
            }
            return r.TrimEnd(',');
        }
        public string ToString2()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string r = "";
            foreach (var item in ps)
            {
                r += "'" + item.GetValue(this) + "',";
            }
            return r.TrimEnd(',');
        }
        /// <summary>
        /// 导出CSV时用
        /// </summary>
        /// <returns></returns>
        public string ToString3()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string r = "";
            foreach (var item in ps)
            {
                r += "\"" + item.GetValue(this) + "\",";
            }
            return r.TrimEnd(',');
        }

        public UserData(string[] args)
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            int len = ps.Length;
            if (len > args.Length)
            {
                len = args.Length;
            }

            for (int i = 0; i < len; i++)
            {

                if (ps[i].PropertyType == typeof(string))
                {
                    ps[i].SetValue(this, args[i]);
                    continue;
                }
                if (ps[i].PropertyType == typeof(DateTime))
                {
                    ps[i].SetValue(this, DateTime.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Double))
                {
                    ps[i].SetValue(this, Double.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Single))
                {
                    ps[i].SetValue(this, Single.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Int32))
                {
                    ps[i].SetValue(this, Int32.Parse(args[i]));
                    continue;
                }

                if (ps[i].PropertyType == typeof(Int16))
                {
                    ps[i].SetValue(this, Int16.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Int64))
                {
                    ps[i].SetValue(this, Int64.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(UInt32))
                {
                    ps[i].SetValue(this, UInt32.Parse(args[i]));
                    continue;
                }

                if (ps[i].PropertyType == typeof(UInt16))
                {
                    ps[i].SetValue(this, UInt16.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(UInt64))
                {
                    ps[i].SetValue(this, UInt64.Parse(args[i]));
                    continue;
                }
            }
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="args"></param>
        public static void Add(string[] args)
        {
            UserData user = new UserData(args);
            try
            {
                DBAdd(user);
            }
            catch
            {
                MessageBox.Show("数据库存储失败");
            }

        }

        public static void DBAdd(UserData data)
        {
            using (LiteDatabase lite = new LiteDatabase(DBPath))
            {
                var lst = lite.GetCollection<UserData>(TableName);
                lst.Insert(data);
            }
        }
        public static void DBAdd(IEnumerable<UserData> datas)
        {
            using (LiteDatabase lite = new LiteDatabase(DBPath))
            {
                var lst = lite.GetCollection<UserData>(TableName);
                lst.Insert(datas);
            }
        }
        public static List<UserData> DBFindAll()
        {
            List<UserData> userDatas = new List<UserData>();
            using (LiteDatabase lite = new LiteDatabase(DBPath))
            {
                var lst = lite.GetCollection<UserData>(TableName);
                userDatas.AddRange(lst.FindAll());
            }
            return userDatas;
        }
        public static string DBPath => "data";
        public static string TableName => "生产记录";
        #endregion


        public DateTime 检测时间 { get; set; }
        public string SN码 { get; set; }
        public string 空转转速 { get; set; }
        public string 空转扭矩 { get; set; }
        public string 空转电流 { get; set; }
        public string 堵转转速 { get; set; }
        public string 堵转扭矩 { get; set; }
        public string 堵转电流 { get; set; }
        public string 检测结果 { get; set; }


    }

}
