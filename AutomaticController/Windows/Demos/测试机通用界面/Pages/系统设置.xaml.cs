using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    /// <summary>
    /// 系统设置.xaml 的交互逻辑
    /// </summary>
    public partial class 系统设置 : Page
    {
        public 系统设置()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;

            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
        }
        //循环呈现
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DatetimeText.Text = DateTime.Now.ToString();
        }

    }

}
