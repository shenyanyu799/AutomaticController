using AutomaticController.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutomaticController.UI
{
    /// <summary>
    /// UTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class UTextBox : TextBox
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
        /// <summary>
        /// 正在输入内容
        /// </summary>
        public bool Writeing { get; set; }

        public UTextBox()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            };
            this.Unloaded += (s, e) => CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
        bool keyFocused;
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
                var obj = (DataContext as IModbus_RTU_Unit);
                obj.RequestRead = true;
            }
            bool kbf = this.IsKeyboardFocused | Writeing;
            //获得焦点
            if (keyFocused == false && kbf == true)
            {
                this.Text = DataContext?.ToString();
                this.SelectAll();
            }
            //失去焦点
            if (keyFocused == true && kbf == false)
            {
                if (DataContext == null || DataContext is string)
                {
                    DataContext = this.Text;
                }
                if (DataContext is ITryText)
                {
                    (DataContext as ITryText).Parse(this.Text); //写入数据
                }
            }
            if (kbf == false)
            {
                this.Text = PrefixText + DataContext?.ToString() + SuffixText;
            }
            keyFocused = kbf;
        }
        /// <summary>
        /// 敲下回车更新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

    }
}
