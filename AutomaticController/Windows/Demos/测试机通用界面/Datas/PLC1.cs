using AutomaticController.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticController.Windows.Demos.测试机通用界面.Datas
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
        public static Modbus_RTU_Num 瞬时流量1 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(10), Modbus_EasyAddress.FloatType) { Digits = 2};
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
        public static Modbus_RTU_Num 结果流量3 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(36), Modbus_EasyAddress.FloatType) { AutoRead = true , Digits = 2 };




        public static Modbus_RTU_Num OK数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1000), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num NG数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1002), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 生产总数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1004), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 产品启动时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1010), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 检测时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1012), Modbus_EasyAddress.DWordType) { Scale = 0.001 };
        public static Modbus_RTU_Num 流量上限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1014), Modbus_EasyAddress.FloatType);
        public static Modbus_RTU_Num 流量下限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1016), Modbus_EasyAddress.FloatType);
        public static Modbus_RTU_Num 流量系数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1018), Modbus_EasyAddress.FloatType);


    }
}
