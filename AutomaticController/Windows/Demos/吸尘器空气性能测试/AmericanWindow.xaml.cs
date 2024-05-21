using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{
    /// <summary>
    /// AmericanWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AmericanWindow : Window
    {
        public static AmericanWindow instance;
        List<TableData> datas = new List<TableData>();
        ObservableDataSource<Point> hs = new ObservableDataSource<Point>();
        ObservableDataSource<Point> Ps = new ObservableDataSource<Point>();
        ObservableDataSource<Point> Aps = new ObservableDataSource<Point>();
        ObservableDataSource<Point> effs = new ObservableDataSource<Point>();
        void DataClear()
        {
            datas.Clear();
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                DataGrid1.ItemsSource = null;
                DataGrid1.ItemsSource = datas;
            });
        }
        public AmericanWindow()
        {
            InitializeComponent();

            LoadData();
            instance = this;
            plotter.AddLineGraph(hs, Colors.Black, 1, "h");
            plotter.AddLineGraph(Ps, Colors.Red, 1, "P1");
            plotter.AddLineGraph(Aps, Colors.Blue, 1, "P2");
            plotter.AddLineGraph(effs, Colors.Green, 1, "η");

            bool start = true;
            this.Closed += (s, e) => {
                start = false;
            };
            //循环检测
            Task.Run(() =>
            {
                while(start)
                {
                    Loop();
                    Task.Delay(10).Wait();
                }
            });


            this.Loaded += (s, e) => {
                CompositionTarget.Rendering += CompositionTarget_Rendering; ;
            };
            this.Unloaded += (s, e) => CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //TestNoText.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (Devices.PLC1.Connected)
            {
                StateTextBlock1.Text = Devices.PLC1.CommunicationDelay.ToString("0.0");
            }
            else
            {
                StateTextBlock1.Text = "--";
            }
        }

        int lastState = 0;
        bool _IsStart;
        bool IsStart { get { return _IsStart; }set {
                App.Current.Dispatcher.InvokeAsync(new Action(() => {
                    if (value)
                    {
                        ManualGroupBox.IsEnabled = false;
                        startButton.IsEnabled = false;
                        stopButton.IsEnabled = true;
                        TestNoText.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
                    }
                    else
                    {
                        ManualGroupBox.IsEnabled = true;
                        startButton.IsEnabled = true;
                        stopButton.IsEnabled = false;
                    }
                    _IsStart = value;
                }));} }
        bool M20 = false;
        public void Loop()
        {
            int state = (int)PLC1Data.State.Value;
            bool m20 = PLC1Data.M20.Value;
            //启动后

            if ((state >= 10 && lastState < 10) || (state >= 100 && lastState < 10))
            {
                IsStart = true;
                DataClear();
            }
            if(state == 0)
            {
                IsStart = false;
            }
            //开始记录
            if (M20 != m20 && m20 == true)
            {
                PLC1Data.M20.Value = false;
                if (PLC1Data.M1000.Value == false)
                {
                    TestVoltage = PLC1Data.ACVoltage.Value.ToString();
                    TestFrequency = "AC " + PLC1Data.Frequency.Value.ToString();
                }
                else
                {
                    TestVoltage = PLC1Data.DCVoltage.Value.ToString();
                    TestFrequency = "DC";
                }
                int d = (int)PLC1Data.D5.Value;
                Task.Delay(500).ContinueWith(t =>
                {
                    if (d > 0 && d <= 16)
                    {
                        AddData(16 - d);
                        UpdateCurv();
                    }
                });
            }
            lastState = state;
            M20 = m20;
        }

        public void LoadData()
        {
            DataGrid1.ItemsSource = datas;
            DataGrid1.AutoGeneratedColumns += (object sender, EventArgs e) =>
            {
                UpdateColumnName(TableData.Names1);
            };
            //添加首行序号
            DataGrid1.LoadingRow += (object sender, DataGridRowEventArgs e) => e.Row.Header = e.Row.GetIndex() + 1;

        }
        /// <summary>
        /// 更新列名
        /// </summary>
        /// <param name="names"></param>
        public void UpdateColumnName(string[] names)
        {
            //设置列名
            int len = DataGrid1.Columns.Count;
            int min = Math.Min(names.Length, len);
            
            for (int i = 0; i < min; i++)
            {
                DataGrid1.Columns[i].Header = names[i];
            }
        }

        public static void Add(int DN)
        {
            //(instance.DataGrid1.ItemsSource as List<TableData>).Add(new TableData());
 
            //输入真空度
            double h = PLC1Data.Suction1_inH20.Value;
            //输入功率
            double p = PLC1Data.Power.Value;


            double Bt = TemperatureData.Atmospheric2.Value;
            double Td = TemperatureData.DrybulbTemperature2.Value;
            double Tw = TemperatureData.WetBulbTemperature2.Value;
            //根据温湿度计算修正
            double Dr = (17.68 * Bt - 0.001978 * Tw * Tw + 0.1064 * Tw + 0.0024575 * Bt * (Td - Tw) - 2.741) / (Td + 459.7);
            //吸力修正系数
            double Cs = 1 + 0.667 * (1 - Dr);
            //输入功率修正系数
            double Cp = 1 + 0.5 * (1 - Dr);
            double hs = h * Cs;
            double Ps = p * Cs;

            double r = (Bt * 0.4912 - h * 0.03607) / (Bt * 0.4912);
            //孔径列表
            double[] diameters = new double[]
            {   0,
                0.25,
                0.375,
                0.5,
                0.625,
                0.75,
                0.875,
                1,
                1.125,
                1.25,
                1.375,
                1.5,
                1.75,
                2,
                2.25,
                2.5,
 };
            //孔系数
            double[] Ks = new double[]
            {
                 0,
                (0.5575*r-0.5955)/(r - 1.0468),
                (0.5553*r-0.5754)/(r - 1.0263),
                (0.5694*r-0.5786)/(r - 1.0138),
                (0.5692*r-0.5767)/(r - 1.0104),
                (0.5715*r-0.5807)/(r - 1.0138),
                (0.5740*r-0.5841)/(r - 1.0158),
                (0.5687*r-0.5785)/(r - 1.0146),
                (0.5675*r-0.5819)/(r - 1.0225),
                (0.5717*r-0.5814)/(r - 1.0152),
                (0.5680*r-0.5826)/(r - 1.0235),
                (0.5719*r-0.5820)/(r - 1.0165),
                (0.5695*r-0.5839)/(r - 1.0235),
                (0.5757*r-0.5853)/(r - 1.0157),
                (0.5709*r-0.5878)/(r - 1.0279),
                (0.5660*r-0.59024)/(r - 1.0400),
            };

            //CFM
            double Q1 = 21.844 * diameters[DN] * diameters[DN] * Ks[DN] * Math.Sqrt(hs);
            //L/s
            double Q2 = 10.309 * diameters[DN] * diameters[DN] * Ks[DN] * Math.Sqrt(hs);
            //吸入功率
            double AP = 0.117354 * Q1 * hs;
            double eff = Math.Round(AP / Ps * 100, 2);
            if (eff.IsFinite() == false) eff = 0;
            TableData data = new TableData()
            {
                OrificeDiameter = diameters[DN],
                OrificeDiameter_ = diameters[DN] * 25.4,
                InputPower1 = p,
                Suction1 = Math.Round(h, 2),
                Suction1_ = Math.Round(h * 0.2491, 2),
                InputPower2 = Math.Round(Ps, 2),
                Suction2 = Math.Round(hs, 2),
                Suction2_ = Math.Round(hs * 0.2491, 2),
                Airflow = Math.Round(Q1, 2),
                Airflow_ = Math.Round(Q2, 2),
                AirPower = Math.Round(AP, 2),
                Efficiency = eff,
                Atmosphere = Bt,
                Atmosphere_ = TemperatureData.Atmospheric1.Value,
                DryBulbTemperature = Td,
                DryBulbTemperature_ = TemperatureData.DrybulbTemperature1.Value,
                WetBulbTemperature = Tw,
                WetBulbTemperature_ = TemperatureData.WetBulbTemperature1.Value,
                Humidity = TemperatureData.RelativeHidity1.Value
            };
            instance.datas.Add(data);





        }
        //更新曲线
        public static void UpdateCurv()
        {
            List<TableData> data = instance.datas;
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                instance.hs.Collection.Clear();
                instance.Ps.Collection.Clear();
                instance.Aps.Collection.Clear();
                instance.effs.Collection.Clear();
                foreach (var item in data)
                {
                    instance.hs.AppendAsync(App.Current.Dispatcher, new Point(item.Airflow, item.Suction1));
                    instance.Ps.AppendAsync(App.Current.Dispatcher, new Point(item.Airflow, item.InputPower1));
                    instance.Aps.AppendAsync(App.Current.Dispatcher, new Point(item.Airflow, item.AirPower));
                    instance.effs.AppendAsync(App.Current.Dispatcher, new Point(item.Airflow, item.Efficiency));
                }
                //instance.plotter.Legend.Remove();
                instance.plotter.FitToView();
            });
        }

        public static void AddData(int DN)
        {
            App.Current.Dispatcher.InvokeAsync(new Action(() => {
                Add(DN);
                UpdateCurv();
                instance.DataGrid1.ItemsSource = null;
                instance.DataGrid1.ItemsSource = instance.datas;
            }));
        }

        string TestVoltage = "";
        string TestFrequency = "";
        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UBitButton_Click(object sender, RoutedEventArgs e)
        {
            string c2 = TestNoText.Text;
            string c3 = ProductModelText.Text;
            string h3 = MotorModelText.Text;
            string c4 = OperatorText.Text;
            string h4 = ClientText.Text;

            //将模板复制到临时文件
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AutomaticController.Windows.Demos.吸尘器空气性能测试.模板美标.xlsx");
            if (Directory.Exists("报表") == false)
            {
                Directory.CreateDirectory("报表");
            }
            string tempPath = "报表\\" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".xlsx";
            using (FileStream fs = new FileStream(tempPath, FileMode.Create))
            {
                tempPath = fs.Name;
                stream.CopyTo(fs);
                fs.Flush();
                fs.Close();
            }
            //创建一个进度条
            var progress = OpenProgressBar(7);
            Task.Run(() =>
            {
                progress("创建文件", 1, false);

                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                application.ScreenUpdating = false;
                progress("打开文件", 2, false);
                Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(tempPath);
                //选择表格
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
                progress("写入表格数据", 3, false);
                worksheet.Cells[2,3] = c2;
                worksheet.Cells[3,3] = c3;
                worksheet.Cells[3,8] = h3;
                worksheet.Cells[4,3] = c4;
                worksheet.Cells[4,8] = h4;
                progress("写入表格数据", 4, false);

                worksheet.Cells[5, 3] = TestVoltage;
                worksheet.Cells[5, 8] = TestFrequency;
                worksheet.Cells[6, 3] = PLC1Data.D1050.Value.ToString();
                worksheet.Cells[6, 8] = PLC1Data.D1052.Value.ToString();
                worksheet.Cells[6, 13] = PLC1Data.D1054.Value.ToString();
                progress("写入表格数据", 5, false);

                int n1 = 9;
                foreach (var item in datas)
                {
                    worksheet.Cells[n1, 1] = item.OrificeDiameter;
                    worksheet.Cells[n1, 2] = item.InputPower2;
                    worksheet.Cells[n1, 3] = item.Suction2;
                    worksheet.Cells[n1, 4] = item.Suction2_;

                    worksheet.Cells[n1, 5] = item.Airflow;
                    worksheet.Cells[n1, 6] = item.Airflow_;
                    worksheet.Cells[n1, 7] = item.AirPower;
                    worksheet.Cells[n1, 8] = item.Efficiency;



                    worksheet.Cells[n1, 9] = item.Atmosphere;
                    worksheet.Cells[n1, 10] = item.Atmosphere_;
                    worksheet.Cells[n1, 11] = item.DryBulbTemperature;
                    worksheet.Cells[n1, 12] = item.DryBulbTemperature_;
                    worksheet.Cells[n1, 13] = item.WetBulbTemperature;
                    worksheet.Cells[n1, 14] = item.WetBulbTemperature_;
                    worksheet.Cells[n1, 15] = item.Humidity;
                    n1++;
                }


                progress("生成完成", 6, false);

                application.ScreenUpdating = true;
                workbook.Save();
                workbook.Close();
                progress("打开文件夹", 7, false);
                // 打开资源管理器并选中文件
                Process.Start("explorer.exe", $"/select,{tempPath}");
                Task.Delay(1000).Wait();
                progress.Invoke("", 0, true);
            });


        }


        /// <summary>
        /// 在本窗口创建一个进度条
        /// </summary>
        /// <param name="Maximum"></param>
        /// <returns></returns>
        Action<string, double, bool> OpenProgressBar(double Maximum)
        {
            Grid grid = ((Grid)this.Content);
            //添加背景
            System.Windows.Controls.Border border = new System.Windows.Controls.Border();
            border.Background = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200));
            grid.Children.Add(border);
            //停用窗体
            this.IsEnabled = false;

            ProgressBar progressBar = new ProgressBar();
            grid.Children.Add(progressBar);
            progressBar.Width = 300;
            progressBar.Height = 60;
            TextBlock textBlock = new TextBlock();
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(textBlock);
            progressBar.Tag = textBlock;


            progressBar.Maximum = Maximum;
            
            return (s, i, c) => {
                App.Current.Dispatcher.InvokeAsync(() =>
                {
                    textBlock.Text = s;
                    progressBar.Value = i;
                    if (c)
                    {
                        grid.Children.Remove(progressBar);
                        grid.Children.Remove(textBlock);
                        grid.Children.Remove(border);
                        this.IsEnabled = true;
                    }
                });

            };
        }

  
    }
}
