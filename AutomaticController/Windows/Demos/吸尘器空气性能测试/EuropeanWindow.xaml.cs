using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using System;
using System.Collections.Generic;
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
using System.Diagnostics;

namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{
    /// <summary>
    /// EuropeanWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EuropeanWindow : Window
    {
        public static EuropeanWindow instance;
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
        public EuropeanWindow()
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
                while (start)
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
            if (Devices.PLC2.Connected)
            {
                StateTextBlock1.Text = Devices.PLC2.CommunicationDelay.ToString("0.0");
            }
            else
            {
                StateTextBlock1.Text = "--";
            }
        }

        int lastState = 0;
        bool _IsStart;
        bool IsStart
        {
            get { return _IsStart; }
            set
            {
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
                }));
            }
        }
        bool M20 = false;
        public void Loop()
        {
            int state = (int)PLC2Data.State.Value;
            bool m20 = PLC2Data.M20.Value;
            //启动后
            if ((state >= 10 && lastState < 10) || (state >= 100 && lastState < 10))
            {
                IsStart = true;
                DataClear();
            }
            if (state == 0)
            {
                IsStart = false;
            }
            //开始记录
            if (M20 != m20 && m20 == true)
            {
                if (PLC2Data.M1000.Value == false)
                {
                    TestVoltage = PLC2Data.ACVoltage.Value.ToString();
                    TestFrequency = "AC " + PLC2Data.Frequency.Value.ToString();
                }
                else
                {
                    TestVoltage = PLC2Data.DCVoltage.Value.ToString();
                    TestFrequency = "DC";
                }
                int d = (int)PLC2Data.D5.Value;
                Task.Delay(500).ContinueWith(t =>
                {
                    if (d > 0 && d <= 10)
                    {
                        AddData(10 - d);
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
                UpdateColumnName(TableData.Names2);
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
            double h = PLC2Data.Suction1_kPa.Value;
            //输入功率
            double p = PLC2Data.Power.Value;


            double Bt = TemperatureData.Atmospheric1.Value;
            double Td = TemperatureData.DrybulbTemperature1.Value;
            double Tw = TemperatureData.WetBulbTemperature1.Value;
            double rh = TemperatureData.RelativeHidity1.Value;

            //根据温湿度计算修正
            double pRH = 0.001 * (0.44 - rh * (2.32 + 0.212 * Td + 0.00028 * Td * Td * Td));
            double Dm = ((Bt + pRH) / 101.3) * (293 / (Td + 273));

            double hs = h * Math.Pow(Dm,-0.67);
            double Ps = p * 1;

            //孔径列表
            double[] diameters = new double[]
            {   0,
                6.5,
                10,
                13,
                16,
                19,
                23,
                30,
                40,
                50,
            };
            //孔系数
            double a = 0.595 + (0.0776 * 2 / diameters[DN]) - (0.0017 * hs);
            //L/s
            double Q1 = a * 0.032 * diameters[DN] * diameters[DN] * Math.Sqrt(hs) * Math.Pow(Dm, 0.17);
            //CFM
            double Q2 = Q1 * 2.11888;
            //吸入功率
            double AP = Q1 * hs;
            double eff = Math.Round(AP / Ps * 100, 2);
            if (eff.IsFinite() == false) eff = 0;
            TableData data = new TableData()
            {
                OrificeDiameter = diameters[DN],
                OrificeDiameter_ = Math.Round(diameters[DN] * 0.03937,2),
                InputPower1 = p,
                Suction1 = Math.Round(h, 2),
                Suction1_ = Math.Round(h * 4.0147, 2),
                InputPower2 = Math.Round(Ps, 2),
                Suction2 = Math.Round(hs, 2),
                Suction2_ = Math.Round(hs * 4.0147, 2),
                Airflow = Math.Round(Q1, 2),
                Airflow_ = Math.Round(Q2, 2),
                AirPower = Math.Round(AP, 2),
                Efficiency = eff,
                Atmosphere = Bt,
                Atmosphere_ = TemperatureData.Atmospheric2.Value,
                DryBulbTemperature = Td,
                DryBulbTemperature_ = TemperatureData.DrybulbTemperature2.Value,
                WetBulbTemperature = Tw,
                WetBulbTemperature_ = TemperatureData.WetBulbTemperature2.Value,
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
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AutomaticController.Windows.Demos.吸尘器空气性能测试.模板欧标.xlsx");
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
                progress("载入表格文件", 2, false);
                Microsoft.Office.Interop.Excel.Workbook workbook = application.Workbooks.Open(tempPath);
                //选择表格
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
                progress("写入表格数据", 3, false);
                worksheet.Cells[2, 3] = c2;
                worksheet.Cells[3, 3] = c3;
                worksheet.Cells[3, 8] = h3;
                worksheet.Cells[4, 3] = c4;
                worksheet.Cells[4, 8] = h4;
                progress("写入表格数据", 4, false);

                worksheet.Cells[5, 3] = TestVoltage;
                worksheet.Cells[5, 8] = TestFrequency;
                worksheet.Cells[6, 3] = PLC2Data.D1050.Value.ToString();
                worksheet.Cells[6, 8] = PLC2Data.D1052.Value.ToString();
                worksheet.Cells[6, 13] = PLC2Data.D1054.Value.ToString();
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
