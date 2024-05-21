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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutomaticController.Windows.SanSheng.数据库扫码查重项目.Pages
{
    /// <summary>
    /// UsetSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UsetSetting : Page
    {
        public UsetSetting()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                //遍历并检测权限
                foreach (FrameworkElement item in StackPanel1.Children)
                {
                    int n = 0;
                    int.TryParse(item.Tag.ToString(), out n);
                    if((int)MainWindow1.Logined < n)
                    {
                        item.IsEnabled = false;
                    }
                }
                if (userPassText.IsEnabled)
                    userPassText.Text = Setting.Instance.UserPassword;

                ComText.Text = Setting.Instance.BarcodeName;
                baudText.Text = Setting.Instance.BarcodeBaud.ToString();
                verifyText.Text = Setting.Instance.BarcodeVerify.ToString();
                databitText.Text = Setting.Instance.BarcodeDatabit.ToString();
                stopbitText.Text = Setting.Instance.BarcodeStopbit.ToString();
            };
            this.Unloaded += (s, e) => {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Setting.Instance.UserPassword = userPassText.Text;
            Setting.Save();
            MainWindow1.Logined = UserPermissions.Normal;
        }
        /// <summary>
        /// 保存串口信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Setting.Instance.BarcodeName = ComText.Text;

            int baud = Setting.Instance.BarcodeBaud;
            int.TryParse(baudText.Text, out baud);
            Setting.Instance.BarcodeBaud = baud;

            Parity parity = Setting.Instance.BarcodeVerify;
            Enum.TryParse(verifyText.Text, out parity);
            Setting.Instance.BarcodeVerify = parity;

            int databit = Setting.Instance.BarcodeDatabit;
            int.TryParse(databitText.Text, out databit);
            Setting.Instance.BarcodeDatabit = databit;

            StopBits stopbit = Setting.Instance.BarcodeStopbit;
            Enum.TryParse(stopbitText.Text, out stopbit);
            Setting.Instance.BarcodeStopbit = stopbit;

            Setting.Save();

        }
    }
}
