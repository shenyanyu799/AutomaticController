using AutomaticController.Device;
using Newtonsoft.Json.Linq;
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

namespace AutomaticController.UI
{
    /// <summary>
    /// UComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class UComboBox : ComboBox
    {
        public UComboBox()
        {
            InitializeComponent();
            UpdateDataContext();
            CompositionTarget_Rendering(this, null);

            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                this.SelectionChanged += UComboBox_SelectionChanged;
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                this.SelectionChanged -= UComboBox_SelectionChanged;
            };
        }
        private void UComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is INum)
            {
                (DataContext as INum).Value = SelectedIndex;
            }
            if (DataContext is IBit)
            {
                (DataContext as IBit).Value = (SelectedIndex == 1);
            }
        }
        /// <summary>
        /// 用于显示状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (this.IsVisible == false) return;
            UpdateDataContext();
        }
        private void UpdateDataContext()
        {
            if (DataContext is IModbus_RTU_Unit)//读取PLC数据
            {
                var obj = (DataContext as IModbus_RTU_Unit);
                obj.RequestRead = true;
                if (DataContext is INum)
                {
                    SelectedIndex = (int)(DataContext as INum).Value;
                }
                if (DataContext is IBit)
                {
                    bool v = (bool)(DataContext as IBit).Value;
                    if (v)
                    {
                        SelectedIndex = 1;
                    }
                    else
                    {
                        SelectedIndex = 0;
                    }
                }
            }

        }
    }
}
