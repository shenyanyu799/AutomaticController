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
    /// TimeSelector.xaml 的交互逻辑
    /// </summary>
    public partial class TimeSelector : UserControl
    {
        private DateTime _date;
        public DateTime DateTime
        {
            get 
            {
                DateTime dt = DateTime.Now;
                return new DateTime(dt.Year, dt.Month, dt.Day, _date.Hour, _date.Minute, 0);
            }
            set
            {
                _date = value;

                HourText.Text = _date.Hour.ToString("D2");
                MinuteText.Text = _date.Minute.ToString("D2");
            }
        }
        public TimeSelector()
        {
            InitializeComponent();
        }

        private void Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                DateTime d = DateTime;
                if (DateTime.TryParse($"{HourText.Text}:{MinuteText.Text}", out d))
                {
                    DateTime = d;
                }
                else
                {
                    DateTime = DateTime;
                }
                return;
            }
            if (e.Key == Key.Up)
            {
                if (sender == HourText)
                {
                    DateTime = _date.AddHours(1);
                    return;
                }
                if (sender == MinuteText)
                {
                    DateTime = _date.AddMinutes(1);
                    return;
                }

            }

            if (e.Key == Key.Down)
            {
                if (sender == HourText)
                {
                    DateTime = _date.AddHours(-1);
                    return;
                }
                if (sender == MinuteText)
                {
                    DateTime = _date.AddMinutes(-1);
                    return;
                }

            }
            if(sender == HourText)
            {
                if (e.Key == Key.OemSemicolon || e.Key == Key.OemPeriod)
                {
                    MinuteText.Focus();
                    e.Handled = true;
                }
                return;
            }

        }

        private void UserControl_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DateTime d = DateTime;
            if (DateTime.TryParse($"{HourText.Text}:{MinuteText.Text}", out d))
            {
                DateTime = d;
            }
            else
            {
                DateTime = DateTime;
            }
        }

        private void YearText_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {

                if (sender == HourText)
                {
                    DateTime = _date.AddHours(1);
                    return;
                }
                if (sender == MinuteText)
                {
                    DateTime = _date.AddMinutes(1);
                    return;
                }

            }

            if (e.Delta < 0)
            {

                if (sender == HourText)
                {
                    DateTime = _date.AddHours(-1);
                    return;
                }
                if (sender == MinuteText)
                {
                    DateTime = _date.AddMinutes(-1);
                    return;
                }

            }
        }
    }
}
