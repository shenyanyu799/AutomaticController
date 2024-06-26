using AutomaticController.Windows.Demos.测试机通用界面.Datas;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
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
                LoadParamList();

            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                Parameters_XMLFile.Instance.SaveParam();
            };
            //加载参数,在这里载入仅存储在本地的数据
            Parameters_XMLFile.Instance.LoadParamEvent += param =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    PageTitle.Text = $"参数设置【{param.FileName}】";
                    //初始化显示
                    VmPathText.Text = param.CCDPath;
                    sncodePrefixText.Text = param.SN码前缀;
                    sncodeLengthText.Text = param.SN码长度.ToString();
                    sncodeDuplicateButton.Value = param.重码检测;
                    sncodeLengthButton.Value = param.长度检测;

                    //更新参数后重置列表选择项
                    for (int i = 0; i < paramListView.Items.Count; i++)
                    {
                        if ((paramListView.Items[i] as ListViewItem).Content.ToString() == param.FileName)
                        {
                            paramListView.SelectedIndex = i;
                            paramSelectedIndex = i;
                        }
                    }
                });

            };
            //保存参数,在这里保存仅存储在本地的数据
            Parameters_XMLFile.Instance.SaveParamEvent += param =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    param.CCDPath = VmPathText.Text;
                    param.SN码前缀 = sncodePrefixText.Text;
                    param.重码检测 = sncodeDuplicateButton.Value;
                    param.长度检测 = sncodeLengthButton.Value;
                    if(int.TryParse(sncodeLengthText.Text, out int len))
                    {
                        param.SN码长度 = len;
                    }
                });
            };
        }

        //当前选择的参数的索引
        int paramSelectedIndex = 0;
        //更新参数列表
        public void LoadParamList()
        {
            //显示参数列表
            var names = Parameters_XMLFile.Instance.GetNames();
            paramListView.Items.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                var viewitem = new ListViewItem() { Content = names[i] };
                viewitem.Selected += (_s, _e) =>
                {
                    renameParamButton.IsEnabled = true;
                    deleteParamButton.IsEnabled = true;
                    if (paramListView.Tag != null) return;
                    if (viewitem.Content.ToString() != Parameters_XMLFile.SelectItem?.FileName)
                    {
                        if (MessageBox.Show($"是否选择【{viewitem.Content}】当前参数", "选择", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            Parameters_XMLFile.Select(viewitem.Content.ToString());
                        }
                        else
                        {
                            Task.Delay(10).ContinueWith(t => App.Current.Dispatcher.Invoke(() =>
                            {
                                if (paramListView.SelectedIndex != paramSelectedIndex)
                                    paramListView.SelectedIndex = paramSelectedIndex;
                            }));
                        }
                    }
                };
                viewitem.LostFocus += (_s, _e) =>
                {
                    if (paramListView.SelectedItem == null)
                    {
                        renameParamButton.IsEnabled = false;
                        deleteParamButton.IsEnabled = false;
                    }
                };
                paramListView.Items.Add(viewitem);
            }

            Parameters_XMLFile.Instance.LoadParam();
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DatetimeText.Text = DateTime.Now.ToString();
        }
        /// <summary>
        /// VM CCDOpenFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            //openFile.FileName = (sender as UTextBox).SourceText;
            openFile.Filter = "(VM程序)|*.sol";
            openFile.Title = "选择VM程序";
            openFile.FileOk += (object _sender, System.ComponentModel.CancelEventArgs _e) =>
            {
                var par = Parameters_XMLFile.SelectItem;
                VmPathText.Text = openFile.FileName;
                if (par?.CCDPath != openFile.FileName)
                {
                    par.CCDPath = openFile.FileName;
                }
            };
            openFile.ShowDialog();
        }
        //参数列表按钮
        private void ParamButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string content = button.CommandParameter.ToString();
            if (content == "save")
            {
                Parameters_XMLFile.Instance.SaveParam();
                return;
            }
            if (content == "new")
            {
                var parm = Parameters_XMLFile.NewParameters();
                Parameters_XMLFile.Instance.SaveParam();
                LoadParamList();
                return;
            }
            if (content == "rename")
            {
                paramListView.Tag = true;
                ListViewItem item = (ListViewItem)paramListView.SelectedItem;
                int index = paramListView.SelectedIndex;
                paramListView.Items.RemoveAt(index);
                TextBox textBox = new TextBox();
                //回车取消后保存重命名
                textBox.KeyDown += (s, _e) =>
                {
                    if (_e.Key == Key.Enter || _e.Key == Key.Escape)
                    {
                        textBox.Focusable = false;
                    }
                };
                //失去焦点后保存重命名
                textBox.LostKeyboardFocus += (s, _e) =>
                {

                    var param = Parameters_XMLFile.SelectItem;
                    if (param.Rename(textBox.Text))
                    {
                        item.Content = textBox.Text;
                        Parameters_XMLFile.Select(textBox.Text);
                    }
                    paramListView.Items.RemoveAt(index);
                    paramListView.Items.Insert(index, item);
                    Task.Delay(500).ContinueWith(t => App.Current.Dispatcher.Invoke(() =>
                    {
                        paramListView.Tag = null;
                        LoadParamList();
                    }));

                };
                //加载后文本框自动获得焦点并选中全部内容
                textBox.Loaded += (s, _e) =>
                {
                    textBox.Focus();
                    textBox.SelectAll();
                };
                textBox.Width = paramListView.ActualWidth - 10;

                textBox.Text = item.Content.ToString();
                paramListView.Items.Insert(index, textBox);

                return;
            }
            if (content == "delete")
            {
                Parameters_XMLFile.SelectItem.Delete();
                Parameters_XMLFile.SelectFirst();
                LoadParamList();
                return;
            }
        }


    }
}
