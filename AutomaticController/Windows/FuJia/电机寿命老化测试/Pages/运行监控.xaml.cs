using AutomaticController.Windows.FuJia.电机寿命老化测试.Datas;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Pages
{
    /// <summary>
    /// 运行监控.xaml 的交互逻辑
    /// </summary>
    public partial class 运行监控 : Page
    {
        public 运行监控()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                stackPanel.Children.Add(createPart(i));
                stackPane2.Children.Add(createPart(10 + i));
                stackPane3.Children.Add(createPart(20 + i));
            }

            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;

            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };

        }
        //画面渲染
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DatetimeText.Text = DateTime.Now.ToString();
            cpdatePart?.Invoke();
            V1Text.Text = "工作电压:" + Devices.采集电压1.ToString("F2") + "(V)";
            V2Text.Text = "工作电压:" + Devices.采集电压2.ToString("F2") + "(V)";
            V3Text.Text = "工作电压:" + Devices.采集电压3.ToString("F2") + "(V)";
        }



        private event Action cpdatePart;
        private Border createPart(int num)
        {
            StackPanel stack = new StackPanel() { Margin = new Thickness(3) };
            Border border = new Border() { Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0xE6, 0xE6)), BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAB, 0xAB, 0xAB)), Child = stack };
            stack.Children.Add(new TextBlock() { Text = $"工位{(num + 1).ToString("D2")}", FontSize = 14 });

            StackPanel stack2 = new StackPanel() { Orientation = Orientation.Horizontal };
            stack.Children.Add(stack2);
            stack2.Children.Add(new TextBlock() { Text = "测试规则:", FontSize = 14 });
            ComboBox combo = new ComboBox() { Tag = num, Width = 60, };
            combo.Loaded += ComboBox_Loaded;
            combo.SelectionChanged += ComboBox_SelectionChanged;
            stack2.Children.Add(combo);

            TextBlock t1 = new TextBlock() { Text = "实时电流:0A", FontSize = 14 };
            TextBlock t1_1 = new TextBlock() { Text = "平均电流:0A", FontSize = 14 };
            TextBlock t1_2 = new TextBlock() { Text = "最大电流:0A", FontSize = 14 };
            TextBlock t1_3 = new TextBlock() { Text = "最小电流:0A", FontSize = 14 };
            TextBlock t2 = new TextBlock() { Text = "运行时间:0s", FontSize = 14 };

            TextBlock t3 = new TextBlock() { Text = "当前次数:0", FontSize = 14 };
            TextBlock t4 = new TextBlock() { Text = "目标次数:0", FontSize = 14 };
            TextBlock t5 = new TextBlock() { Text = "测试状态", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Border border2 = new Border() { Child = t5, Height = 26 };
            stack.Children.Add(t1);
            stack.Children.Add(t1_1);
            stack.Children.Add(t1_2);
            stack.Children.Add(t1_3);
            stack.Children.Add(t2);
            stack.Children.Add(t3);
            stack.Children.Add(t4);
            stack.Children.Add(border2);

            Brush yellow = new SolidColorBrush(Color.FromRgb(0xff, 0xeb, 0x3b));
            Brush red = new SolidColorBrush(Colors.Red);
            Brush blue = new SolidColorBrush(Color.FromRgb(0x56, 0xd4, 0xf0));
            Brush green = new SolidColorBrush(Colors.LawnGreen);
            cpdatePart += () =>
            {
                RunningData runningData = RunningData.Get(num);

                t1.Text = $"实时电流:{RunningState.States[num].ElectriCurrent.ToString("F2")}A";
                t1_1.Text = $"平均电流:{RunningState.States[num].AverageCurrent.ToString("F2")}A";
                t1_2.Text = $"最大电流:{RunningState.States[num].MaxCurrent.ToString("F2")}A";
                t1_3.Text = $"最小电流:{RunningState.States[num].MinCurrent.ToString("F2")}A";
                t2.Text = $"运行时间:{RunningState.States[num].Runtime_s}s";
                //t2.Text = $"运行时间:{runningData.Runtime}s";
                t3.Text = $"运行次数:{runningData.TestCount}";
                if (runningData.Name != null)
                    t4.Text = $"目标次数:{Parameters_XMLFile.Instance[runningData.Name].TotalCount}";
                string state = "";
                int s = RunningState.States[num].State;
                switch (s)
                {
                    case 0:
                        state = "待机中";
                        border2.Background = yellow;
                        break;
                    case 1:
                        state = "准备启动";
                        border2.Background = yellow;
                        break;
                    case 2:
                        state = "运行异常";
                        border2.Background = red;
                        break;
                    case 3:
                        state = "测试完成";
                        border2.Background = green;
                        break;
                    case 4:
                        state = "请选择测试规则";
                        border2.Background = red;
                        break;
                    case 5:
                        state = "暂停中";
                        border2.Background = yellow;
                        break;
                    case 10:
                        state = "测试运行中";
                        border2.Background = blue;
                        break;
                    case 11:
                        state = "测试等待中";
                        border2.Background = blue;
                        break;
                }
                t5.Text = state;
            };
            Grid grid = new Grid() { Height = 28, Margin = new Thickness(0, 2, 0, 2) };
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            Button button1 = new Button() { Content = "启动", Background = new SolidColorBrush(Color.FromRgb(0x3E, 0xB1, 0x5A)) };
            Button button2 = new Button() { Content = "停止", Background = new SolidColorBrush(Color.FromRgb(0xB1, 0x4E, 0x3E)) };
            Button button3 = new Button() { Height = 26, Margin = new Thickness(0, 2, 0, 2), Content = "清除记录", Background = new SolidColorBrush(Color.FromRgb(0x9A, 0xA7, 0xB1)) };
            grid.Children.Add(button1);
            grid.Children.Add(button2);
            button1.SetValue(Grid.ColumnProperty, 0);
            button2.SetValue(Grid.ColumnProperty, 1);
            var tm = MainWindow.Window.TryFindResource("ButtonTemplate2");
            button1.Template = (ControlTemplate)tm;
            button2.Template = (ControlTemplate)tm;
            button3.Template = (ControlTemplate)tm;
            stack.Children.Add(grid);
            stack.Children.Add(button3);

            button1.Click += (s, e) =>
            {
                RunningState.States[num].RequestStart = true;
            };
            button2.Click += (s, e) =>
            {
                RunningState.States[num].RequestStop = true;
            };
            button3.Click += (s, e) =>
            {
                var data = RunningData.Get(num);
                //data.Runtime = 0;
                data.TestCount = 0;
                //data.Tag = 0;
                //RunningState.States[num].StartTime = DateTime.Now;
                RunningData.Set(num, data);
            };

            return border;
        }
        //启动停止按钮事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string param = button.CommandParameter.ToString();
            int n = 0;
            bool start = false;
            switch (param)
            {
                case "AStart":
                    n = 0;
                    start = true;
                    break;
                case "AStop":
                    n = 0;
                    start = false;
                    break;
                case "BStart":
                    n = 10;
                    start = true;
                    break;
                case "BStop":
                    n = 10;
                    start = false;
                    break;
                case "CStart":
                    n = 20;
                    start = true;
                    break;
                case "CStop":
                    n = 20;
                    start = false;
                    break;
            }
            int len = n + 10;
            for (int i = n; i < len; i++)
            {
                if (start)
                {
                    RunningState.States[i].RequestStart = true;
                }
                else
                {
                    RunningState.States[i].RequestStop = true;
                }
            }
        }


        bool ComboBox_Loading = false;
        /// <summary>
        /// 更新下拉列表参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_Loading = true;
            ComboBox comboBox = sender as ComboBox;

            int n = int.Parse(comboBox.Tag.ToString());
            var sv = RunningData.Get(n);
            comboBox.Items.Clear();
            comboBox.Items.Add(null);
            string[] names = Parameters_XMLFile.Instance.GetNames();
            ComboBoxItem selename = null;
            foreach (string name in names)
            {
                var p = Parameters_XMLFile.Instance[name];
                var com = new ComboBoxItem() { Content = p.Name, Tag = name };
                comboBox.Items.Add(com);
                if (sv.Name == name)
                {
                    selename = com;
                }
            }
            if (selename == null)
            {
                comboBox.SelectedIndex = 0;
            }
            else
            {
                comboBox.SelectedItem = selename;
            }
            ComboBox_Loading = false;
        }
        /// <summary>
        /// 选择下拉列表参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_Loading) return;
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem is ComboBoxItem)
            {
                var com = comboBox.SelectedItem as ComboBoxItem;
                int n = int.Parse(comboBox.Tag.ToString());
                var sv = RunningData.Get(n);
                sv.Name = com.Tag.ToString();
                RunningData.Set(n, sv);
            }
        }


    }
}
