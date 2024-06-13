using AutomaticController.Windows.FuJia.电机寿命老化测试.Datas;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Pages
{
    /// <summary>
    /// 系统设置.xaml 的交互逻辑
    /// </summary>
    public partial class 系统设置 : Page
    {
        public 系统设置()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
 
            };
            this.Unloaded += (s, e) => {
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
