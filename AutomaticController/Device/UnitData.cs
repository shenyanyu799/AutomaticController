using AutomaticController.Function;
using System;
using System.Security;

namespace AutomaticController.Device
{
    public class UnitData
    {
        public byte this[int index] { get => Data[index]; set => Data[index] = value; }
        /// <summary>
        /// 存储数据的内存空间
        /// </summary>
        public byte[] Data { get; private set; }
        public UnitData(int size)
        {

            Data = new byte[size];
        }
        public bool GetBit(int index, int bitIndex)
        {
            int n = (Data[index] >> bitIndex) & 1;
            return n == 1;
        }
        public byte GetByte(int index)
        {
            return Data[index];
        }

        public short GetInt16_LH(int index)
        {
            return BitConverter.ToInt16(Data, index);
        }
        public short GetInt16_HL(int index)
        {
            return BitConverter.ToInt16(Data, index).SwapHL();
        }

        public int GetInt32_HLhl(int index)
        {
            return (Data[index + 0] << 24) | (Data[index + 1] << 16) | (Data[index + 2] << 8) | Data[index + 3];
        }
        public int GetInt32_hlHL(int index)
        {
            return (Data[index + 2] << 24) | (Data[index + 3] << 16) | (Data[index + 0] << 8) | Data[index + 1];
        }
        public int GetInt32_LHlh(int index)
        {
            return (Data[index + 1] << 24) | (Data[index + 0] << 16) | (Data[index + 3] << 8) | Data[index + 2];
        }
        public int GetInt32_lhLH(int index)
        {
            return (Data[index + 3] << 24) | (Data[index + 2] << 16) | (Data[index + 1] << 8) | Data[index + 0];
        }

        [SecuritySafeCritical]
        public unsafe float GetFloat_HLhl(int index)
        {
            int n = GetInt32_HLhl(index);
            return *(float*)&n;
        }
        [SecuritySafeCritical]
        public unsafe float GetFloat_hlHL(int index)
        {
            int n = GetInt32_hlHL(index);
            return *(float*)&n;
        }
        [SecuritySafeCritical]
        public unsafe float GetFloat_LHlh(int index)
        {
            int n = GetInt32_LHlh(index);
            return *(float*)&n;
        }
        [SecuritySafeCritical]
        public unsafe float GetFloat_lhLH(int index)
        {
            int n = GetInt32_lhLH(index);
            return *(float*)&n;
        }
        public void SetBit(int index, int bitIndex, bool value = true)
        {
            if (value)
            {
                Data[index] = (byte)(Data[index] | (1 << bitIndex));
            }
            else
            {
                Data[index] = (byte)(Data[index] & (~(1 << bitIndex)));
            }
        }
        public void SetByte(int index, byte value)
        {
            Data[index] = value;
        }
        public void SetInt16_HL(int index, short value)
        {
            Data[index] = (byte)(value >> 8);
            Data[index + 1] = (byte)(value & 0x00ff);
        }
        public void SetInt16_LH(int index, short value)
        {
            Data[index] = (byte)(value & 0x00ff);
            Data[index + 1] = (byte)(value >> 8);
        }


        public void SetInt32_HLhl(int index, int value)
        {
            Data[index + 0] = (byte)(value >> 24);
            Data[index + 1] = (byte)(value >> 16);
            Data[index + 2] = (byte)(value >> 8);
            Data[index + 3] = (byte)(value & 0x00FF);
        }
        public void SetInt32_hlHL(int index, int value)
        {
            Data[index + 2] = (byte)(value >> 24);
            Data[index + 3] = (byte)(value >> 16);
            Data[index + 0] = (byte)(value >> 8);
            Data[index + 1] = (byte)(value & 0x00FF);
        }
        public void SetInt32_LHlh(int index, int value)
        {
            Data[index + 1] = (byte)(value >> 24);
            Data[index + 0] = (byte)(value >> 16);
            Data[index + 3] = (byte)(value >> 8);
            Data[index + 2] = (byte)(value & 0x00FF);
        }
        public void SetInt32_lhLH(int index, int value)
        {
            Data[index + 3] = (byte)(value >> 24);
            Data[index + 2] = (byte)(value >> 16);
            Data[index + 1] = (byte)(value >> 8);
            Data[index + 0] = (byte)(value & 0x00FF);
        }
        [SecuritySafeCritical]
        public unsafe void SetFloat_HLhl(int index, float value)
        {
            SetInt32_HLhl(index, *(int*)(&value));
        }

        [SecuritySafeCritical]
        public unsafe void SetFloat_hlHL(int index, float value)
        {
            SetInt32_hlHL(index, *(int*)(&value));
        }
        [SecuritySafeCritical]
        public unsafe void SetFloat_LHlh(int index, float value)
        {
            SetInt32_LHlh(index, *(int*)(&value));
        }
        [SecuritySafeCritical]
        public unsafe void SetFloat_lhLH(int index, float value)
        {
            SetInt32_lhLH(index, *(int*)(&value));
        }
    }

    public interface IBit
    {
        bool Value { get; set; }
    }
    public interface INum
    {
        double Value { get; set; }
    }
    public interface ITryText
    {
        void Parse(string text);
    }
    public class TextDouble : ITryText
    {
        public TextDouble(Func<double> get, Action<double> set, string format = null)
        {
            Get = get;
            Set = set;
            Format = format;
        }
        private Func<double> Get;
        private Action<double> Set;
        public double Value { get => Get(); set => Set(value); }
        public string Format;
        public void Parse(string text)
        {
            double d = 0;
            if (double.TryParse(text, out d))
            {
                Value = d;
            }
        }
        public override string ToString()
        {
            if (Format == null) return Value.ToString();
            return Value.ToString(Format);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    public class Num_Double : INum
    {
        public double Value { get => Get(); set => Set?.Invoke(value); }
        private Func<double> Get;
        private Action<double> Set;
        public Num_Double(Func<double> get, Action<double> set = null)
        {
            Get = get;
            Set = set;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public string ToString(string format)
        {
            return Value.ToString(format);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }



}
