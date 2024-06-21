using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutomaticController.UI
{
    /// <summary>
    /// DateTimeSelector.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeSelector : UserControl
    {
        private DateTime _date;
        public DateTime DateTime
        {
            get => _date; set
            {
                _date = value;
                YearText.Text = _date.Year.ToString("D4");
                MonthText.Text = _date.Month.ToString("D2");
                DayText.Text = _date.Day.ToString("D2");

                HourText.Text = _date.Hour.ToString("D2");
                MinuteText.Text = _date.Minute.ToString("D2");
                SecondText.Text = _date.Second.ToString("D2");

            }
        }
        public DateTimeSelector()
        {
            InitializeComponent();
        }


        private void Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                DateTime d = DateTime;
                if (DateTime.TryParse($"{YearText.Text}/{MonthText.Text}/{DayText.Text} {HourText.Text}:{MinuteText.Text}:{SecondText.Text}", out d))
                {
                    DateTime = d;
                }
                else
                {
                    DateTime = DateTime;
                }
            }
            if (e.Key == Key.Up)
            {
                if (sender == YearText)
                {
                    DateTime = _date.AddYears(1);
                    return;
                }
                if (sender == MonthText)
                {
                    DateTime = _date.AddMonths(1);
                    return;
                }
                if (sender == DayText)
                {
                    DateTime = _date.AddDays(1);
                    return;
                }
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
                if (sender == SecondText)
                {
                    DateTime = _date.AddSeconds(1);
                    return;
                }
            }

            if (e.Key == Key.Down)
            {
                if (sender == YearText)
                {
                    DateTime = _date.AddYears(-1);
                    return;
                }
                if (sender == MonthText)
                {
                    DateTime = _date.AddMonths(-1);
                    return;
                }
                if (sender == DayText)
                {
                    DateTime = _date.AddDays(-1);
                    return;
                }
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
                if (sender == SecondText)
                {
                    DateTime = _date.AddSeconds(-1);
                    return;
                }
            }

        }

        private void UserControl_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DateTime d = DateTime;
            if (DateTime.TryParse($"{YearText.Text}/{MonthText.Text}/{DayText.Text} {HourText.Text}:{MinuteText.Text}:{SecondText.Text}", out d))
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
                if (sender == YearText)
                {
                    DateTime = _date.AddYears(1);
                    return;
                }
                if (sender == MonthText)
                {
                    DateTime = _date.AddMonths(1);
                    return;
                }
                if (sender == DayText)
                {
                    DateTime = _date.AddDays(1);
                    return;
                }
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
                if (sender == SecondText)
                {
                    DateTime = _date.AddSeconds(1);
                    return;
                }
            }

            if (e.Delta < 0)
            {
                if (sender == YearText)
                {
                    DateTime = _date.AddYears(-1);
                    return;
                }
                if (sender == MonthText)
                {
                    DateTime = _date.AddMonths(-1);
                    return;
                }
                if (sender == DayText)
                {
                    DateTime = _date.AddDays(-1);
                    return;
                }
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
                if (sender == SecondText)
                {
                    DateTime = _date.AddSeconds(-1);
                    return;
                }
            }
        }
    }
}
