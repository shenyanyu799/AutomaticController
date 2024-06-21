using AutomaticController.Device;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{

    /// <summary>
    /// 美标
    /// </summary>
    public class PLC1Data
    {
        static Modbus_RTU PLC => Devices.PLC1;
        public static Modbus_RTU_Num ID { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(0), Modbus_EasyAddress.WordType);
        /// <summary>
        /// 运行状态
        /// </summary>
        public static Modbus_RTU_Num State { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1), Modbus_EasyAddress.WordType) { AutoRead = true };
        /// <summary>
        /// 当前位置
        /// </summary>
        public static Modbus_RTU_Num D5 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(5), Modbus_EasyAddress.WordType) { AutoRead = true };
        /// <summary>
        /// 真空度kPa
        /// </summary>
        public static Modbus_RTU_Num D10 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(10), Modbus_EasyAddress.FloatType) { AutoRead = true };

        public static Modbus_RTU_Num D12 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(12), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D13 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(13), Modbus_EasyAddress.WordType) { AutoRead = true };

        public static Modbus_RTU_Num D14 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(14), Modbus_EasyAddress.WordType) { AutoRead = true };

        public static Modbus_RTU_Num D15 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(15), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D16 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(16), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D17 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(17), Modbus_EasyAddress.WordType) { AutoRead = true };

        public static Modbus_RTU_Num D18 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(18), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D19 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(19), Modbus_EasyAddress.FloatType) { AutoRead = true };


        /// <summary>
        /// 输入真空度in.H2O
        /// </summary>
        public static TextDouble Suction1_inH20 { get; set; } = new TextDouble(() => D10.Value * 4.014741295, d => D10.Value = d / 4.014741295);
        public static TextDouble Suction1_kPa { get; set; } = new TextDouble(() => D10.Value, d => D10.Value = d);




        public static TextDouble ACVoltage { get; set; } = new TextDouble(() => Math.Round(D12.Value * 0.03, 2), d => D12.Value = d / 0.03, "0.00");
        public static TextDouble ACCurrent { get; set; } = new TextDouble(() => Math.Round(D13.Value * 0.002, 2), d => D13.Value = d / 0.002, "0.00");
        public static TextDouble ACPower { get; set; } = new TextDouble(() => Math.Round(D14.Value * 0.6, 2), d => D14.Value = d / 0.6, "0.00");
        public static TextDouble Frequency { get; set; } = new TextDouble(() => Math.Round(D15.Value * 0.01, 2), d => D15.Value = d / 0.01, "0.00");

        public static TextDouble DCVoltage { get; set; } = new TextDouble(() => Math.Round(D16.Value * 0.005, 2), d => D16.Value = d / 0.005, "0.00");
        public static TextDouble DCCurrent { get; set; } = new TextDouble(() => Math.Round(D17.Value * 0.002, 2), d => D17.Value = d / 0.002, "0.00");
        public static TextDouble DCPower { get; set; } = new TextDouble(() => Math.Round(D18.Value * 0.15, 2), d => D18.Value = d / 0.15, "0.00");

        //public static TextDouble Power { get; set; } = new TextDouble(() => D19.Value, d => D19.Value = d);
        public static TextDouble Power { get; set; } = new TextDouble(() => ACPower.Value, d => ACPower.Value = d);




        /// <summary>
        /// 孔1手动测试
        /// </summary>
        public static Modbus_RTU_Bit M0 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(0));
        /// <summary>
        /// 孔2手动测试
        /// </summary>
        public static Modbus_RTU_Bit M1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1));
        public static Modbus_RTU_Bit M2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(2));
        public static Modbus_RTU_Bit M3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(3));
        public static Modbus_RTU_Bit M4 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(4));
        public static Modbus_RTU_Bit M5 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(5));
        public static Modbus_RTU_Bit M6 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(6));
        public static Modbus_RTU_Bit M7 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(7));
        public static Modbus_RTU_Bit M8 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(8));
        public static Modbus_RTU_Bit M9 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(9));
        public static Modbus_RTU_Bit M10 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(10));
        public static Modbus_RTU_Bit M11 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(11));
        public static Modbus_RTU_Bit M12 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(12));
        public static Modbus_RTU_Bit M13 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(13));
        public static Modbus_RTU_Bit M14 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(14));
        public static Modbus_RTU_Bit M15 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(15));
        public static Modbus_RTU_Bit M16 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(16));
        public static Modbus_RTU_Bit M17 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(17));
        /// <summary>
        /// 记录
        /// </summary>
        public static Modbus_RTU_Bit M20 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(20)) { AutoRead = true };
        /// <summary>
        ///  启动
        /// </summary>
        public static Modbus_RTU_Bit M21 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(21));
        /// <summary>
        /// 停止
        /// </summary>
        public static Modbus_RTU_Bit M22 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(22));

        /// <summary>
        /// 电源模式选择
        /// </summary>
        public static Modbus_RTU_Bit M1000 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1000)) { AutoRead = true };



        /// <summary>
        /// 热机时间
        /// </summary>
        public static Modbus_RTU_Num D1050 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1050), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        /// <summary>
        /// 开口等待时间
        /// </summary>
        public static Modbus_RTU_Num D1052 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1052), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        /// <summary>
        /// 测试时间
        /// </summary>
        public static Modbus_RTU_Num D1054 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1054), Modbus_EasyAddress.DWordType) { Scale = 0.001 };

    }
    /// <summary>
    /// 欧标
    /// </summary>
    public class PLC2Data
    {
        static Modbus_RTU PLC => Devices.PLC2;
        public static Modbus_RTU_Num ID { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(0), Modbus_EasyAddress.WordType);
        /// <summary>
        /// 运行状态
        /// </summary>
        public static Modbus_RTU_Num State { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1), Modbus_EasyAddress.WordType) { AutoRead = true };
        /// <summary>
        /// 当前位置
        /// </summary>
        public static Modbus_RTU_Num D5 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(5), Modbus_EasyAddress.WordType) { AutoRead = true };
        /// <summary>
        /// 真空度kPa
        /// </summary>
        public static Modbus_RTU_Num D10 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(10), Modbus_EasyAddress.FloatType) { AutoRead = true };

        public static Modbus_RTU_Num D12 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(12), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D13 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(13), Modbus_EasyAddress.WordType) { AutoRead = true };

        public static Modbus_RTU_Num D14 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(14), Modbus_EasyAddress.WordType) { AutoRead = true };

        public static Modbus_RTU_Num D15 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(15), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D16 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(16), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D17 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(17), Modbus_EasyAddress.WordType) { AutoRead = true };

        public static Modbus_RTU_Num D18 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(18), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num D19 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(19), Modbus_EasyAddress.FloatType) { AutoRead = true };


        /// <summary>
        /// 输入真空度in.H2O
        /// </summary>
        public static TextDouble Suction1_inH20 { get; set; } = new TextDouble(() => D10.Value * 4.014741295, d => D10.Value = d / 4.014741295);
        public static TextDouble Suction1_kPa { get; set; } = new TextDouble(() => D10.Value, d => D10.Value = d);




        public static TextDouble ACVoltage { get; set; } = new TextDouble(() => Math.Round(D12.Value * 0.03, 2), d => D12.Value = d / 0.03, "0.00");
        public static TextDouble ACCurrent { get; set; } = new TextDouble(() => Math.Round(D13.Value * 0.002, 2), d => D13.Value = d / 0.002, "0.00");
        public static TextDouble ACPower { get; set; } = new TextDouble(() => Math.Round(D14.Value * 0.6, 2), d => D14.Value = d / 0.6, "0.00");
        public static TextDouble Frequency { get; set; } = new TextDouble(() => Math.Round(D15.Value * 0.01, 2), d => D15.Value = d / 0.01, "0.00");

        public static TextDouble DCVoltage { get; set; } = new TextDouble(() => Math.Round(D16.Value * 0.005, 2), d => D16.Value = d / 0.005, "0.00");
        public static TextDouble DCCurrent { get; set; } = new TextDouble(() => Math.Round(D17.Value * 0.002, 2), d => D17.Value = d / 0.002, "0.00");
        public static TextDouble DCPower { get; set; } = new TextDouble(() => Math.Round(D18.Value * 0.15, 2), d => D18.Value = d / 0.15, "0.00");

        //public static TextDouble Power { get; set; } = new TextDouble(() => D19.Value, d => D19.Value = d);
        public static TextDouble Power { get; set; } = new TextDouble(() => ACPower.Value, d => ACPower.Value = d);




        /// <summary>
        /// 孔1手动测试
        /// </summary>
        public static Modbus_RTU_Bit M0 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(0));
        /// <summary>
        /// 孔2手动测试
        /// </summary>
        public static Modbus_RTU_Bit M1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1));
        public static Modbus_RTU_Bit M2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(2));
        public static Modbus_RTU_Bit M3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(3));
        public static Modbus_RTU_Bit M4 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(4));
        public static Modbus_RTU_Bit M5 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(5));
        public static Modbus_RTU_Bit M6 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(6));
        public static Modbus_RTU_Bit M7 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(7));
        public static Modbus_RTU_Bit M8 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(8));
        public static Modbus_RTU_Bit M9 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(9));
        public static Modbus_RTU_Bit M10 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(10));
        public static Modbus_RTU_Bit M11 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(11));
        public static Modbus_RTU_Bit M12 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(12));
        public static Modbus_RTU_Bit M13 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(13));
        public static Modbus_RTU_Bit M14 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(14));
        public static Modbus_RTU_Bit M15 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(15));
        public static Modbus_RTU_Bit M16 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(16));
        public static Modbus_RTU_Bit M17 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(17));
        /// <summary>
        /// 记录
        /// </summary>
        public static Modbus_RTU_Bit M20 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(20)) { AutoRead = true };
        /// <summary>
        ///  启动
        /// </summary>
        public static Modbus_RTU_Bit M21 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(21));
        /// <summary>
        /// 停止
        /// </summary>
        public static Modbus_RTU_Bit M22 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(22));

        /// <summary>
        /// 电源模式选择
        /// </summary>
        public static Modbus_RTU_Bit M1000 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1000)) { AutoRead = true };



        /// <summary>
        /// 热机时间
        /// </summary>
        public static Modbus_RTU_Num D1050 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1050), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        /// <summary>
        /// 开口等待时间
        /// </summary>
        public static Modbus_RTU_Num D1052 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1052), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        /// <summary>
        /// 测试时间
        /// </summary>
        public static Modbus_RTU_Num D1054 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1054), Modbus_EasyAddress.DWordType) { Scale = 0.001 };

    }


    public class TemperatureData
    {
        /// <summary>
        /// 相对湿度%
        /// </summary>
        public static Modbus_RTU_Num RelativeHidity { get; set; } = new Modbus_RTU_Num(Devices.Temperature, 0x14, Modbus_RTU_Type.Float_LH) { ReadInterval = 1, AutoRead = true };
        public static TextDouble RelativeHidity1 { get; set; } = new TextDouble(() => Math.Round(RelativeHidity.Value, 2), d => RelativeHidity.Value = d, "0.00");

        /// <summary>
        /// 干球温度℃
        /// </summary>
        public static Modbus_RTU_Num DrybulbTemperature { get; set; } = new Modbus_RTU_Num(Devices.Temperature, 0x16, Modbus_RTU_Type.Float_LH) { ReadInterval = 1, AutoRead = true };
        /// <summary>
        /// 干球温度℃
        /// </summary>
        public static TextDouble DrybulbTemperature1 { get; set; } = new TextDouble(() => Math.Round(DrybulbTemperature.Value, 2), d => DrybulbTemperature.Value = d, "0.00");
        /// <summary>
        /// 干球温度℉
        /// </summary>
        public static TextDouble DrybulbTemperature2 { get; set; } = new TextDouble(() => Math.Round(DrybulbTemperature.Value * 1.8 + 32, 2), d => DrybulbTemperature.Value = (d - 32) / 1.8, "0.00");

        /// <summary>
        /// 湿球温度℃=> 1.8+32 ℉
        /// </summary>
        public static Modbus_RTU_Num WetBulbTemperature { get; set; } = new Modbus_RTU_Num(Devices.Temperature, 0x18, Modbus_RTU_Type.Float_LH) { ReadInterval = 1, AutoRead = true };
        /// <summary>
        /// 湿球温度℃
        /// </summary>
        public static TextDouble WetBulbTemperature1 { get; set; } = new TextDouble(() => Math.Round(WetBulbTemperature.Value, 2), d => WetBulbTemperature.Value = d, "0.00");
        /// <summary>
        /// 湿球温度℉
        /// </summary>
        public static TextDouble WetBulbTemperature2 { get; set; } = new TextDouble(() => Math.Round(WetBulbTemperature.Value * 1.8 + 32, 2), d => WetBulbTemperature.Value = (d - 32) / 1.8, "0.00");

        /// <summary>
        /// 大气压强kPa;1kPa = 0.295301 in.Hg
        /// </summary>
        public static Modbus_RTU_Num Atmospheric { get; set; } = new Modbus_RTU_Num(Devices.Temperature, 0x24, Modbus_RTU_Type.Float_LH) { ReadInterval = 1, AutoRead = true };
        /// <summary>
        /// kPa
        /// </summary>
        public static TextDouble Atmospheric1 { get; set; } = new TextDouble(() => Math.Round(Atmospheric.Value * 0.1, 2), d => Atmospheric.Value = d * 10, "0.00");
        /// <summary>
        /// in.Hg
        /// </summary>
        public static TextDouble Atmospheric2 { get; set; } = new TextDouble(() => Math.Round(Atmospheric.Value * 0.0295301, 2), d => Atmospheric.Value = d / 0.0295301, "0.00");

    }



    public class Devices
    {

        public static Modbus_RTU PLC1 { get; set; } = new Modbus_RTU(1);
        public static Modbus_RTU PLC2 { get; set; } = new Modbus_RTU(1);

        public static Modbus_RTU Temperature { get; set; } = new Modbus_RTU(1, 255, 255);

        static string ASName = null;
        static string ENName = null;
        static string TemperatureName = null; //=> Parameters.Instance.TemperatureName;
        public static bool IsPLC1Start { get; set; } = false;
        public static bool IsPLC2Start { get; set; } = false;
        public static bool IsTemperatureStart { get; set; } = false;
        static bool IsPLCFind = false;
        public static async void Find()
        {
            if (IsPLCFind) return;
            IsPLCFind = true;

            if (Parameters.Instance.ASName != null)
            {
                using (SerialPort port = new SerialPort(Parameters.Instance.ASName)) //创建串口
                {
                    FindPLC(port);
                }
            }
            if (Parameters.Instance.ENName != null)
            {
                using (SerialPort port = new SerialPort(Parameters.Instance.ENName)) //创建串口
                {
                    FindPLC(port);
                }
            }
            if (Parameters.Instance.TemperatureName != null)
            {
                using (SerialPort port = new SerialPort(Parameters.Instance.TemperatureName)) //创建串口
                {
                    FindTemp(port);
                }
            }


            while (IsPLCFind)
            {
                string[] names = SerialPort.GetPortNames();
                int n = 0;
                //查找PLC
                for (int i = 0; i < names.Length; i++)
                {
                    try
                    {
                        if (names[i] == ASName || names[i] == ENName || names[i] == TemperatureName)
                        {
                            n++;
                            continue;
                        }
                        int num = i;
                        using (SerialPort port = new SerialPort(names[num])) //创建串口
                        {

                            if (FindPLC(port) == false)
                            {
                                FindTemp(port);
                            }
                            n++;
                            port.Close();
                        }

                    }
                    catch { }


                }
                while (n != names.Length)
                {
                    await Task.Delay(100);
                }
                await Task.Delay(100);
            }
        }
        public static bool FindPLC(SerialPort port)
        {
            //美标编号24101，欧标编号24201
            byte[] mbnum = new byte[] { 0x5E, 0x25 };
            byte[] obnum = new byte[] { 0x5E, 0x89 };


            try
            {
                Console.Write(port.PortName + " PLC ");

                port.BaudRate = 19200;//设置波特率
                port.Parity = Parity.None;//设置校验

                port.StopBits = StopBits.One;//停止位位1
                port.ReadTimeout = 1000;//读取超时1s
                port.WriteTimeout = 1000;//写入超时1s
                port.Open();
                byte[] writeBytes = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0A };
                port.Write(writeBytes, 0, writeBytes.Length);
                Task.Delay(100).Wait();
                byte[] readByte = new byte[8];
                bool cs = false;
                for (int j = 0; j < 5; j++)
                {
                    Task.Delay(100).Wait();
                    if (port.BytesToRead > 0)
                    {
                        cs = true;
                    }
                }
                if (cs == false) throw (new TimeoutException(port.PortName + "读取超时"));
                int len = port.Read(readByte, 0, readByte.Length);

                foreach (var item in readByte)
                {
                    Console.Write(item.ToString("X2") + " ");
                }
                Console.WriteLine();
                if (readByte[3] == mbnum[0] && readByte[4] == mbnum[1])
                {
                    ASName = port.PortName;
                    Parameters.Instance.ASName = ASName;
                    port.Close();
                    return true;
                }
                if (readByte[3] == obnum[0] && readByte[4] == obnum[1])
                {
                    ENName = port.PortName;
                    Parameters.Instance.ENName = ASName;
                    port.Close();
                    return true;
                }
            }
            catch
            {
                port.Close();
            }
            return false;
        }
        public static bool FindTemp(SerialPort port)
        {
            try
            {
                port.BaudRate = 9600;//设置波特率
                port.Parity = Parity.None;//设置校验

                port.StopBits = StopBits.One;//停止位位1
                port.ReadTimeout = 500;//读取超时1s
                port.WriteTimeout = 1000;//写入超时1s
                port.Open();
                port.ReadExisting();
                byte[] writeBytes2 = new byte[] { 0x01, 0x03, 0x00, 0x0F, 0x00, 0x01, 0xB4, 0x09 };
                port.Write(writeBytes2, 0, writeBytes2.Length);
                Task.Delay(100).Wait();
                byte[] readByte = new byte[8];
                int len = port.Read(readByte, 0, readByte.Length);

                Console.Write(port.PortName + " Temp ");
                if (readByte[3] == 0 && readByte[4] == 1)
                {
                    TemperatureName = port.PortName;
                    Parameters.Instance.TemperatureName = TemperatureName;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        //启动PLC1
        public static void PLC1Start()
        {
            if (IsPLC1Start) return;
            IsPLC1Start = true;

            Task.Run(async () =>
            {

                while (IsPLC1Start)
                {
                    if (ASName == null)
                    {
                        Task.Run(Find);
                    }
                    SerialPort port = new SerialPort();
                    try
                    {
                        if (PLC1.Started == false && /*PLC1.Connected == false && */ASName != null)
                        {
                            port.PortName = ASName;
                            port.BaudRate = 19200;//设置波特率
                            port.Parity = Parity.None;//设置偶校验

                            port.StopBits = StopBits.One;//停止位位1
                            port.ReadTimeout = 1000;//读取超时1s
                            port.WriteTimeout = 1000;//写入超时1s
                            Task.Delay(100).Wait();
                            port.Open();
                            PLC1.Start(port);
                        }
                    }
                    catch
                    {
                        PLC1.Close();
                        ASName = null;
                        if (AmericanWindow.instance != null)
                            App.Current.Dispatcher.Invoke(AmericanWindow.instance.Close);
                        await Task.Delay(100);
                    }
                    await Task.Delay(100);
                }


            });


        }
        public static void PLC1Stop()
        {
            IsPLC1Start = false;
            PLC1.Close();
        }


        //启动PLC2
        public static void PLC2Start()
        {
            if (IsPLC2Start) return;
            IsPLC2Start = true;

            Task.Run(async () =>
            {

                while (IsPLC2Start)
                {
                    if (ENName == null)
                    {
                        Task.Run(Find);
                    }
                    SerialPort port = new SerialPort();
                    try
                    {
                        if (PLC2.Started == false && /*PLC2.Connected == false && */ENName != null)
                        {
                            port.PortName = ENName;
                            port.BaudRate = 19200;//设置波特率
                            port.Parity = Parity.None;//设置偶校验

                            port.StopBits = StopBits.One;//停止位位1
                            port.ReadTimeout = 1000;//读取超时1s
                            port.WriteTimeout = 1000;//写入超时1s
                            Task.Delay(100).Wait();
                            port.Open();
                            PLC2.Start(port);
                        }
                    }
                    catch
                    {
                        PLC2.Close();
                        ENName = null;
                        await Task.Delay(100);
                    }
                    await Task.Delay(100);
                }


            });
        }
        public static void PLC2Stop()
        {
            IsPLC2Start = false;
            PLC2.Close();
        }
        //温度采集模块
        public static void TemperatureStart()
        {
            if (IsTemperatureStart) return;
            IsTemperatureStart = true;
            Task.Run(async () =>
            {

                while (IsTemperatureStart)
                {
                    SerialPort port = new SerialPort();
                    try
                    {
                        if (Temperature.Started == false && /*Temperature.Connected == false &&*/ TemperatureName != null)
                        {
                            port.PortName = TemperatureName;
                            port.BaudRate = 9600;//设置波特率
                            port.Parity = Parity.None;//设置偶校验

                            port.StopBits = StopBits.One;//停止位位1
                            port.ReadTimeout = 1000;//读取超时1s
                            port.WriteTimeout = 1000;//写入超时1s
                            Task.Delay(100).Wait();
                            port.Open();
                            Temperature.Start(port);
                        }
                    }
                    catch
                    {
                        Temperature.Close();
                        TemperatureName = null;
                        await Task.Delay(100);
                    }
                }
                await Task.Delay(100);


            });
        }
        public static void TemperatureStop()
        {
            IsTemperatureStart = false;
            Temperature.Close();
        }
        /// <summary>
        /// 释放全部资源
        /// </summary>
        public static void Dispose()
        {
            IsPLCFind = false;
            PLC1Stop();
            PLC2Stop();
            TemperatureStop();

        }
        public static void Test()
        {
            SerialPort port = new SerialPort();

            port.PortName = "COM2";
            port.BaudRate = 19200;//设置波特率
            port.Parity = Parity.None;//设置偶校验

            port.StopBits = StopBits.One;//停止位位1
            port.ReadTimeout = 1000;//读取超时1s
            port.WriteTimeout = 1000;//写入超时1s
            Task.Delay(100).Wait();
            port.Open();
            PLC1.Start(port);
            Modbus_RTU_Num ID = new Modbus_RTU_Num(PLC1, Modbus_EasyAddress.D(20), Modbus_EasyAddress.DWordType) { AutoRead = true };
            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine(DateTime.Now.ToString() + "    " + ID.Value);
                    Task.Delay(100).Wait();
                }
            });
        }

    }
}
