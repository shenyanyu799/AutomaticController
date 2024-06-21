using AutomaticController.Device;
using AutomaticController.Function;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutomaticController.Windows.Demos.PLC通讯样例.Pages
{
    /// <summary>
    /// Page2.xaml 的交互逻辑
    /// </summary>
    public partial class Page3 : Page
    {
        public Page3()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering; ;
            };
            this.Unloaded += (s, e) => {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DeviceLink.站号波特率.RequestRead = true;
        }
        SerialPort port = new SerialPort("COM4");//创建串口
        private void Open()
        {
            if (port.IsOpen)
            {
                return;
            }
            port.BaudRate = 9600;//设置波特率
            port.Parity = Parity.None;//设置偶校验

            port.StopBits = StopBits.One;//停止位位1
            port.ReadTimeout = 1000;
            //port.WriteTimeout = 200;//写入超时1s
            port.Open();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Open();
            //Thread thread = new Thread(Test1);
            //thread.Start();
            //Task.Run(Test1);
            Test1();
        }
        public void Test1()
        {
            port.DataReceived += Port_DataReceived;
            for (int i = 0; i < 256; i++)
            {
                Debug.Write("Read " + i + ":    ");

                var ws = Modbus_RTU.ReadData_Code((byte)i, 0x20, 1);
                port.Write(ws,0,ws.Length);
                Task.Delay(100).Wait();
                byte[] bs = new byte[20];
                try
                {
                    port.Read(bs, 0, bs.Length);
                }
                catch { }
                int len = port.Read(bs,0, bs.Length);
                Debug.WriteLine(bs?.ToStringH(0,len));
            }
          
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
