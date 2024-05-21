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

namespace AutomaticController.Windows.Demos.数据上传报表
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            comRefresh(ComText);
            comRefresh(sweepComText);
            ComText.SelectedValue = Setting.Example.ComName;
            sweepComText.SelectedValue = Setting.Example.SweepComName;
            baudText.Text = Setting.Example.Baud.ToString();
            databitText.Text = Setting.Example.Databit.ToString();
            AddsText.Text = Setting.Example.Address.ToString();
            verifyText.Text = Setting.Example.Verify.ToString();
            stopbitText.Text = Setting.Example.Stopbit.ToString();


        }
        //退出
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //保存
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Setting.Example.ComName = ComText.Text;
            Setting.Example.SweepComName = sweepComText.Text;

            int baud = Setting.Example.Baud;
            int.TryParse(baudText.Text, out baud);
            Setting.Example.Baud = baud;

            int databit = Setting.Example.Databit;
            int.TryParse(databitText.Text, out databit);
            Setting.Example.Databit = databit;

            byte adds = Setting.Example.Address;
            byte.TryParse(AddsText.Text, out adds);
            Setting.Example.Address = adds;

            Parity verify = Setting.Example.Verify;
            Enum.TryParse(verifyText.Text, out verify); 
            Setting.Example.Verify = verify;

            StopBits stopBits = Setting.Example.Stopbit;
            Enum.TryParse(stopbitText.Text, out stopBits);
            Setting.Example.Stopbit = stopBits;

            Setting.Save();
            this.Close();
        }
        //产品端口下拉打开时刷新
        void comRefresh(ComboBox combo)
        {
            string st = combo.Text;
            combo.Items.Clear();
            string[] coms = System.IO.Ports.SerialPort.GetPortNames();
            int i = 0;
            int s = 0;
            foreach (string com in coms)
            {
                if (com == st)
                {
                    s = i;
                }
                combo.Items.Add(com);
                i++;
            }
            combo.SelectedIndex = s;
        }
        /// <summary>
        ///产品端口下拉打开时刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComText_DropDownOpened(object sender, EventArgs e)
        {
            comRefresh((ComboBox)sender);
        }


    }
}
