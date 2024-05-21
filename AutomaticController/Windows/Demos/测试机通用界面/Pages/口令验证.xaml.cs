using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    /// <summary>
    /// 口令验证.xaml 的交互逻辑
    /// </summary>
    public partial class 口令验证 : KeyPage
    {
        public 口令验证()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string password = keyText.Password;
            keyText.Password = "";
            base.Verify?.Invoke(password);
        }
        //取消按钮
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            base.Cancel?.Invoke();
        }
    }
}
