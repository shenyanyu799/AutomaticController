using System;
using System.Collections.Generic;
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

namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : Window
    {
        bool ASOpen = false;
        bool ENOpen = false;
        public StartWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                //Devices.Test();
                Devices.PLC1Start();
                Devices.PLC2Start();
                Devices.TemperatureStart();
                //读取PLCID
                PLC1Data.ID.RequestRead = true;
                PLC2Data.ID.RequestRead = true;
            };
            this.Unloaded += (s, e) => {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };

            this.Closed += (s, e) => Devices.Dispose();
            
        }
        /// <summary>
        /// 循环
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (ASOpen)
            {
                Button1.Content = "Stated";
                Button1.IsEnabled = false;
            }
            else
            {
                Button1.Content = Devices.PLC1.Connected ? "美标 American standard" : "Find for device...";
                Button1.IsEnabled = Devices.PLC1.Connected;
            }
            if (ENOpen)
            {
                Button2.Content = "Stated";
                Button2.IsEnabled = false;
            }
            else
            {
                Button2.Content = Devices.PLC2.Connected ? "欧标 European standard" : "Find for device...";
                Button2.IsEnabled = Devices.PLC2.Connected;
            }
            //TemperatureData.RelativeHidity.RequestRead = true;

            //if (Devices.Temperature.Started)
            //{
            //    Console.WriteLine($"相对湿度：{TemperatureData.RelativeHidity} 干球温度：{TemperatureData.DrybulbTemperature1} 湿球温度：{TemperatureData.WetBulbTemperature1} 大气压强：{TemperatureData.Atmospheric1}");
            //    Console.WriteLine(Devices.Temperature.CommunicationDelay);
            //}
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new AmericanWindow();
            ASOpen = true;
            win.Closed += (s, _e) => {
                ASOpen = false;
            };
            win.Show();
            //this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var win = new EuropeanWindow();
            ENOpen = true;
            win.Closed += (s, _e) => {
                ENOpen = false;
            };
            win.Show();
            //this.Close();
        }
    }
}
