using AutomaticController.Windows.FuJia.电机寿命老化测试.Datas;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Pages
{
    /// <summary>
    /// 参数设置.xaml 的交互逻辑
    /// </summary>
    public partial class 参数设置 : Page
    {
        public 参数设置()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                Load();
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DatetimeText.Text = DateTime.Now.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string pargm = button.CommandParameter.ToString();
            if (pargm == "Create")
            {
                Create();
            }
            if (pargm == "Delete")
            {
                if (parmsList.SelectedItem != null)
                {
                    int sindex = parmsList.SelectedIndex;
                    var listitem = (parmsList.SelectedItem as ListBoxItem);
                    if (listitem == null) return;
                    Delete(listitem.Tag.ToString());
                    parmsList.Items.RemoveAt(sindex);
                    if (parmsList.Items.Count > sindex)
                    {
                        parmsList.SelectedIndex = sindex;
                    }
                    parmsList.Focus();
                }
            }
            if (pargm == "Editing")
            {
                int sindex = parmsList.SelectedIndex;
                var listitem = (parmsList.SelectedItem as ListBoxItem);
                if (listitem == null) return;
                Editing(listitem.Tag.ToString());
            }
        }
        public void Load()
        {
            parmsList.Items.Clear();
            string[] names = Parameters_XMLFile.Instance.GetNames();
            foreach (var name in names)
            {
                var ps = Parameters_XMLFile.Instance[name];
                parmsList.Items.Add(new ListBoxItem() { Content = ps.Name, Tag = name });
            }
        }
        public void Create()
        {
            string[] names = Parameters_XMLFile.Instance.GetNames();
            int i = 0;
            string setName = "";
            while (true)
            {
                i++;
                bool o = false;
                foreach (var name in names)
                {
                    if (name == "parms" + i)
                    {
                        o = true;
                        break;
                    }
                }
                if (o == false)
                {
                    setName = "parms" + i;
                    break;
                }
            }
            Parameters_XMLFile.Instance[setName] = new Parameters(setName);
            var listitem = new ListBoxItem() { Content = setName, Tag = setName };
            parmsList.Items.Add(listitem);
            parmsList.SelectedItem = listitem;
            parmsList.Focus();
        }
        public void Editing(string name)
        {
            var par = Parameters_XMLFile.Instance[name];
            parmsStackPanel.Children.Clear();
            parmsGrid.Visibility = Visibility.Visible;
            parmsList.IsEnabled = false;
            buttonsStackPanel.IsEnabled = false;
            //创建参数名UI
            StackPanel stackPanel1 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock1 = new TextBlock() { Text = "参数名：", FontSize = 25 };
            TextBox textBox1 = new TextBox() { Text = par.Name, FontSize = 25, Width = 200 };
            stackPanel1.Children.Add(textBlock1);
            stackPanel1.Children.Add(textBox1);

            //创建测试次数
            StackPanel stackPanel2 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock2 = new TextBlock() { Text = "循环次数：", FontSize = 25 };
            TextBox textBox2 = new TextBox() { Text = par.TotalCount.ToString(), FontSize = 25, Width = 200 };
            stackPanel2.Children.Add(textBlock2);
            stackPanel2.Children.Add(textBox2);


            //创建开启时间
            StackPanel stackPanel3 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock3 = new TextBlock() { Text = "开启时长(s)：", FontSize = 25 };
            TextBox textBox3 = new TextBox() { Text = par.PowerOnTime.ToString(), FontSize = 25, Width = 200 };
            stackPanel3.Children.Add(textBlock3);
            stackPanel3.Children.Add(textBox3);


            //创建关闭时间
            StackPanel stackPanel4 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock4 = new TextBlock() { Text = "关闭时长(s)：", FontSize = 25 };
            TextBox textBox4 = new TextBox() { Text = par.PowerOffTime.ToString(), FontSize = 25, Width = 200 };
            stackPanel4.Children.Add(textBlock4);
            stackPanel4.Children.Add(textBox4);


            //创建关闭时间
            StackPanel stackPanel5 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock5 = new TextBlock() { Text = "检测延时(s)：", FontSize = 25 };
            TextBox textBox5 = new TextBox() { Text = par.AlarmDelay.ToString(), FontSize = 25, Width = 200 };
            stackPanel5.Children.Add(textBlock5);
            stackPanel5.Children.Add(textBox5);


            //创建关闭时间
            StackPanel stackPanel6 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock6 = new TextBlock() { Text = "电流上限(A)：", FontSize = 25 };
            TextBox textBox6 = new TextBox() { Text = par.ElectriCurrentUpper.ToString(), FontSize = 25, Width = 200 };
            stackPanel6.Children.Add(textBlock6);
            stackPanel6.Children.Add(textBox6);


            //创建关闭时间
            StackPanel stackPanel7 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(1, 1, 1, 1) };
            TextBlock textBlock7 = new TextBlock() { Text = "电流下限(A)：", FontSize = 25 };
            TextBox textBox7 = new TextBox() { Text = par.ElectriCurrentLower.ToString(), FontSize = 25, Width = 200 };
            stackPanel7.Children.Add(textBlock7);
            stackPanel7.Children.Add(textBox7);


            //创建按钮
            StackPanel stackPanel_Button = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(1, 1, 1, 1) };
            Button button1 = new Button() { Width = 120, Height = 60, FontSize = 25, Content = "确定", Margin = new Thickness(10, 10, 10, 10) };
            Button button2 = new Button() { Width = 120, Height = 60, FontSize = 25, Content = "取消", Margin = new Thickness(10, 10, 10, 10) };
            button1.Click += (s, e) =>
            {
                int c = par.TotalCount;
                double onT = par.PowerOnTime;
                double offT = par.PowerOffTime;
                double aT = par.AlarmDelay;
                double cu = par.ElectriCurrentUpper;
                double cl = par.ElectriCurrentLower;
                //判断并转换
                if (int.TryParse(textBox2.Text, out c) == false)
                {
                    textBox2.Text = par.TotalCount.ToString();
                    return;
                }
                if (double.TryParse(textBox3.Text, out onT) == false)
                {
                    textBox3.Text = par.PowerOnTime.ToString();
                    return;
                }
                if (double.TryParse(textBox4.Text, out offT) == false)
                {
                    textBox4.Text = par.PowerOffTime.ToString();
                    return;
                }
                if (double.TryParse(textBox5.Text, out aT) == false)
                {
                    textBox5.Text = par.AlarmDelay.ToString();
                    return;
                }
                if (double.TryParse(textBox6.Text, out cu) == false)
                {
                    textBox6.Text = par.ElectriCurrentUpper.ToString();
                    return;
                }
                if (double.TryParse(textBox7.Text, out cl) == false)
                {
                    textBox7.Text = par.ElectriCurrentLower.ToString();
                    return;
                }


                Parameters_XMLFile.Instance[name] = new Parameters()
                {
                    Name = textBox1.Text,
                    TotalCount = c,
                    PowerOnTime = onT,
                    PowerOffTime = offT,
                    AlarmDelay = aT,
                    ElectriCurrentUpper = cu,
                    ElectriCurrentLower = cl,
                };
                parmsGrid.Visibility = Visibility.Hidden;
                parmsList.IsEnabled = true;
                buttonsStackPanel.IsEnabled = true;
                (parmsList.SelectedItem as ListBoxItem).Content = textBox1.Text;
            };
            button2.Click += (s, e) =>
            {
                parmsGrid.Visibility = Visibility.Hidden;
                parmsList.IsEnabled = true;
                buttonsStackPanel.IsEnabled = true;
            };
            stackPanel_Button.Children.Add(button1);
            stackPanel_Button.Children.Add(button2);


            //加入窗口
            parmsStackPanel.Children.Add(stackPanel1);
            parmsStackPanel.Children.Add(stackPanel2);
            parmsStackPanel.Children.Add(stackPanel3);
            parmsStackPanel.Children.Add(stackPanel4);
            parmsStackPanel.Children.Add(stackPanel5);
            parmsStackPanel.Children.Add(stackPanel6);
            parmsStackPanel.Children.Add(stackPanel7);
            parmsStackPanel.Children.Add(stackPanel_Button);
        }
        public void Delete(string name)
        {
            Parameters_XMLFile.Instance.Remove(name);
        }

    }
}
