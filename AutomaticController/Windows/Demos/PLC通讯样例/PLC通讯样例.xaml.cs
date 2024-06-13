using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutomaticController.Device;
using AutomaticController.Windows.Demos.PLC通讯样例.Pages;

namespace AutomaticController.Windows.Demos.PLC通讯样例
{
    /// <summary>
    /// PLC通讯样例.xaml 的交互逻辑
    /// </summary>
    public partial class PLC通讯样例 : Window
    {
        Dictionary<string,Page> pages = new Dictionary<string,Page>();
        public PLC通讯样例()
        {
            InitializeComponent();
            this.Closed += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;//移除画面渲染事件
                //窗口关闭时，关闭通讯
                DeviceLink.PLC.Close();
            };
            CompositionTarget.Rendering += CompositionTarget_Rendering;//添加画面渲染事件


            pages.Add("Page1", new Page1());//添加页面到字典方便查
            pages.Add("Page2", new Page2());//添加页面到字典方便查
            pages.Add("Page3", new Page3());//添加页面到字典方便查
            UserFrame.Navigate(pages["Page3"]);//加载首页面
        }
        //画面渲染事件
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //实时显示PLC通讯状态
            if (DeviceLink.PLC.Connected)
            {
                stateText.Text = $"PLC:{DeviceLink.PLC.CommunicationDelay.ToString("0.00")}ms";
                stateText.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                stateText.Text = "PLC未连接";
                stateText.Foreground = new SolidColorBrush(Colors.Red);
            }
            if (DeviceLink.PLC.Started)
                ComSelect1.Header = DeviceLink.PLC.Serial.PortName;
        }

        //鼠标按下窗口时移除键盘焦点
        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();//让输入框变得更加丝滑
        }
        //按下菜单
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem)//如果按下的是菜单选项
            {
                MenuItem item = (MenuItem)sender;//强制转换
                Page page = null;
                if (pages.TryGetValue(item.Header.ToString(), out page))
                {
                    UserFrame.Navigate(page);
                }
            }
        }

        private void ComSelect1_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            ComSelect1.Items.Clear();
            string[] names = SerialPort.GetPortNames();
            foreach (string name in names)
            {
                ComSelect1.Items.Add(name);
            }
        }

        private void ComSelect1_Click(object sender, RoutedEventArgs e)
        {
            string h = ((MenuItem)e.OriginalSource).Header.ToString();
            if (DeviceLink.PLC.Started == true)
            {
                DeviceLink.PLC.Close();
            }
            ComSelect1.Header = "打开COM口";
            Task.Delay(500).ContinueWith(t => DeviceLink.PLCStart(h));//延时0.5秒后启动
        }
    }



}
