using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutomaticController.Windows.SanSheng.数据库扫码查重项目.Pages
{
    /// <summary>
    /// DataManager.xaml 的交互逻辑
    /// </summary>
    public partial class DataManager : Page
    {
        public DataManager()
        {
            InitializeComponent();

            DataGrid1.ItemsSource = new List<UserData>();
            //添加首行序号
            DataGrid1.LoadingRow += (object sender, DataGridRowEventArgs e) => e.Row.Header = e.Row.GetIndex() + 1;
            this.Loaded += (s, e) =>
            {
                startTimeText.DateTime = DateTime.Now;
                endTimeText.DateTime = DateTime.Now;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };
        }
        bool SNCodeChange = false;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            typetext.Text = MainWindow1.Module;
            bool b = MainWindow1.SNCode?.Length > 0;

            if (b == true && SNCodeChange == false)
                SNCodeText.Text = MainWindow1.SNCode;
            SNCodeChange = b;


            MainWindow1.WorkState = 0;
        }


        #region 按钮事件


        //查询状态
        bool finding = false;

        /// <summary>
        /// SN码查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            finding = true;
            DataGrid1.ItemsSource = null;
            var sd = startTimeText.DateTime;
            var ed = endTimeText.DateTime;
            string code = SNCodeText.Text;
            Task.Run(() =>
            {
                List<UserData> users = new List<UserData>();
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    var lst = lite.GetCollection<UserData>("生产记录");
                    //时间范围搜索
                    users.AddRange(lst.Find(x => x.SN码 == code));
                }
                //转换并显示
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            //users.Reverse();//反转序列
                            DataGrid1.ItemsSource = users;
                            (DataGrid1.Columns[1] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                        }
                        catch { }
                    });
                finding = false;
            });
        }
        /// <summary>
        /// 时间查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            finding = true;
            DataGrid1.ItemsSource = null;
            var sd = startTimeText.DateTime;
            var ed = endTimeText.DateTime;
            Task.Run(() =>
            {
                List<UserData> users = new List<UserData>();
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    var lst = lite.GetCollection<UserData>("生产记录");
                    //时间范围搜索
                    users.AddRange(lst.Find(x => sd <= x.扫码时间 && x.扫码时间 < ed));
                }
                //转换并显示
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            //users.Reverse();//反转序列
                            DataGrid1.ItemsSource = users;
                            (DataGrid1.Columns[1] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                        }
                        catch { }
                    });
                finding = false;
            });
        }
        /// <summary>
        /// 全部查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
            finding = true;
            DataGrid1.ItemsSource = null;
            var sd = startTimeText.DateTime;
            var ed = endTimeText.DateTime;
            Task.Run(() =>
            {
                List<UserData> users = new List<UserData>();
                users = UserData.DBFindAll();
                //转换并显示
                App.Current.Dispatcher.Invoke(
                    () =>
                    {
                        try
                        {
                            //users.Reverse();//反转序列
                            DataGrid1.ItemsSource = users;
                            (DataGrid1.Columns[1] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
                        }
                        catch { }
                    });
                finding = false;
            });
        }
        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (finding)
            {
                MessageBox.Show("正在查询...");
                return;
            }
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
        /// <summary>
        /// 删除选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid1.SelectedItems.Count == 0) return;
            if (MessageBox.Show("删除后将不可恢复，是否删除", "删除记录", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            int start = DataGrid1.SelectedIndex;
            int end = DataGrid1.SelectedItems.Count + start;
            List<UserData> userDatas = DataGrid1.ItemsSource as List<UserData>;

            try
            {
                //删除数据
                using (LiteDatabase lite = new LiteDatabase(UserData.DBPath))
                {
                    var lst = lite.GetCollection<UserData>("生产记录");

                    for (int i = start; i < end; i++)
                    {
                        lst.Delete(userDatas[i].ID);
                    }
                }
                for (int i = start; i < end; i++)
                {
                    userDatas.RemoveAt(start);
                }
                DataGrid1.ItemsSource = null;
                DataGrid1.ItemsSource = userDatas;
                (DataGrid1.Columns[1] as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd HH:mm:ss";
            }
            catch { }
        }
        #endregion


    }
}
