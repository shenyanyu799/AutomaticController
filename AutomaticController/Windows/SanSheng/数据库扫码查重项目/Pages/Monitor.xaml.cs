using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.SanSheng.数据库扫码查重项目.Pages
{
    /// <summary>
    /// Monitor.xaml 的交互逻辑
    /// </summary>
    public partial class Monitor : Page
    {
        static Monitor monitor;
        public Monitor()
        {
            InitializeComponent();
            monitor = this;
            ComboBox1.SelectedIndex = MainWindow1.TestCount - 1;
            ComboBox1.SelectionChanged += ComboBox_SelectionChanged;

            DataGrid1.ItemsSource = new List<UserData>();
            //添加首行序号
            DataGrid1.LoadingRow += (object sender, DataGridRowEventArgs e) => e.Row.Header = e.Row.GetIndex() + 1;

            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {

            patrType.Text = MainWindow1.Module;
            if (ComboBox1.SelectedIndex != MainWindow1.TestCount - 1)
            {
                ComboBox1.SelectedIndex = MainWindow1.TestCount - 1;
            }


            switch (MainWindow1.PartState[0])
            {
                case 0:
                    Label1.Content = "未启用";
                    Label1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB5, 0xB5, 0xB5));
                    break;
                case 1:
                    Label1.Content = "请扫码";
                    Label1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xD6, 0xFF));
                    break;
                case 2:
                    Label1.Content = "完成";
                    Label1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0xFF, 0x00));
                    break;
                case 3:
                    Label1.Content = "重码/错码";
                    Label1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
                    break;
            }
            switch (MainWindow1.PartState[1])
            {
                case 0:
                    Label2.Content = "未启用";
                    Label2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB5, 0xB5, 0xB5));
                    break;
                case 1:
                    Label2.Content = "请扫码";
                    Label2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xD6, 0xFF));
                    break;
                case 2:
                    Label2.Content = "完成";
                    Label2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0xFF, 0x00));
                    break;
                case 3:
                    Label2.Content = "重码/错码";
                    Label2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
                    break;
            }
            switch (MainWindow1.PartState[2])
            {
                case 0:
                    Label3.Content = "未启用";
                    Label3.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB5, 0xB5, 0xB5));
                    break;
                case 1:
                    Label3.Content = "请扫码";
                    Label3.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xD6, 0xFF));
                    break;
                case 2:
                    Label3.Content = "完成";
                    Label3.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x5A, 0xFF, 0x00));
                    break;
                case 3:
                    Label3.Content = "重码/错码";
                    Label3.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
                    break;
            }

            TextBox1.Text = MainWindow1.Codes[0];
            TextBox2.Text = MainWindow1.Codes[1];
            TextBox3.Text = MainWindow1.Codes[2];

            if (DataGrid1.ItemsSource != MainWindow1.SelectData)
            {
                DataGrid1.ItemsSource = MainWindow1.SelectData;
                (DataGrid1.Columns[1] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow1.TestCount = ComboBox1.SelectedIndex + 1;
        }

        public static void Add()
        {
            App.Current.Dispatcher.InvokeAsync(() => monitor.DataGrid1.ItemsSource = null);
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (DataGrid1.ItemsSource as List<UserData>).Clear();
            DataGrid1.ItemsSource = null;
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog saveFile = new Microsoft.Win32.SaveFileDialog();
            saveFile.Filter = "表格(*.csv)|*.csv";
            saveFile.Title = "导出数据";
            try
            {
                if (saveFile.ShowDialog() == true)
                {
                    using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create))
                    {
                        List<UserData> datas = (List<UserData>)DataGrid1.ItemsSource;
                        string s = "";
                        if (datas != null)
                        {
                            var ns = UserData.GetNames();
                            foreach (var item in ns)
                            {
                                s += $"\"{item}\",";
                            }
                        }
                        s = s.TrimEnd(',') + "\n";
                        byte[] bytes = Encoding.UTF8.GetBytes(s);
                        fs.Write(bytes, 0, bytes.Length);
                        foreach (var item in datas)
                        {
                            bytes = Encoding.UTF8.GetBytes(item.ToString3() + "\n");
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        fs.Close();
                        MessageBox.Show("导出完成");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }
    }
}
