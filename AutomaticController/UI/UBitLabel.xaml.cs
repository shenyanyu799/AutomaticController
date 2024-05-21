using AutomaticController.Device;
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
    /// UBitLabel.xaml 的交互逻辑
    /// </summary>
    public partial class UBitLabel : Label
    {
        public Brush OffBackground { get; set; }
        public Brush OnBackground { get; set; }
        public Brush OffBorderBrush { get; set; }
        public Brush OnBorderBrush { get; set; }
        public Brush OffForeground { get; set; }
        public Brush OnForeground { get; set; }
        public object OffContent { get; set; }
        public object OnContent { get; set; }
        public bool Value
        {
            get
            {
                if (DataContext is IBit)
                {
                    bit = (DataContext as IBit).Value;
                }
                if (bit)
                {
                    Background = OnBackground;
                    Foreground = OnForeground;
                    BorderBrush = OnBorderBrush;
                    Content = OnContent;
                }
                else
                {
                    Background = OffBackground;
                    Foreground = OffForeground;
                    BorderBrush = OffBorderBrush;
                    Content = OffContent;
                }

                return bit;
            }
            set
            {
                bit = value;
                if (DataContext is IBit)
                {
                    (DataContext as IBit).Value = value;
                }
                if (bit)
                {
                    Background = OnBackground;
                    Foreground = OnForeground;
                    BorderBrush = OnBorderBrush;
                    Content = OnContent;
                }
                else
                {
                    Background = OffBackground;
                    Foreground = OffForeground;
                    BorderBrush = OffBorderBrush;
                    Content = OffContent;
                }
            }
        }
        private bool bit;
        public UBitLabel()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                bool b = Value;
            };
            this.Unloaded += (s, e) => CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
        /// <summary>
        /// 用于显示状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (DataContext is IModbus_RTU_Unit)//读取PLC数据
            {
                var obj = (DataContext as IModbus_RTU_Unit);
                obj.RequestRead = true;
            }
            bool b = bit;
            if (DataContext is IBit)
            {
                b = (DataContext as IBit).Value;
            }
            if (b != bit)
            {
                bit = b;
                if (bit)
                {
                    Background = OnBackground;
                    Foreground = OnForeground;
                    BorderBrush = OnBorderBrush;
                    Content = OnContent;
                }
                else
                {
                    Background = OffBackground;
                    Foreground = OffForeground;
                    BorderBrush = OffBorderBrush;
                    Content = OffContent;
                }
            }

        }

    }
}
