using AutomaticController.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutomaticController.UI
{
    /// <summary>
    /// UBitButton.xaml 的交互逻辑
    /// </summary>
    public partial class UBitButton : Button
    {
        public CornerRadius CornerRadius
        {
            get =>
                (CornerRadius)Resources["BorderCornerRadius"];
            set =>
                Resources["BorderCornerRadius"] = value;
        }
        public Brush OffBackground { get; set; }
        public Brush OnBackground { get; set; }
        public object OffContent { get; set; }
        public object OnContent { get; set; }
        public PressModule PressModule { get; set; }
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
                    Content = OnContent;
                }
                else
                {
                    Background = OffBackground;
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
                    Content = OnContent;
                }
                else
                {
                    Background = OffBackground;
                    Content = OffContent;
                }
            }
        }
        private bool bit;
        public UBitButton()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
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
            if (isDown && this.IsMouseOver == false)//鼠标离开后算抬起
            {
                Up();
            }
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
                    Content = OnContent;
                }
                else
                {
                    Background = OffBackground;
                    Content = OffContent;
                }
            }

        }

        bool isDown = false;
        private void Down()
        {
            if (isDown) return;
            isDown = true;
            switch (PressModule)
            {
                case PressModule.Set:
                case PressModule.PressOn:
                    Value = true; break;
                case PressModule.Reset:
                case PressModule.PressOff:
                    Value = false; break;
                case PressModule.Invert:
                    Value = !Value;
                    break;
            }

        }
        private void Up()
        {
            switch (PressModule)
            {
                case PressModule.PressOn:
                    Value = false; break;
                case PressModule.PressOff:
                    Value = true; break;
            }
            isDown = false;
        }
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Down();
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                Up();
            }
        }
        private void Button_TouchDown(object sender, TouchEventArgs e)
        {
            Down();
        }

    }
    public enum PressModule
    {
        Set,
        Reset,
        PressOn,
        PressOff,
        Invert
    }
}
