using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Shapes;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Datas
{
    //运行过程中会自动存储的数据
    public struct RunningData
    {
        private static RunningData[] States { get; set; } = new RunningData[30];
        public int TestCount { get; set; }
        //记录状态用
        //public int Tag { get; set; }
        //public int Runtime { get; set; }
        public DateTime StartTime { get; set; }

        public string Name { get; set; }

        private static bool readingOrwriting;
        public static RunningData Get(int index)
        {
            if (index < States.Length)
            {
                return States[index];
            }
            return new RunningData();
        }
        public static void Set(int index, RunningData state)
        {
            if (index < States.Length)
            {
                States[index] = state;
                Save();
            }
        }

        public static void Load()
        {
            if (readingOrwriting) return;
            string path = "runtime";
            if (File.Exists(path))
            {
                readingOrwriting = true;
                string json = File.ReadAllText(path);
                States = JsonConvert.DeserializeObject<RunningData[]>(json);
                readingOrwriting = false;
            }
        }
        public static void Save()
        {
            if (readingOrwriting) return;
            readingOrwriting = true;
            byte[] bytes = new byte[States.Length * Marshal.SizeOf(default(RunningState))];
            string path = "runtime";
            File.WriteAllText(path, JsonConvert.SerializeObject(States));
            readingOrwriting = false;
        }
    }
    //运行过程中的普通数据
    public struct RunningState
    {
        public static RunningState[] States {  get; set; } = new RunningState[30];

        public double ElectriCurrent { get; set; }
        public double MaxCurrent { get; set; }
        public double MinCurrent { get; set; }
        public double AverageCurrent { get; set; }
        public int Runtime_s { get; set; }
        
        /// <summary>
        /// 0=>待机中; 1=>准备启动; 2=>运行异常; 3=>运行完成; 10=>运行中;
        /// </summary>
        public int State { get; set; }
        public bool RequestStart { get; set; }
        public bool RequestStop { get; set; }
        public DateTime RuningTime { get; set; }
    }
    public struct Parameters
    {

        public Parameters(string name)
        {
            Name = name;
            TotalCount = 100;
            PowerOnTime = 3600;
            PowerOffTime = 3600;
            AlarmDelay = 1;
            ElectriCurrentUpper = 10;
            ElectriCurrentLower = 0;

        }
        public string Name { get; set; } 
        /// <summary>
        /// 测试总次数
        /// </summary>
        public int TotalCount { get; set; } 
        /// <summary>
        /// 开启时间
        /// </summary>
        public double PowerOnTime { get; set; } 
        /// <summary>
        /// 关闭时间
        /// </summary>
        public double PowerOffTime { get; set; }
        /// <summary>
        /// 报警延时
        /// </summary>
        public double AlarmDelay { get; set; }
        /// <summary>
        /// 电流上限
        /// </summary> 
        public double ElectriCurrentUpper{ get; set; }
        /// <summary>
        /// 电流下限
        /// </summary> 
        public double ElectriCurrentLower { get; set; }
    }
    
    public class Parameters_XMLFile : XMLFile
    {
        public Parameters this[string name] { get => GetValue(name, new Parameters("default")); set => SetValue(name, value); }
        
        public static Parameters_XMLFile Instance { get; set; } = new Parameters_XMLFile("parameters.xml");

        public Parameters_XMLFile(string path) : base(path) { }
    }
}
