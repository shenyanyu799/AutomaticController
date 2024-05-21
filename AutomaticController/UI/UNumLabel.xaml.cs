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
using AutomaticController.Device;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace AutomaticController.UI
{
    /// <summary>
    /// UNumLabel.xaml 的交互逻辑
    /// </summary>
    public partial class UNumLabel : Label
    {
        public Brush[] Backgrounds { get; set; }
        public Brush[] BorderBrushs { get; set; }
        public Brush[] Foregrounds { get; set; }
        public object[] Contents { get; set; }
        private int index;
        public int Index
        {
            get
            {
                if (DataContext is INum)
                {
                    index = (int)(DataContext as INum).Value;
                }
                if (index < 0) return index;
                if (Backgrounds?.Length > index)
                {
                    Background = Backgrounds[index];
                }
                if (BorderBrushs?.Length > index)
                {
                    BorderBrush = BorderBrushs[index];
                }
                if (Foregrounds?.Length > index)
                {
                    Foreground = Foregrounds[index];
                }
                if (Contents?.Length > index)
                {
                    Content = Contents[index];
                }
                return index;
            }
            set
            {
                if (Backgrounds?.Length > value)
                {
                    Background = Backgrounds[value];
                }
                if (BorderBrushs?.Length > value)
                {
                    BorderBrush = BorderBrushs[value];
                }
                if (Foregrounds?.Length > value)
                {
                    Foreground = Foregrounds[value];
                }
                if (Contents?.Length > value)
                {
                    Content = Contents[value];
                }
                index = value;
                if (DataContext is INum)
                {
                    (DataContext as INum).Value = value;
                }
            }
        }
        public UNumLabel()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                int i = Index;
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
            if (this.IsVisible == false) return;

            if (DataContext is IModbus_RTU_Unit)//读取PLC数据
            {
                (DataContext as IModbus_RTU_Unit).RequestRead = true;
            }
            int n = index;
            if (DataContext is INum)
            {
                n = (int)(DataContext as INum).Value;
            }
            if (n < 0) return;
            if(n != index)
            {
                index = n;
                if (Backgrounds?.Length > index)
                {
                    Background = Backgrounds[index];
                }
                if (BorderBrushs?.Length > index)
                {
                    BorderBrush = BorderBrushs[index];
                }
                if (Foregrounds?.Length > index)
                {
                    Foreground = Foregrounds[index];
                }
                if (Contents?.Length > index)
                {
                    Content = Contents[index];
                }
            }
        }
    }
}
