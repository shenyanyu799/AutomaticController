using AutomaticController.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticController.Windows.Demos.测试机通用界面.Datas
{
    public class PLC1
    {
        public static Modbus_RTU PLC { get; set; } = new Modbus_RTU(1);

        public static Modbus_RTU_Bit 重量校准 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(0));
        public static Modbus_RTU_Bit 产量清零 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1));
        public static Modbus_RTU_Bit 运行按钮 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(10));
        public static Modbus_RTU_Bit 扫码完成 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(12));
        public static Modbus_RTU_Bit 产品拍照 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(13)) { AutoRead = true };
        public static Modbus_RTU_Bit 拍照OK { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(14));
        public static Modbus_RTU_Bit 拍照NG { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(15));
        public static Modbus_RTU_Bit 电机方向 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1000));
        public static Modbus_RTU_Bit 扫码启用 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1001));
        public static Modbus_RTU_Bit 拍照启用 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(1002));

        public static Modbus_RTU_Bit 数据记录 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.M(32)) { AutoRead = true };
        public static Modbus_RTU_Bit 升降气缸 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_EasyAddress.Y(4)) ;


        public static Modbus_RTU_Num OK数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1002), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num NG数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1004), Modbus_EasyAddress.DWordType);
        public static Num_Double 生产总数 { get; set; } = new Num_Double(() => (OK数.Value + NG数.Value));
        public static Modbus_RTU_Num 信号检测延时 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1010), Modbus_EasyAddress.WordType) { Scale = 0.1 };
        public static Modbus_RTU_Num 气缸升降时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1011), Modbus_EasyAddress.WordType) { Scale = 0.1 };
        public static Modbus_RTU_Num 推料动作时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1014), Modbus_EasyAddress.WordType) { Scale = 0.1 };
        public static Modbus_RTU_Num 重量下限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1020), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 重量上限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(1022), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 实时重量 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(4), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 检测重量 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(6), Modbus_EasyAddress.DWordType);
        public static Modbus_RTU_Num 检测流程 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(10), Modbus_EasyAddress.WordType);
        public static Modbus_RTU_Num 检测结果 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(11), Modbus_EasyAddress.WordType);

        //public static Modbus_RTU_Bit 重量校准 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(0));
        //public static Modbus_RTU_Bit 产量清零 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(1));
        //public static Modbus_RTU_Bit 运行按钮 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(10));
        //public static Modbus_RTU_Bit 扫码完成 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(12));
        //public static Modbus_RTU_Bit 产品拍照 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(13)){ AutoRead = true };
        //public static Modbus_RTU_Bit 拍照OK { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(14));
        //public static Modbus_RTU_Bit 拍照NG { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(15));
        //public static Modbus_RTU_Bit 电机方向 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(3000));
        //public static Modbus_RTU_Bit 扫码启用 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(3001));
        //public static Modbus_RTU_Bit 拍照启用 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(3002));
        //public static Modbus_RTU_Bit 数据记录 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.M(32));
        //public static Modbus_RTU_Bit 升降气缸 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XCAddress.Y(4));



        //public static Modbus_RTU_Num OK数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4002), Modbus_XCAddress.DWordType);
        //public static Modbus_RTU_Num NG数 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4004), Modbus_XCAddress.DWordType);
        //public static Num_Double 生产总数 { get; set; } = new Num_Double(() => (OK数.Value + NG数.Value));
        //public static Modbus_RTU_Num 信号检测延时 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4010), Modbus_XCAddress.WordType) { Scale = 0.1 };
        //public static Modbus_RTU_Num 气缸升降时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4011), Modbus_XCAddress.WordType) { Scale = 0.1 };
        //public static Modbus_RTU_Num 推料动作时间 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4014), Modbus_XCAddress.WordType) { Scale = 0.1 };
        //public static Modbus_RTU_Num 重量下限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4020), Modbus_XCAddress.DWordType);
        //public static Modbus_RTU_Num 重量上限 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4022), Modbus_XCAddress.DWordType);
        //public static Modbus_RTU_Num 实时重量 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(4), Modbus_XCAddress.DWordType);
        //public static Modbus_RTU_Num 检测重量 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(6), Modbus_XCAddress.DWordType);
        //public static Modbus_RTU_Num 检测流程 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(10), Modbus_XCAddress.WordType);
        //public static Modbus_RTU_Num 检测结果 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XCAddress.D(11), Modbus_XCAddress.WordType);



    }
}
