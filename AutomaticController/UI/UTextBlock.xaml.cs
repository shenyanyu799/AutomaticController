using AutomaticController.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.UI
{
    /// <summary>
    /// UTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class UTextBlock : TextBlock
    {
        /// <summary>
        /// 前缀
        /// </summary>
        [Localizability(LocalizationCategory.Text)]
        public string PrefixText { get; set; }
        /// <summary>
        /// 后缀
        /// </summary>
        [Localizability(LocalizationCategory.Text)]
        public string SuffixText { get; set; }
        public UTextBlock()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
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
            if (DataContext == null)
            {
                this.Text = PrefixText + this.Text + SuffixText;
            }
            else
            {
                this.Text = PrefixText + DataContext.ToString() + SuffixText;
            }

        }
    }
}
