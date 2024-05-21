using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticController.Device
{
    public class Modbus_XD3Address
    {
        public const int M0 = 0;
        public const int X0 = 20480;
        public const int X10000 = 20736;
        public const int Y0 = 24576;
        public const int Y10000 = 24832;
        public const int T0 = 40960;
        public const int C0 = 45056;
        public const int HM0 = 49408;


        public const int D0 = 0;
        public const int TD0 = 32768;
        public const int CD0 = 36864;
        public const int HD0 = 41088;
        public const int FD0 = 50368;
        public const Modbus_RTU_Type WordType = Modbus_RTU_Type.Word;
        public const Modbus_RTU_Type DWordType = Modbus_RTU_Type.DWord_LH;
        public const Modbus_RTU_Type FloatType = Modbus_RTU_Type.Float_LH;

        public static int M(int num) => M0 + num;
        public static int X(int num) => X0 + num;
        public static int X_(int num) => X10000 + num;
        public static int Y(int num) => Y0 + num;
        public static int Y_(int num) => Y10000 + num;
        public static int T(int num) => T0 + num;
        public static int C(int num) => C0 + num;
        public static int HM(int num) => HM0 + num;

        public static int D(int num) => D0 + num;
        public static int TD(int num) => TD0 + num;
        public static int CD(int num) => CD0 + num;
        public static int HD(int num) => HD0 + num;
        public static int FD(int num) => FD0 + num;
    }
    public class Modbus_XCAddress
    {
        public const int M0 = 0;
        public const int X0 = 16384;
        public const int Y0 = 18432;
        public const int S0 = 20480;
        public const int M8000 = 24576;
        public const int T0 = 25600;
        public const int C0 = 27648;

        public const int D0 = 0;
        public const int TD0 = 12288;
        public const int CD0 = 14336;
        public const int D8000 = 16384;
        public const int FD0 = 18432;

        public const Modbus_RTU_Type WordType = Modbus_RTU_Type.Word;
        public const Modbus_RTU_Type DWordType = Modbus_RTU_Type.DWord_LH;
        public const Modbus_RTU_Type FloatType = Modbus_RTU_Type.Float_LH;
        public static int M(int num) => M0 + num;
        public static int X(int num) => X0 + num;
        public static int Y(int num) => Y0 + num;
        public static int S(int num) => S0 + num;
        public static int SM(int num) => M8000 + num;
        public static int T(int num) => T0 + num;
        public static int C(int num) => C0 + num;

        public static int D(int num) => D0 + num;
        public static int TD(int num) => TD0 + num;
        public static int CD(int num) => CD0 + num;
        public static int SD(int num) => D8000 + num;
        public static int FD(int num) => FD0 + num;

    }
    public class Modbus_EasyAddress
    {
        /*
        M0-M7999 8000 0x0000-0x1F3F (0-7999)
        B0-B32767 32768 0x3000-0xAFFF (12288-45055)
        S0-S4095 4096 0xE000-0xEFFF (57344-61439)
        X0-X1777 (8进制) 1024 0xF800-0xFBFF (63488-64511)
        Y0-Y1777 (8进制) 1024 0xFC00-0xFFFF (64512-65535)

        D0-D7999 8000 0x0000-0x1F3F (0-7999)
        R0-R32767 32768 0x3000-0xAFFF (12288-45055)
         */
        public const int M0 = 0;
        public const int B0 = 12288;
        public const int S0 = 57344;
        public const int X0 = 63488;
        public const int Y0 = 64512;

        public const int D0 = 0;
        public const int R0 = 12288;

        public const Modbus_RTU_Type WordType = Modbus_RTU_Type.Word;
        public const Modbus_RTU_Type DWordType = Modbus_RTU_Type.DWord_LH;
        public const Modbus_RTU_Type FloatType = Modbus_RTU_Type.Float_LH;
        public static int M(int num) => M0 + num;
        public static int X(int num) => X0 + num;
        public static int Y(int num) => Y0 + num;
        public static int S(int num) => S0 + num;
        public static int B(int num) => B0 + num;

        public static int D(int num) => D0 + num;
        public static int R(int num) => R0 + num;
    }


}
