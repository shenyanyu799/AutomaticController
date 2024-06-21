using AutomaticController.Windows.Demos.测试机通用界面.Datas;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    /// <summary>
    /// 运行监控.xaml 的交互逻辑
    /// </summary>
    public partial class 运行监控 : Page
    {
        public 运行监控()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                ComboBox_DropDownOpened(null, null);//更新参数列表
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
        //打开参数选择列表
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            string sele = Parameters_XMLFile.SelectItem?.FileName;
            ParamSelest_ComboBox.Items.Clear();
            string[] names = Parameters_XMLFile.Instance.GetNames();
            foreach (string name in names)
            {
                ParamSelest_ComboBox.Items.Add(name);
            }
            ParamSelest_ComboBox.SelectedItem = sele;
        }
        //关闭参数选择列表
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            string sele = ParamSelest_ComboBox.SelectedItem.ToString();
            Parameters_XMLFile.Select(sele);
        }
    }
}
