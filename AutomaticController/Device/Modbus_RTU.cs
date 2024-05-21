using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using AutomaticController.Function;
using static System.Collections.Specialized.BitVector32;

namespace AutomaticController.Device
{
    public class Modbus_RTU
    {
        /// <summary>
        /// 串口单元
        /// </summary>
        public SerialPort Serial { get; set; }

        public BitArray Bits { get; set; }
        public UnitData Data1 { get; set; }
        /// <summary>
        /// 连续读取间隙,位数据判断时*16；这个参数对读写速度有巨大影响
        /// </summary>
        public int ContinuousSpac { get; set; } = 3;
        /// <summary>
        /// 最大读取数量,位数据判断时*16；这个参数对读写速度有巨大影响
        /// </summary>
        public int MaxCount { get; set; } = 32;
        /// <summary>
        /// 启动状态
        /// </summary>
        public bool Started { get; private set; }
        /// <summary>
        /// 通讯已连接，从建立第一次通讯开始到通讯断开
        /// </summary>
        public bool Connected { get; private set; }
        /// <summary>
        /// 站号
        /// </summary>
        public byte ModbusAddress { get; set; }
        /// <summary>
        /// 完成一轮通讯所需的时间
        /// </summary>
        public double CommunicationDelay { get; private set; }
        /// <summary>
        /// 读取超时
        /// </summary>
        public int ReadTimeout { get; set; } = 3000;
        public List<IModbus_RTU_Unit> Units { get; set; }
        public Modbus_RTU(byte modbusAddress, int bitSize = ushort.MaxValue, int dataSize = ushort.MaxValue)
        {
            ModbusAddress = modbusAddress;
            Units = new List<IModbus_RTU_Unit>();
            Bits = new BitArray(bitSize);
            Data1 = new UnitData(dataSize);
        }
        public void Start()
        {
            Start(Serial);
        }
        public void Start(SerialPort serialPort)
        {
            if (serialPort?.IsOpen == false)
            {
                try
                {
                    serialPort.Open();
                }
                catch (Exception ex)
                {
                    LogManager.LogShow(ex.Message, LogLevel.Error);
                    return;
                }
            }
            Serial = serialPort;
            Started = true;
            DateTime lastTime = DateTime.Now;
            uint RWCount = 0;
            uint oldCount = 0;
            //连接前清除所有写入请求
            foreach (var item in Units)
            {
                item.RequestWrite = false;
            }
            Task.Run(() =>
            {
                while (Started)
                {
                    int rt = serialPort.ReadTimeout + ReadTimeout;
                    if ((DateTime.Now - lastTime).TotalMilliseconds > rt)
                    {
                        try
                        {
                            LogManager.Log("Modbus-RTU通讯超时", LogLevel.Error);
                            this.Close();
                        }
                        catch { }
                    }
                    Task.Delay(100).Wait();
                }
            });
            Task.Run(async () =>
            {
                try
                {
                    List<int> writeBits = new List<int>();
                    List<int> readBits = new List<int>();
                    List<int> writeDatas = new List<int>();
                    List<int> readDatas = new List<int>();
                    List<IModbus_RTU_Unit> writeBits_unit = new List<IModbus_RTU_Unit>();
                    List<IModbus_RTU_Unit> readBits_unit = new List<IModbus_RTU_Unit>();
                    List<IModbus_RTU_Unit> writeDatas_unit = new List<IModbus_RTU_Unit>();
                    List<IModbus_RTU_Unit> readDatas_unit = new List<IModbus_RTU_Unit>();
                    int errorCount = 0;
                    //await Task.Delay(1000);
                    while (Started)
                    {
                        
                        if (errorCount >= 3)
                        {
                            throw new Exception("ModBUs-RTU通讯异常");
                        }
                        DateTime st = DateTime.Now;
                        writeBits.Clear();
                        readBits.Clear();
                        writeDatas.Clear();
                        readDatas.Clear();
                        writeBits_unit.Clear();
                        readBits_unit.Clear();
                        writeDatas_unit.Clear();
                        readDatas_unit.Clear();
                        int ccc = 0;
                        //遍历所有地址，确定是否需要读写
                        foreach (var unit in Units)
                        {
                            if (unit.RequestWrite)//可以写入
                            {
                                switch (unit.UnitType)
                                {
                                    case Modbus_RTU_Type.Bit:
                                        writeBits.Add(unit.DataAddress);
                                        writeBits_unit.Add(unit);
                                        ccc++;
                                        break;
                                    case Modbus_RTU_Type.Word:
                                    case Modbus_RTU_Type.Word_LH:
                                        writeDatas.Add(unit.DataAddress);
                                        writeDatas_unit.Add(unit);
                                        ccc++;
                                        break;
                                    case Modbus_RTU_Type.DWord:
                                    case Modbus_RTU_Type.DWord_LH:
                                    case Modbus_RTU_Type.Float:
                                    case Modbus_RTU_Type.Float_LH:
                                        writeDatas.Add(unit.DataAddress);
                                        writeDatas.Add(unit.DataAddress + 1);
                                        writeDatas_unit.Add(unit);
                                        ccc++;
                                        break;
                                }
                            }
                            if (unit.AllowRead)//可以读取
                            {
                                switch (unit.UnitType)
                                {
                                    case Modbus_RTU_Type.Bit:
                                        readBits.Add(unit.DataAddress);
                                        readBits_unit.Add(unit);
                                        ccc++;
                                        break;
                                    case Modbus_RTU_Type.Word:
                                    case Modbus_RTU_Type.Word_LH:
                                        readDatas.Add(unit.DataAddress);
                                        readDatas_unit.Add(unit);
                                        ccc++;
                                        break;
                                    case Modbus_RTU_Type.DWord:
                                    case Modbus_RTU_Type.DWord_LH:
                                    case Modbus_RTU_Type.Float:
                                    case Modbus_RTU_Type.Float_LH:
                                        readDatas.Add(unit.DataAddress);
                                        readDatas.Add(unit.DataAddress + 1);
                                        readDatas_unit.Add(unit);
                                        ccc++;
                                        break;
                                }
                            }
                        }
                        if (ccc == 0)//如果不需要读写则延时后进入下一个循环
                        {
                            await Task.Delay(5);
                            continue;
                        }
                        //排序
                        writeBits.Sort();
                        readBits.Sort();
                        writeDatas.Sort();
                        readDatas.Sort();
                        //读取.............................
                        //读Bits
                        try
                        {
                            if (readBits.Count > 0)
                            {
                                ReadBits(readBits);
                            }
                        }
                        catch
                        {
                            await Task.Delay(10);
                            errorCount++;
                            continue;
                        }                        //读取完成

                        foreach (var item in readBits_unit)
                        {
                            item.ReadFinish();
                        }
                        //读Data
                        try
                        {
                            if (readDatas.Count > 0)
                            {
                                ReadData(readDatas);
                            }
                        }
                        catch
                        {
                            await Task.Delay(10);
                            errorCount++;
                            continue;
                        }                        //读取完成

                        foreach (var item in readDatas_unit)
                        {
                            item.ReadFinish();
                        }

                        //写入。。。。。。。。。。。。。。
                        //写入前再次写入
                        foreach (var item in writeBits_unit)
                        {
                            item.SetRemoteValue?.Invoke();
                        }
                        //写Bit
                        try
                        {
                            if (writeBits.Count > 0)
                            {
                                WriteBits(writeBits);
                            }
                        }
                        catch
                        {
                            await Task.Delay(10);
                            errorCount++;
                            continue;
                        }                        //写入完成
                        foreach (var item in writeBits_unit)
                        {
                            item.WriteFinish();
                        }
                        //写入前再次写入
                        foreach (var item in writeDatas_unit)
                        {
                            item.SetRemoteValue?.Invoke();
                        }
                        //写Data
                        try
                        {
                            if (writeDatas.Count > 0)
                            {
                                WriteData(writeDatas);
                            }
                        }
                        catch
                        {
                            await Task.Delay(10);
                            errorCount++;
                            continue;
                        }
                        //写入完成
                        foreach (var item in writeDatas_unit)
                        {
                            item.WriteFinish();
                        }

                        

                        errorCount = 0;
                        Connected = true;
                        RWCount++;//新增读写次数，方便计算通讯延时
                        double dt = (DateTime.Now - lastTime).TotalSeconds;
                        uint countC = RWCount - oldCount;
                        if (dt >= 1)
                        {
                            CommunicationDelay = (1000 / ((countC) / dt));
                            //Console.WriteLine($"{(countC)} : {CommunicationDelay.ToString("0.00") + "ms"}");
                            lastTime = DateTime.Now;
                            oldCount = RWCount;
                        }
                        //if (countC > 10000) Task.Delay(1).Wait();
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Log(ex.Message, LogLevel.Error);
                    Close();
                    await Task.Delay(1000);
                    return;
                }
            });
        }
        public void Close()
        {
            Started = false;
            Connected = false;
            try { Serial?.Close(); Serial?.Dispose(); } catch { }
        }

        /// <summary>
        /// 读取所有地址的位状态
        /// </summary>
        /// <param name="adds"></param>
        private void ReadBits(List<int> list)
        {
            int startAdd = 0;
            int rwLen = 0;
            int endAdd = 0;

            int cspac = ContinuousSpac * 16;
            int maxcount = MaxCount * 16;

            //bit读取
            for (int i = 0; i < list.Count; i++)
            {
                if (rwLen == 0)
                {
                    startAdd = list[i];
                    endAdd = startAdd;
                }
                //间距大于设定值
                if ((list[i] - endAdd) >= cspac)
                {
                    ReadBits(startAdd, rwLen);
                    startAdd = list[i];
                }
                if (rwLen >= maxcount)
                {
                    ReadBits(startAdd, rwLen);
                    startAdd = list[i];
                }
                endAdd = list[i];
                rwLen = endAdd - startAdd + 1;
            }
            if (rwLen > 0)
            {
                int s = startAdd;
                int l = rwLen;
                ReadBits(startAdd, rwLen);
            }

        }
        /// <summary>
        /// 读取从开始地址到指定长度的位
        /// </summary>
        /// <param name="startAdd"></param>
        /// <param name="len"></param>
        public void ReadBits(int startAdd, int len)
        {

            byte[] bt = ReadBit(Serial, ModbusAddress, (ushort)startAdd, (ushort)len);

            BitArray array = new BitArray(bt);
            for (int i = 0; i < len; i++)
            {
                Bits[startAdd + i] = array[i];
            }

        }
        /// <summary>
        /// 写入所有地址位状态
        /// </summary>
        /// <param name="list"></param>
        private void WriteBits(List<int> list)
        {
            int startAdd = 0;
            int rwLen = 0;
            int endAdd = 0;

            int cspac = ContinuousSpac * 16;
            int maxcount = MaxCount * 16;

            //bit写入
            for (int i = 0; i < list.Count; i++)
            {
                if (rwLen == 0)
                {
                    startAdd = list[i];
                    endAdd = startAdd;
                }
                //间距大于设定值
                if ((list[i] - endAdd) >= cspac)
                {
                    WriteBits(startAdd, rwLen);
                    startAdd = list[i];
                }
                if (rwLen >= maxcount)
                {
                    WriteBits(startAdd, rwLen);
                    startAdd = list[i];
                }
                endAdd = list[i];
                rwLen = endAdd - startAdd + 1;
            }
            if (rwLen > 0)
            {
                int s = startAdd;
                int l = rwLen;
                WriteBits(startAdd, rwLen);
            }
        }

        /// <summary>
        /// 写入从开始地址到指定长度的位
        /// </summary>
        /// <param name="startAdd"></param>
        /// <param name="len"></param>
        public void WriteBits(int startAdd, int len)
        {
            if(len == 1)//如果只有一个位
            {
                if (Bits[startAdd])
                {
                    SetBit(Serial, ModbusAddress, (ushort)startAdd);
                }
                else
                {
                    RstBit(Serial, ModbusAddress, (ushort)startAdd);
                }
                return;
            }
            int l = 0;
            if(len % 8 == 0)
            {
                l = len;
            }
            else
            {
                l = len / 8 + 1;
            }
            byte[] bs = new byte[l];
            int n = 0;
            for (int i = 0; i < len; i++) 
            {
                int c = i % 8;
                if (Bits[startAdd + i])
                {
                    bs[n] = (byte)(bs[n] | (1 << c));
                }
                else
                {
                    bs[n] = (byte)(bs[n] & (~(1 << c)));
                }
                if (c == 7 )n++;
            }
            WriteBits(Serial, ModbusAddress, (ushort)startAdd, (ushort)len, bs);

        }
        /// <summary>
        /// 读取所有地址的数据
        /// </summary>
        /// <param name="adds"></param>
        private void ReadData(List<int> list)
        {
            int startAdd = 0;
            int rwLen = 0;
            int endAdd = 0;

            int cspac = ContinuousSpac * 16;
            int maxcount = MaxCount * 16;

            //bit读取
            for (int i = 0; i < list.Count; i++)
            {
                if (rwLen == 0)
                {
                    startAdd = list[i];
                    endAdd = startAdd;
                }
                //间距大于设定值
                if ((list[i] - endAdd) >= cspac)
                {
                    ReadData(startAdd, rwLen);
                    startAdd = list[i];
                }
                if (rwLen >= maxcount)
                {
                    ReadData(startAdd, rwLen);
                    startAdd = list[i];
                }
                endAdd = list[i];
                rwLen = endAdd - startAdd + 1;
            }
            if (rwLen > 0)
            {
                int s = startAdd;
                int l = rwLen;
                ReadData(startAdd, rwLen);
            }
        }
        /// <summary>
        /// 读取从开始地址到指定长度的数据
        /// </summary>
        /// <param name="startAdd"></param>
        /// <param name="len"></param>
        public void ReadData(int startAdd, int len)
        {
            byte[] bytes = ReadData(Serial, ModbusAddress, (ushort)startAdd, (ushort)len);
            bytes.CopyTo(Data1.Data, startAdd * 2);
        }

        /// <summary>
        /// 写入所有地址数据
        /// </summary>
        /// <param name="list"></param>
        private void WriteData(List<int> list)
        {
            int startAdd = 0;
            int rwLen = 0;
            int endAdd = 0;

            int cspac = ContinuousSpac * 16;
            int maxcount = MaxCount * 16;

            //bit写入
            for (int i = 0; i < list.Count; i++)
            {
                if (rwLen == 0)
                {
                    startAdd = list[i];
                    endAdd = startAdd;
                }
                //间距大于设定值
                if ((list[i] - endAdd) >= cspac)
                {
                    WriteData(startAdd, rwLen);
                    startAdd = list[i];
                }
                if (rwLen >= maxcount)
                {
                    WriteData(startAdd, rwLen);
                    startAdd = list[i];
                }
                endAdd = list[i];
                rwLen = endAdd - startAdd + 1;
            }
            if (rwLen > 0)
            {
                int s = startAdd;
                int l = rwLen;
                WriteData(startAdd, rwLen);
            }
        }
        /// <summary>
        /// 写入从开始地址到指定长度的数据
        /// </summary>
        /// <param name="startAdd"></param>
        /// <param name="len"></param>
        public void WriteData(int startAdd, int len)
        {
            int i = startAdd * 2;
            if (len == 1)
            {
                WriteData(Serial, ModbusAddress, (ushort)startAdd, Data1[i], Data1[i + 1]);
            }
            else
            {
                WriteDatas(Serial, ModbusAddress, (ushort)startAdd, Data1.Data, (byte)(len * 2), i);
            }
        }

        #region 通讯指令


        /// <summary>
        /// 置位单个位状态 0x05
        /// 读取成功返回true
        /// </summary>
        /// <param name="Address">站号</param>
        /// <param name="bitAddress">地址</param>
        /// <returns></returns>
        public static void SetBit(SerialPort serialPort, byte Address, ushort bitAddress)
        {
            byte[] code = new byte[8];
            code[0] = Address;
            code[1] = 0x05;
            byte[] sa = BitConverter.GetBytes(bitAddress);
            code[2] = sa[1];
            code[3] = sa[0];
            code[4] = 0xFF;
            code[5] = 0;
            byte[] crc = CRC16(code, 6);
            code[6] = crc[1];
            code[7] = crc[0];

            serialPort.ReadExisting();
            serialPort.Write(code, 0, code.Length);

            try
            {
                //校验返回相同报文
                for (int i = 0; i < code.Length; i++)
                {
                    if (serialPort.ReadByte() != code[i])
                    {
                        throw new Exception("Modbus_RTU置位Bit校验异常");
                    }
                }
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU置位Bit异常");
            }
        }
        /// <summary>
        /// 复位单个位状态 0x05
        /// 读取成功返回true
        /// </summary>
        /// <param name="Address">站号</param>
        /// <param name="bitAddress">地址</param>
        /// <returns></returns>
        public static void RstBit(SerialPort serialPort, byte Address, ushort bitAddress)
        {
            byte[] code = new byte[8];
            code[0] = Address;
            code[1] = 0x05;
            byte[] sa = BitConverter.GetBytes(bitAddress);
            code[2] = sa[1];
            code[3] = sa[0];
            code[4] = 0;
            code[5] = 0;
            byte[] crc = CRC16(code, 6);
            code[6] = crc[1];
            code[7] = crc[0];
            serialPort.ReadExisting();
            serialPort.Write(code, 0, code.Length);

            try
            {
                //校验返回相同报文
                for (int i = 0; i < code.Length; i++)
                {
                    if (serialPort.ReadByte() != code[i])
                    {
                        throw new Exception("Modbus_RTU复位Bit校验异常");
                    }
                }
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU置位Bit异常");
            }
        }
        /// <summary>
        /// 写入多个位 0x0F
        /// 读取成功返回true
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="Address"></param>
        /// <param name="bitAddress"></param>
        /// <param name="bitCount"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static void WriteBits(SerialPort serialPort, byte Address, ushort bitAddress, ushort bitCount, byte[] datas)
        {
            serialPort.ReadExisting();

            int c = bitCount / 8;//字节数
            if (c * 8 < bitCount)//如果字节数不够加1
            {
                c++;
            }
            //如果字节数大于数据数量
            if(c > datas.Length)
            {
                c = datas.Length;
            }

            byte[] code = new byte[9 + c];
            byte[] readCode = new byte[8];

            code[0] = Address;
            code[1] = 0x0F;
            byte[] sa = BitConverter.GetBytes(bitAddress);//寄存器地址
            code[2] = sa[1];
            code[3] = sa[0];
            byte[] dc = BitConverter.GetBytes(bitCount);//寄存器数量
            code[4] = dc[1];
            code[5] = dc[0];

            readCode[0] = code[0];
            readCode[1] = code[1];
            readCode[2] = code[2];
            readCode[3] = code[3];
            readCode[4] = code[4];
            readCode[5] = code[5];



            code[6] = (byte)c;//字节数量

            for (int i = 0; i < c; i++)
            {
                code[i + 7] = datas[i];
            }



            byte[] crc = CRC16(code, code.Length - 2);
            code[code.Length - 2] = crc[1];
            code[code.Length - 1] = crc[0];

            serialPort.Write(code, 0, code.Length);



            crc = CRC16(readCode, 6);
            readCode[6] = crc[1];
            readCode[7] = crc[0];

            try
            {
                //校验返回相同报文
                for (int i = 0; i < readCode.Length; i++)
                {
                    if (serialPort.ReadByte() != readCode[i])
                    {
                        throw new Exception("Modbus_RTU写入Bits校验异常");
                    }
                }
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU写入Bits异常");
            }

        }
        /// <summary>
        /// 读取指定数量位
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="Address"></param>
        /// <param name="bitAddress"></param>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        public static byte[] ReadBit(SerialPort serialPort, byte Address, ushort bitAddress, ushort bitCount)
        {
            serialPort.ReadExisting();

            byte[] code = new byte[8];
            int c = bitCount / 8;//字节数
            if (c * 8 < bitCount)//如果字节数不够加1
            {
                c++;
            }
            byte[] states = new byte[c];
            code[0] = Address;
            code[1] = 0x01;
            byte[] sa = BitConverter.GetBytes(bitAddress);//寄存器地址
            code[2] = sa[1];
            code[3] = sa[0];
            byte[] dc = BitConverter.GetBytes(bitCount);//寄存器数量
            code[4] = dc[1];
            code[5] = dc[0];
            byte[] crc = CRC16(code, 6);
            code[6] = crc[1];
            code[7] = crc[0];

            serialPort.Write(code, 0, code.Length);

            try
            {

                //校验返回相同报文
                if (serialPort.ReadByte() != code[0]) throw new Exception("Modbus_RTU读取Bit异常");
                if (serialPort.ReadByte() != code[1]) throw new Exception("Modbus_RTU读取Bit异常");
                if (serialPort.ReadByte() != c) throw new Exception("Modbus_RTU读取Bit异常");

                for (int i = 0; i < c; i++)
                {
                    states[i] = (byte)serialPort.ReadByte();
                }
                serialPort.ReadByte();//读crc
                serialPort.ReadByte();//读crc
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU读取Bit异常");
            }

            return states;
        }
        /// <summary>
        /// 写入单个数据
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="Address"></param>
        /// <param name="dataAddress"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void WriteData(SerialPort serialPort, byte Address, ushort dataAddress, byte Hdata, byte Ldata)
        {
            serialPort.ReadExisting();
            byte[] code = new byte[8];
            code[0] = Address;
            code[1] = 0x06;
            byte[] sa = BitConverter.GetBytes(dataAddress);
            code[2] = sa[1];
            code[3] = sa[0];

            code[4] = Hdata;
            code[5] = Ldata;
            byte[] crc = CRC16(code, 6);
            code[6] = crc[1];
            code[7] = crc[0];
            serialPort.Write(code, 0, code.Length);

            try
            {
                //校验返回相同报文
                for (int i = 0; i < code.Length; i++)
                {
                    if (serialPort.ReadByte() != code[i])
                    {
                        throw new Exception("Modbus_RTU写入Data校验异常");
                    }
                }
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU写入Data异常");
            }

        }
        /// <summary>
        /// 写入多个数据
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="Address"></param>
        /// <param name="dataAddress"></param>
        /// <param name="datas"></param>
        /// <param name="bytecount"></param>
        /// <param name="byteoffs"></param>
        /// <returns></returns>
        public static void WriteDatas(SerialPort serialPort, byte Address, ushort dataAddress, byte[] datas ,byte bytecount,int byteoffs)
        {
            serialPort.ReadExisting();

            int len = bytecount / 2;
            byte[] code = new byte[9 + bytecount];
            byte[] readCode = new byte[8];

            code[0] = Address;
            code[1] = 0x10;
            byte[] sa = BitConverter.GetBytes(dataAddress);
            code[2] = sa[1];
            code[3] = sa[0];
            byte[] da = BitConverter.GetBytes(len);
            code[4] = da[1];
            code[5] = da[0];
            code[6] = bytecount;

            readCode[0] = code[0];
            readCode[1] = code[1];
            readCode[2] = code[2];
            readCode[3] = code[3];
            readCode[4] = code[4];
            readCode[5] = code[5];


            for (int i = 0; i < bytecount; i++)
            {
                code[i + 7] = datas[i + byteoffs];
            }

            byte[] crc = CRC16(code, code.Length - 2);
            code[code.Length - 2] = crc[1];
            code[code.Length - 1] = crc[0];
            serialPort.Write(code, 0, code.Length);

            crc = CRC16(readCode, 6);
            readCode[6] = crc[1];
            readCode[7] = crc[0];

            try
            {
                //校验返回相同报文
                for (int i = 0; i < readCode.Length; i++)
                {
                    if (serialPort.ReadByte() != readCode[i])
                    {
                        throw new Exception("Modbus_RTU写入Datas校验异常");
                    }
                }
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU写入Datas异常");
            }
        }
        /// <summary>
        /// 读取数据,如果读取失败则返回null
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="Address"></param>
        /// <param name="dataAddress"></param>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        public static byte[] ReadData(SerialPort serialPort, byte Address, ushort dataAddress, ushort dataCount)
        {
            serialPort.ReadExisting();

            byte[] code = new byte[8];

            code[0] = Address;
            code[1] = 0x03;
            byte[] sa = BitConverter.GetBytes(dataAddress);
            code[2] = sa[1];
            code[3] = sa[0];
            byte[] da = BitConverter.GetBytes(dataCount);
            code[4] = da[1];
            code[5] = da[0];
            byte[] crc = CRC16(code, 6);
            code[6] = crc[1];
            code[7] = crc[0];
            serialPort.Write(code, 0, code.Length);


            byte[] states = new byte[0];
            try
            {
                int len = 0;
                //校验返回相同报文
                if (serialPort.ReadByte() != code[0]) throw new Exception("Modbus_RTU读取Data异常");
                if (serialPort.ReadByte() != code[1]) throw new Exception("Modbus_RTU读取Data异常");
                len = serialPort.ReadByte();
                states = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    states[i] = (byte)serialPort.ReadByte();
                }
                serialPort.ReadByte();//读crc
                serialPort.ReadByte();//读crc
            }
            catch
            {
                serialPort.ReadExisting();
                throw new Exception("Modbus_RTU读取Data异常");
            }

            return states;
        }

        /// <summary>
        /// CRC校验，参数data为byte数组
        /// </summary>
        /// <param name="data">校验数据，字节数组</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRC16(byte[] data, int length = 0)
        {
            //crc计算赋初始值
            int crc = 0xffff;
            for (int i = 0; i < length; i++)
            {
                crc = crc ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            //CRC寄存器的高低位进行互换
            byte[] crc16 = new byte[2];
            //CRC寄存器的高8位变成低8位，
            crc16[0] = (byte)((crc >> 8) & 0xff);
            //CRC寄存器的低8位变成高8位
            crc16[1] = (byte)(crc & 0xff);
            return crc16;
        }
        #endregion
    }
    public enum Modbus_RTU_Type
    {
        Bit,
        Word,
        Word_LH,
        DWord,
        DWord_LH,
        Float,
        Float_LH
    }

    public interface IModbus_RTU_Unit
    {
        Modbus_RTU_Type UnitType { get; set; }
        /// <summary>
        /// 数据地址
        /// </summary>
        int DataAddress { get; set; }
        /// <summary>
        /// 自动读取
        /// </summary>
        bool AutoRead { get; set; }
        /// <summary>
        /// 请求读取
        /// </summary>
        bool RequestRead { get; set; }
        /// <summary>
        /// 请求写入
        /// </summary>
        bool RequestWrite { get; set; }
        /// <summary>
        /// 读取间隔 单位 s
        /// </summary>
        double ReadInterval { get; set; }
        /// <summary>
        /// 通讯前再次设定值
        /// </summary>
        Action SetRemoteValue { get; set; }
        /// <summary>
        /// 最后一次读写时间
        /// </summary>
        DateTime LastTime { get; set; }
        Modbus_RTU RTU { get; set; }
        /// <summary>
        /// 允许发送读取报文
        /// </summary>
        /// <returns></returns>
        bool AllowRead { get; }
        /// <summary>
        /// 读取完成后触发的事件
        /// </summary>
        event Action<IModbus_RTU_Unit> ReadFinishEvent;
        /// <summary>
        /// 写入完成后触发的事件
        /// </summary>
        event Action<IModbus_RTU_Unit> WriteFinishEvent;
        /// <summary>
        /// 读取完成
        /// </summary>
        void ReadFinish();
        /// <summary>
        /// 写入完成
        /// </summary>
        void WriteFinish();
        /// <summary>
        /// 立即读取
        /// </summary>
        /// <param name="count">读取数量</param>
        void ExecuteRead();
        /// <summary>
        /// 立即写入
        /// </summary>
        /// <param name="count">写入数量</param>
        void ExecuteWrite();
    }
    public class Modbus_RTU_Bit : IModbus_RTU_Unit, IBit, ITryText
    {
        public Modbus_RTU_Type UnitType { get; set; } = Modbus_RTU_Type.Bit;
        public int DataAddress { get; set; }
        public bool RequestRead { get; set; }
        public bool RequestWrite { get; set; }
        public bool AutoRead { get; set; } = false;
        public double ReadInterval { get; set; } = 0.1;
        public DateTime LastTime { get; set; }
        public Modbus_RTU RTU { get; set; }
        public bool AllowRead => (AutoRead || RequestRead) && ((DateTime.Now - LastTime).TotalSeconds >= ReadInterval && RequestWrite == false);
        public Action SetRemoteValue { get; set; }
        public bool Value
        {
            get
            {
                return RTU.Bits[DataAddress];
            }
            set
            {
                RequestWrite = true;
                RTU.Bits[DataAddress] = value;
                SetRemoteValue = () => RTU.Bits[DataAddress] = value;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPort">串口资源</param>
        /// <param name="block">数据映射块</param>
        /// <param name="modbusAddress">modbus站号</param>
        /// <param name="dataAddress">数据地址</param>
        public Modbus_RTU_Bit(Modbus_RTU Rtu, int dataAddress)
        {
            RTU = Rtu;
            DataAddress = dataAddress;
            ReadInterval = 0.1;
            Rtu.Units.Add(this);
        }

        public event Action<IModbus_RTU_Unit> ReadFinishEvent;
        public event Action<IModbus_RTU_Unit> WriteFinishEvent;
        public void ReadFinish()
        {
            RequestRead = false;
            LastTime = DateTime.Now;
            ReadFinishEvent?.Invoke(this);
        }
        public void WriteFinish()
        {
            RequestWrite = false;
            LastTime = DateTime.Now;
            WriteFinishEvent?.Invoke(this);
        }


        public void ExecuteRead()
        {
            var rv = Modbus_RTU.ReadBit(RTU.Serial, RTU.ModbusAddress, (ushort)DataAddress, 1);
            RTU.Bits[DataAddress ] = rv[0].GetBit(0);
            LastTime = DateTime.Now;
            RequestRead = false;
        }
        public void ExecuteWrite()
        {
            if (Value)
            {
                Modbus_RTU.SetBit(RTU.Serial, RTU.ModbusAddress, (ushort)(DataAddress));
            }
            else
            {
                Modbus_RTU.RstBit(RTU.Serial, RTU.ModbusAddress, (ushort)(DataAddress));
            }
            LastTime = DateTime.Now;
            RequestWrite = false;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public void Parse(string text)
        {
            bool d = false;
            if (bool.TryParse(text, out d))
            {
                Value = d;
            }
        }
    }
    public class Modbus_RTU_Num : IModbus_RTU_Unit, INum, ITryText
    {
        public Modbus_RTU_Type UnitType { get; set; }
        public int DataAddress { get; set; }
        public bool RequestRead { get; set; }
        public bool RequestWrite { get; set; }
        public bool AutoRead { get; set; } = false;
        public double ReadInterval { get; set; } = 0.1;
        public DateTime LastTime { get; set; }
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int Digits { get; set; } = 3;
        /// <summary>
        /// 缩放倍数
        /// </summary>
        public double Scale { get; set; } = 1;
        public Modbus_RTU RTU { get; set; }
        public bool AllowRead => (AutoRead || RequestRead) && ((DateTime.Now - LastTime).TotalSeconds >= ReadInterval && RequestWrite == false);
        public Action SetRemoteValue { get; set; }

        public bool this[int index]
        {
            get
            {
                int n = index / 8;
                int i = index % 8;
                int num = DataAddress * 2;
                switch (UnitType)
                {
                    case Modbus_RTU_Type.Word:
                        if (n == 0)
                        {
                            n = 1;
                            break;
                        }
                        if (n == 1) n = 0;
                        break;
                    case Modbus_RTU_Type.Word_LH:
                        break;
                    case Modbus_RTU_Type.DWord:
                    case Modbus_RTU_Type.Float:
                        if (n == 0)
                        {
                            n = 3;
                            break;
                        }
                        if (n == 1)
                        {
                            n = 2;
                            break;
                        }
                        if (n == 2)
                        {
                            n = 1;
                            break;
                        }
                        if (n == 3)
                        {
                            n = 0;
                        }
                        break;
                    case Modbus_RTU_Type.DWord_LH:
                    case Modbus_RTU_Type.Float_LH:
                        if (n == 3)
                        {
                            n = 2;
                            break;
                        }
                        if (n == 2)
                        {
                            n = 3;
                            break;
                        }
                        if (n == 1)
                        {
                            n = 0;
                            break;
                        }
                        if (n == 0) n = 1;
                        break;
                }
                return RTU.Data1.GetBit(num + n, i);
            }
            set
            {
                RequestWrite = true;
                int n = index / 8;
                int i = index % 8;
                int num = DataAddress * 2;
                switch (UnitType)
                {
                    case Modbus_RTU_Type.Word:
                        if (n == 0)
                        {
                            n = 1;
                            break;
                        }
                        if (n == 1) n = 0;
                        break;
                    case Modbus_RTU_Type.Word_LH:
                        break;
                    case Modbus_RTU_Type.DWord:
                    case Modbus_RTU_Type.Float:
                        if (n == 0)
                        {
                            n = 3;
                            break;
                        }
                        if (n == 1)
                        {
                            n = 2;
                            break;
                        }
                        if (n == 2)
                        {
                            n = 1;
                            break;
                        }
                        if (n == 3)
                        {
                            n = 0;
                        }
                        break;
                    case Modbus_RTU_Type.DWord_LH:
                    case Modbus_RTU_Type.Float_LH:
                        if (n == 3)
                        {
                            n = 2;
                            break;
                        }
                        if (n == 2)
                        {
                            n = 3;
                            break;
                        }
                        if (n == 1)
                        {
                            n = 0;
                            break;
                        }
                        if (n == 0) n = 1;
                        break;
                }

                RTU.Data1.SetBit(num + n, i, value); 

                SetRemoteValue = () => this[index] = value;
            }
        }

        public double Value
        {
            get
            {
                double r = 0;
                int num = DataAddress * 2;
                switch (UnitType)
                {
                    case Modbus_RTU_Type.Word:
                        r = RTU.Data1.GetInt16_HL(num); break;
                    case Modbus_RTU_Type.Word_LH:
                        r = RTU.Data1.GetInt16_LH(num); break;
                    case Modbus_RTU_Type.DWord:
                        r = RTU.Data1.GetInt32_HLhl(num); 
                        break;
                    case Modbus_RTU_Type.DWord_LH:
                        r = RTU.Data1.GetInt32_hlHL(num); break;
                    case Modbus_RTU_Type.Float:
                        r = RTU.Data1.GetFloat_HLhl(num); break;
                    case Modbus_RTU_Type.Float_LH:
                        r = RTU.Data1.GetFloat_hlHL(num); break;
                }
                return Math.Round(r * Scale, Digits);
            }
            set
            {
                double v = Math.Round(value / Scale, Digits);
                int num = DataAddress * 2;
                if (v == this.Value) return;
                RequestWrite = true;
                switch (UnitType)
                {
                    case Modbus_RTU_Type.Word:
                        RTU.Data1.SetInt16_HL(num, (short)v); break;
                    case Modbus_RTU_Type.Word_LH:
                        RTU.Data1.SetInt16_LH(num, (short)v); break;
                    case Modbus_RTU_Type.DWord:
                        RTU.Data1.SetInt32_HLhl(num, (int)v); break;
                    case Modbus_RTU_Type.DWord_LH:
                        RTU.Data1.SetInt32_hlHL(num, (int)v); break;
                    case Modbus_RTU_Type.Float:
                        RTU.Data1.SetFloat_HLhl(num, (float)v); break;
                    case Modbus_RTU_Type.Float_LH:
                        RTU.Data1.SetFloat_hlHL(num, (float)v); break;
                }
                SetRemoteValue = () => this.Value = value;
            }
        }


        public Modbus_RTU_Num(Modbus_RTU Rtu, int dataAddress, Modbus_RTU_Type type = Modbus_RTU_Type.Word)
        {
            RTU = Rtu;
            UnitType = type;
            DataAddress = dataAddress;
            ReadInterval = 0.1;
            Rtu.Units.Add(this);
        }
        public event Action<IModbus_RTU_Unit> ReadFinishEvent;
        public event Action<IModbus_RTU_Unit> WriteFinishEvent;


        public void ReadFinish()
        {
            RequestRead = false;
            LastTime = DateTime.Now;
            ReadFinishEvent?.Invoke(this);
        }
        public void WriteFinish()
        {
            RequestWrite = false;
            LastTime = DateTime.Now;
            WriteFinishEvent?.Invoke(this);
        }
        public void ExecuteRead()
        {
            int num = DataAddress * 2;
            ushort count = 1;
            switch (UnitType)
            {
                case Modbus_RTU_Type.DWord:
                case Modbus_RTU_Type.DWord_LH:
                case Modbus_RTU_Type.Float:
                case Modbus_RTU_Type.Float_LH:
                    count = 2; break;
            }
            var rv = Modbus_RTU.ReadData(RTU.Serial, RTU.ModbusAddress, (ushort)DataAddress, count);
            if (rv?.Length == 0) return;
            for (int i = 0; i < rv.Length; i++)
            {
                RTU.Data1.Data[num + i] = rv[i];
            }
            LastTime = DateTime.Now;
            RequestRead = false;
        }
        public void ExecuteWrite()
        {
            int num = DataAddress * 2;
            switch (UnitType)
            {

                case Modbus_RTU_Type.Word:
                case Modbus_RTU_Type.Word_LH:
                    Modbus_RTU.WriteData(RTU.Serial, RTU.ModbusAddress, (ushort)(DataAddress), RTU.Data1.Data[num], RTU.Data1.Data[num + 1]);
                    break;
                case Modbus_RTU_Type.DWord:
                case Modbus_RTU_Type.DWord_LH:
                case Modbus_RTU_Type.Float:
                case Modbus_RTU_Type.Float_LH:
                    Modbus_RTU.WriteDatas(RTU.Serial, RTU.ModbusAddress, (ushort)(DataAddress), RTU.Data1.Data, 4, num); 
                    break;
            }

            LastTime = DateTime.Now;
            RequestWrite = false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public void Parse(string text)
        {
            double d = 0;
            if(double.TryParse(text, out d))
            {
                Value = d;
            }
        }

    }

    public class Modbus_RTU_NumBit : IBit
    {
        public Modbus_RTU_Num Num { get; set; }
        public int Index { get; set; }
        public bool Value { get => Num[Index]; set => Num[Index] = value; }
        public Modbus_RTU_NumBit(Modbus_RTU_Num num,int index)
        {
            Num = num;
            Index = index;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
