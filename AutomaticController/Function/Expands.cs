using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Media;

namespace AutomaticController.Function
{
    /// <summary>
    /// 包含拓展功能
    /// </summary>
    public static class Expands
    {
        #region 位操作


        public static bool GetBit(this byte value, int bitIndex)
        {
            int n = (value >> bitIndex) & 1;
            return n == 1;
        }
        public static bool GetBit(this short value, int bitIndex)
        {
            int n = (value >> bitIndex) & 1;
            return n == 1;
        }
        public static bool GetBit(this int value, int bitIndex)
        {
            int n = (value >> bitIndex) & 1;
            return n == 1;
        }
        [SecuritySafeCritical]
        public static unsafe bool GetBit(this float value, int bitIndex)
        {
            int v = *(int*)&value;

            int n = (v >> bitIndex) & 1;
            return n == 1;
        }
        public static void SetBit(this ref byte value, int bitIndex, bool bit = true)
        {
            if (bit)
            {
                value = (byte)(value | (1 << bitIndex));
            }
            else
            {
                value = (byte)(value & (~(1 << bitIndex)));
            }
        }
        public static void SetBit(this ref short value, int bitIndex, bool bit = true)
        {
            if (bit)
            {
                value = (short)(value | (1 << bitIndex));
            }
            else
            {
                value = (short)(value & (~(1 << bitIndex)));
            }
        }
        public static void SetBit(this ref int value, int bitIndex, bool bit = true)
        {
            if (bit)
            {
                value = value | (1 << bitIndex);
            }
            else
            {
                value = value & (~(1 << bitIndex));
            }
        }
        [SecuritySafeCritical]
        public static unsafe void SetBit(this ref float value, int bitIndex, bool bit = true)
        {
            float n = value;
            int num = (*(int*)&n);
            int r = 0;
            if (bit)
            {
                r = (num | (1 << bitIndex));
            }
            else
            {
                r = (num & (~(1 << bitIndex)));
            }
            value = (*(float*)&r);
        }

        #endregion
        /// <summary>
        /// 整数拼接
        /// </summary>
        /// <param name="LNum"></param>
        /// <param name="HNum"></param>
        /// <returns></returns>
        public static int SplicingInt(ushort LNum, ushort HNum)
        {
            return (HNum << 16) | LNum;
        }
        /// <summary>
        /// 浮点拼接
        /// </summary>
        /// <param name="LNum"></param>
        /// <param name="HNum"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static float SplicingFloat(ushort LNum, ushort HNum)
        {
            int num = (HNum << 16) | LNum;
            return *(float*)&num;
        }
        /// <summary>
        /// 交换高低8位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short SwapHL(this short value)
        {
            int v = value & 0x0000FFFF;
            return (short)((v << 8) | (v >> 8));
        }
        /// <summary>
        /// 交换高低16位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int SwapHL(this int value)
        {
            return (value << 16) | (value >> 16);
        }
        /// <summary>
        /// 交换高低16位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [SecuritySafeCritical]
        public unsafe static float SwapHL(this float value)
        {
            int n = SwapHL(*(int*)&value);
            return *(float*)&n;
        }
        /// <summary>
        /// 获得控件所在窗口
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Window GetParentWindow(this FrameworkElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as Window;
        }

        /// <summary>
        /// 返回指定枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetIEnumerable<T>(T obj) where T : struct
        {
            return Enum.GetValues(typeof(T)).OfType<T>();
        }
        public static string ToStringH(this byte[] data, string separator = " ")
        {

            string t = "";
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                t += data[i].ToString("X2") + separator;
            }
            return t.Trim();
        }
        public static string ToStringH(this byte[] data, int startIndex, int length, string separator = " ")
        {

            string t = "";
            int len = length + startIndex;
            for (int i = startIndex; i < len; i++)
            {
                t += data[i].ToString("X2") + separator;
            }
            return t.Trim();
        }
    }
}
