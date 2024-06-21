using AutomaticController.Device;
using System.IO.Ports;

namespace AutomaticController.Windows.FuJia.电机寿命老化测试.Datas
{
    public class PLC1
    {
        public static Modbus_RTU PLC { get; set; } = new Modbus_RTU(1);
        public static Modbus_RTU_Bit 检测OK1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(0));
        public static Modbus_RTU_Bit 检测NG1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1));
        public static Modbus_RTU_Bit 检测OK2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(2));
        public static Modbus_RTU_Bit 检测NG2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(3));
        public static Modbus_RTU_Bit 检测OK3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(4));
        public static Modbus_RTU_Bit 检测NG3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(5));
        public static Modbus_RTU_Bit 启动继电器1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(6));
        public static Modbus_RTU_Bit 启动继电器2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(7));
        public static Modbus_RTU_Bit 启动继电器3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(8));
        public static Modbus_RTU_Bit 记录1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(10)) { AutoRead = true };
        public static Modbus_RTU_Bit 记录2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(11)) { AutoRead = true };
        public static Modbus_RTU_Bit 记录3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(12)) { AutoRead = true };
        public static Modbus_RTU_Bit 产量清零 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(13));


        public static Modbus_RTU_Num 状态1 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(0), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num 状态2 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num 状态3 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(2), Modbus_EasyAddress.WordType) { AutoRead = true };
        public static Modbus_RTU_Num 瞬时流量1 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(10), Modbus_EasyAddress.FloatType) { Digits = 2 };
        public static Modbus_RTU_Num 累计流量1 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(12), Modbus_EasyAddress.FloatType) { Digits = 2 };
        public static Modbus_RTU_Num 测试时间1 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(14), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 结果流量1 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(16), Modbus_EasyAddress.FloatType) { AutoRead = true, Digits = 2 };

        public static Modbus_RTU_Num 瞬时流量2 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(20), Modbus_EasyAddress.FloatType) { Digits = 2 };
        public static Modbus_RTU_Num 累计流量2 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(22), Modbus_EasyAddress.FloatType) { Digits = 2 };
        public static Modbus_RTU_Num 测试时间2 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(24), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 结果流量2 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(26), Modbus_EasyAddress.FloatType) { AutoRead = true, Digits = 2 };

        public static Modbus_RTU_Num 瞬时流量3 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(30), Modbus_EasyAddress.FloatType) { Digits = 2 };
        public static Modbus_RTU_Num 累计流量3 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(32), Modbus_EasyAddress.FloatType) { Digits = 2 };
        public static Modbus_RTU_Num 测试时间3 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(34), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 结果流量3 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(36), Modbus_EasyAddress.FloatType) { AutoRead = true, Digits = 2 };




        public static Modbus_RTU_Num OK数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1000), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num NG数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1002), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 生产总数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1004), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 产品启动时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1010), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 检测时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1012), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 流量上限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1014), Modbus_EasyAddress.FloatType);
        public static Modbus_RTU_Num 流量下限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1016), Modbus_EasyAddress.FloatType);
        public static Modbus_RTU_Num 流量系数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1018), Modbus_EasyAddress.FloatType);


    }

    public class Devices
    {
        public static Modbus_RTU DODevice { get; set; } = new Modbus_RTU(1);
        public static SerialPort AIDevice { get; set; }
        public static double 采集电压1 { get; set; }
        public static double 采集电压2 { get; set; }
        public static double 采集电压3 { get; set; }
        public static double[] 采集电流 { get; set; } = new double[30];

        public static Modbus_RTU_Bit[] 继电器组 { get; set; } = new Modbus_RTU_Bit[] {
            new Modbus_RTU_Bit(DODevice, 0){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 1){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 2){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 3){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 4){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 5){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 6){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 7){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 8){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 9){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 10){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 11){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 12){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 13){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 14){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 15){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 16){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 17){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 18){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 19){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 20){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 21){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 22){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 23){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 24){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 25){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 26){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 27){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 28){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 29){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 30){AutoRead = true},
            new Modbus_RTU_Bit(DODevice, 31){AutoRead = true},
        };

        public static void ReadAI()
        {
            采集电压1 = GetDouble(Modbus_RTU.ReadData(AIDevice, 1, 0, 1));
            采集电压2 = GetDouble(Modbus_RTU.ReadData(AIDevice, 2, 0, 1));
            采集电压3 = GetDouble(Modbus_RTU.ReadData(AIDevice, 3, 0, 1));

            for (int i = 0; i < 采集电流.Length; i++)
            {
                采集电流[i] = GetDouble(Modbus_RTU.ReadData(AIDevice, (byte)(4 + i), 0, 1));
            }

        }
        private static double GetDouble(byte[] bytes)
        {
            UnitData AIData = new UnitData(255);

            if (bytes.Length > 0)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    AIData.Data[i] = bytes[i];
                }
            }
            return AIData.GetFloat_hlHL(0);
        }
    }

}
