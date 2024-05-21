using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomaticController.Device;

namespace AutomaticController.Windows.Demos.PLC通讯样例
{
    /// <summary>
    /// PLC管理，在这里创建全局的PLC相关对象
    /// </summary>
    public class DeviceLink
    {
        public static Modbus_RTU PLC { get; set; } = new Modbus_RTU(1);
        public static Modbus_RTU_Bit Y0 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XD3Address.Y(0));
        public static Modbus_RTU_Bit Y1 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XD3Address.Y(1));
        public static Modbus_RTU_Bit Y2 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XD3Address.Y(2));
        public static Modbus_RTU_Bit Y3 { get; set; } = new Modbus_RTU_Bit(PLC, Modbus_XD3Address.Y(3));
        public static Modbus_RTU_Num D0 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XD3Address.D(0), Modbus_XD3Address.WordType);
        public static Modbus_RTU_Num D2 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XD3Address.D(2), Modbus_XD3Address.WordType);
        public static Modbus_RTU_Num D4 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XD3Address.D(4), Modbus_XD3Address.WordType);
        public static Modbus_RTU_Num D6 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_XD3Address.D(6), Modbus_RTU_Type.Word);


        public static void PLCStart(string portName)
        {
            SerialPort port = new SerialPort(portName);//创建串口
            port.BaudRate = 19200;//设置波特率
            port.Parity = Parity.Even;//设置偶校验

            port.StopBits = StopBits.One;//停止位位1
            port.ReadTimeout = 1000;//读取超时1s
            port.WriteTimeout = 1000;//写入超时1s
            PLC.Start(port);//启动modubs通讯
        }

    }
}
