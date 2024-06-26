using AutomaticController.Windows.Demos.测试机通用界面.Datas;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VM.Core;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    /// <summary>
    /// 运行监控_CCD.xaml 的交互逻辑
    /// </summary>
    public partial class 运行监控_CCD : Page
    {
        //public static VmRenderControl MainVmRenderControl;
        VmProcedure vmProcedure;
        public 运行监控_CCD()
        {
            InitializeComponent();
            CCD编辑.CCDLoadedEvent += () =>
            {
                BindVmRenderControl();
            };
            //MainVmRenderControl = VmRenderControl1;
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                ComboBox_DropDownOpened(null, null);//更新参数列表

                if (VmSolution.Instance.SolutionPath == null)
                {
                    CCD编辑.LoadCCD(true);
                }
                else
                {
                    CCD编辑.LoadCCD();
                }

                //BindVmRenderControl();
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                //VmRenderControl1.ModuleSource = null;
            };
        }

        public void BindVmRenderControl()
        {
            var pro_list = VmSolution.Instance.GetAllProcedureList().astProcessInfo;
            vmProcedure = (VmProcedure)VmSolution.Instance[pro_list[0].strProcessName];
            //加载第一个流程
            if (pro_list.Length > 0)
            {
                VmRenderControl1.ModuleSource = null;
                VmRenderControl1.ModuleSource = vmProcedure;
            }
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DatetimeText.Text = DateTime.Now.ToString();
            SNCodeTextbox.Text = MainWindow.SNCode;
            switch (MainWindow.SNCodeResult)
            {
                case SNCodeState.None:
                    SNCodeStateLabel.Content = "";
                    SNCodeStateLabel.Background = null;
                    break;
                case SNCodeState.Pass:
                    SNCodeStateLabel.Content = "通过";
                    SNCodeStateLabel.Background = new SolidColorBrush(Color.FromRgb(0x51, 0xd7, 0x2b));
                    break;
                case SNCodeState.Error:
                    SNCodeStateLabel.Content = "错码";
                    SNCodeStateLabel.Background = new SolidColorBrush(Color.FromRgb(0xd7, 0x2b, 0x51));
                    break;
                case SNCodeState.Repetition:
                    SNCodeStateLabel.Content = "重码";
                    SNCodeStateLabel.Background = new SolidColorBrush(Color.FromRgb(0xd7, 0x2b, 0x51));
                    break;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string con = button.Content.ToString();
            if (con == "刷新")
            {
                CCD编辑.LoadCCD(true);
                //BindVmRenderControl();
            }
            if (con == "执行")
            {
                CCD编辑.ExecuteCCD();
            }
            if (con == "编辑")
            {
                MainWindow.Pages.Next("CCD编辑");
            }
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
