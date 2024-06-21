using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Pages
{
    /// <summary>
    /// IO监控.xaml 的交互逻辑
    /// </summary>
    public partial class IO监控 : Page
    {
        public IO监控()
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
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DatetimeText.Text = DateTime.Now.ToString();
        }
    }
}
