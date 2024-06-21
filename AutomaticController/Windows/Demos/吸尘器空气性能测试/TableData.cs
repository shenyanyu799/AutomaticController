namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{
    /// <summary>
    /// 表格数据
    /// </summary>
    public class TableData
    {
        public static string[] Names1 { get; set; } = new string[]
        {
            "Diameter\n d(In.)",
            "Diameter\n d(mm)",
            "InputPower1\n p(W)",
            "Suction1\n h(In.H2O)",
            "Suction1\n h(kPa)",
            "InputPower2\n Ps(W)",
            "Suction2\n Hs(In.H2O)",
            "Suction2\n Hs(kPa)",
            "Airflow\n Q(CFM)",
            "Airflow\n Q(L/s)",
            "AirPower\n AP(AW)",
            "Efficiency\n η(%)",
            "Atmosphere\n (in.Hg)",
            "Atmosphere\n (kPa)",
            "DryBulbTemp\n Td(℉)",
            "DryBulbTemp\n Td(℃)",
            "WetBulbTemp\n Wd(℉)",
            "WetBulbTemp\n Wd(℃)",
            "Humidity\n RH(%)",
        };
        public static string[] Names2 { get; set; } = new string[]
{
            "Diameter\n d(mm)",
            "Diameter\n d(in.)",
            "InputPower1\n p(W)",
            "Suction1\n h(kPa)",
            "Suction1\n h(In.H2O)",
            "InputPower2\n Ps(W)",
            "Suction2\n Hs(kPa)",
            "Suction2\n Hs(In.H2O)",
            "Airflow\n Q(L/s)",
            "Airflow\n Q(CFM)",
            "AirPower\n AP(AW)",
            "Efficiency\n η(%)",
            "Atmosphere\n (kPa)",
            "Atmosphere\n (in.Hg)",
            "DryBulbTemp\n Td(℃)",
            "DryBulbTemp\n Td(℉)",
            "WetBulbTemp\n Wd(℃)",
            "WetBulbTemp\n Wd(℉)",
            "Humidity\n RH(%)",
};


        /// <summary>
        /// 孔径
        /// </summary>
        public double OrificeDiameter { get; set; }
        public double OrificeDiameter_ { get; set; }
        /// <summary>
        /// 输入功率
        /// </summary>
        public double InputPower1 { get; set; }

        /// <summary>
        /// 真空度
        /// </summary>
        public double Suction1 { get; set; }
        public double Suction1_ { get; set; }
        /// <summary>
        /// 修正输入功率
        /// </summary>

        public double InputPower2 { get; set; }

        /// <summary>
        /// 修正真空度
        /// </summary>
        public double Suction2 { get; set; }
        public double Suction2_ { get; set; }
        /// <summary>
        /// 风量
        /// </summary>
        public double Airflow { get; set; }
        public double Airflow_ { get; set; }
        /// <summary>
        /// 吸入功率
        /// </summary>
        public double AirPower { get; set; }
        /// <summary>
        /// 效率
        /// </summary>
        public double Efficiency { get; set; }




        /// <summary>
        /// 大气压强
        /// </summary>
        public double Atmosphere { get; set; }
        public double Atmosphere_ { get; set; }
        /// <summary>
        /// 干球温度
        /// </summary>
        public double DryBulbTemperature { get; set; }
        public double DryBulbTemperature_ { get; set; }
        /// <summary>
        /// 湿球温度
        /// </summary>
        public double WetBulbTemperature { get; set; }
        public double WetBulbTemperature_ { get; set; }
        /// <summary>
        /// 相对湿度
        /// </summary>
        public double Humidity { get; set; }
    }
}
