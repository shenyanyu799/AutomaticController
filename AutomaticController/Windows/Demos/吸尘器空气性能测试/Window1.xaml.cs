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
using System.Windows.Shell;
using Microsoft.Office.Interop.Excel;
using static System.Net.WebRequestMethods;

namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{
    //****************************************
    //测试报表生成功能
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : System.Windows.Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //将模板复制到临时文件
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AutomaticController.Windows.Demos.吸尘器空气性能测试.模板.xlsx");
            if(Directory.Exists("报表") == false)
            {
                Directory.CreateDirectory("报表");
            }
            string tempPath = "报表\\" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".xlsx";
            using(FileStream fs = new FileStream(tempPath, FileMode.Create))
            {
                tempPath = fs.Name;
                stream.CopyTo(fs);
                fs.Flush();
                fs.Close();
            }
            var pro = OpenProgressBar(6);
            Task.Run(()=>Test2(tempPath, pro));

        }

        void Test2(string tempPath, Action<string, int, bool> progress)
        {
            progress("正在创建文件", 1, false);

            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            application.ScreenUpdating = false;

            Workbook workbook = application.Workbooks.Open(tempPath);
            //选择表格
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            Worksheet worksheet2 = (Worksheet)workbook.Worksheets[2];
            progress("正在写入表格数据", 2, false);


            worksheet.Cells[1, 1] = DateTime.Now;
            worksheet.Cells[2, 1] = "数据2";
            Random random = new Random();

            progress("生成数据1", 1, false);

            for (int i = 0; i < 1000; i++)
            {
                worksheet2.Cells[2 + i, 1] = random.Next(1, 100);
            }
            progress("生成数据2", 1, false);

            for (int i = 0; i < 1000; i++)
            {
                worksheet2.Cells[2 + i, 2] = random.Next(1, 50);
            }
            progress("生成数据3", 1, false);
            for (int i = 0; i < 1000; i++)
            {
                worksheet2.Cells[2 + i, 3] = random.Next(1, 200);
            }
            progress("生成完成", 1, false);

            application.ScreenUpdating = true;
            application.Visible = true;
            Console.WriteLine("正在启动表格");

            progress("表格已启动", 0, true);


        }



        void Test()
        {
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = application.Workbooks.Add();
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            worksheet.Name = "检测数据";
            // 定义表格的数据  
            List<object[]> data = new List<object[]>();
            data.Add(new object[] { "Name", "Age", "City", "备注", "", "" });
            data.Add(new object[] { "Alice", 28, "New York" });
            data.Add(new object[] { "Bob", 32, "San Francisco" });
            data.Add(new object[] { "syy", 25, "Los 314" });
            data.Add(new object[] { "stt", 22, "Los qqq2" });
            data.Add(new object[] { "sat", 22, "Los Angeles" });

            // 添加表格到工作表  
            int rows = data.Count;

            for (int i = 0; i < rows; i++)
            {
                int cols = data[i].Length;
                for (int j = 0; j < cols; j++)
                {
                    worksheet.Cells[i + 1, j + 1] = data[i][j];

                }
            }
            //合并单元格
            Range rangeToMerge = worksheet.get_Range("D1", "F1");
            rangeToMerge.Merge();
            // 设置水平和垂直居中  
            rangeToMerge.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            rangeToMerge.VerticalAlignment = XlVAlign.xlVAlignCenter;

            // 定义边框样式  
            var borderStyle = XlLineStyle.xlContinuous;
            var borderColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            var borderWeight = XlBorderWeight.xlThin;
            // 为整个工作表设置边框  
            Range range = worksheet.UsedRange; // 使用的范围，即包含数据的区域  
            range.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = borderStyle;
            range.Borders[XlBordersIndex.xlEdgeTop].LineStyle = borderStyle;
            range.Borders[XlBordersIndex.xlEdgeLeft].LineStyle = borderStyle;
            range.Borders[XlBordersIndex.xlEdgeRight].LineStyle = borderStyle;
            range.Borders[XlBordersIndex.xlInsideVertical].LineStyle = borderStyle;
            range.Borders[XlBordersIndex.xlInsideHorizontal].LineStyle = borderStyle;

            // 自动调整列宽  
            worksheet.Columns.AutoFit();
            application.Visible = true;
        }


        /// <summary>
        /// 在本窗口创建一个进度条
        /// </summary>
        /// <param name="Maximum"></param>
        /// <returns></returns>
        Action<string,int,bool> OpenProgressBar(double Maximum)
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
            progressBar.Tag= textBlock;


            progressBar.Maximum = Maximum;

            return (s,i,c)=> {
                App.Current.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = s;
                    progressBar.Value += i;
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
