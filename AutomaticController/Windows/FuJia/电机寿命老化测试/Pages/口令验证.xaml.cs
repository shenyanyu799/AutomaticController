using System.Windows;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Pages
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
