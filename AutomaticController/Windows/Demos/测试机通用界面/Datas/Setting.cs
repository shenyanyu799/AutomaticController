using AutomaticController.Function;
using AutomaticController.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using static AutomaticController.Windows.Demos.测试机通用界面.Datas.Enums;

namespace AutomaticController.Windows.Demos.测试机通用界面.Datas
{
    /// <summary>
    /// 存储一些参数
    /// </summary>
    public class Setting : XMLFile
    {
        public static Setting Instance { get; set; } = new Setting("setting.xml");
        public Setting(string path) : base(path) { }



        public string UserKey { get => GetValue("UserKey", "888888"); set => SetValue("UserKey", value); }
        public WindowStartupState WindowStartupState { get => GetValue("WindowStartupState", WindowStartupState.None); set => SetValue("WindowStartupState", value); }
        public string PLC1_Name { get => GetValue("PLC1_Name", "COM1"); set => SetValue("PLC1_Name", value); }
        public int PLC1_Baud { get => GetValue("PLC1_Baud", 9600); set => SetValue("PLC1_Baud", value); }
        public Parity PLC1_Parity { get => GetValue("PLC1_Parity", Parity.None); set => SetValue("PLC1_Parity", value); }
        public int PLC1_Databit { get => GetValue("PLC1_Databit", 8); set => SetValue("PLC1_Databit", value); }
        public StopBits PLC1_Stopbit { get => GetValue("PLC1_Stopbit", StopBits.One); set => SetValue("PLC1_Stopbit", value); }



    }

    /// <summary>
    /// 以XML文件形式存储数据
    /// </summary>
    public class XMLFile : INotifyPropertyChanged
    {
        public string Path { get; set; }

        private XDocument Document;

        public event PropertyChangedEventHandler PropertyChanged;
        public XMLFile()
        {

        }
        public XMLFile(string path)
        {
            Path = path;
        }

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 在本地初始化数据，确保所有参数都在本地有值，如果文件创建后没有执行该方法
        /// 则仅在调用参数初始化被调用的参数
        /// </summary>
        public void Initialization()
        {
            var ps = this.GetType().GetProperties();
            foreach (var item in ps)
            {
                item.GetValue(this);
            }
        }

        #region GetValue

        private string GetValue(string key)
        {
            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(Path))
                {
                    try
                    {
                        Document = XDocument.Load(Path);
                    }
                    catch
                    {
                        isnew = true;
                    }
                }
                else
                {
                    isnew = true;
                }

                if (isnew)
                {
                    new XElement("setting").Save(Path);
                    Document = XDocument.Load(Path);
                }
            }

            //获取根节点
            XElement root = Document.Root;
            //查找数据
            var v = root.Element(key);
            if (v != null)
            {
                return v.Value;
            }
            return null;
        }
        public string GetValue(string key, string defaultValue = "")
        {
            string r = GetValue(key);
            if (r == null)
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
            return r;
        }


        public T GetValueFromJson<T>(string key, T defaultValue = default(T))
        {

            string t = GetValue(key);
            T result = defaultValue;
            try
            {
                result = JsonConvert.DeserializeObject<T>(t);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    _savevalue(key, defaultValue);
                    return defaultValue;
                }
            }
            catch
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }

        }
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            T value = defaultValue;
            Type tp = value.GetType();

            if (tp.IsEnum)//判断是否为枚举类型
            {
                try
                {
                    value = (T)Enum.Parse(tp, GetValue(key));
                }
                catch
                {
                    _savevalue(key, defaultValue);
                    return defaultValue;
                }
                if (value != null)
                {
                    return value;
                }
                else
                {
                    _savevalue(key, defaultValue);
                    return defaultValue;
                }
            }
            if (tp.IsValueType || tp.IsClass)
            {
                return GetValueFromJson(key, defaultValue);
            }
            return defaultValue;
        }

        public long GetValue(string key, long defaultValue = 0)
        {
            string t = GetValue(key);
            long result = defaultValue;
            if (long.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public double GetValue(string key, double defaultValue = 0)
        {
            string t = GetValue(key);
            double result = defaultValue;
            if (double.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public float GetValue(string key, float defaultValue = 0)
        {

            string t = GetValue(key);
            float result = defaultValue;
            if (float.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public int GetValue(string key, int defaultValue = 0)
        {
            string t = GetValue(key);
            int result = defaultValue;
            if (int.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public short GetValue(string key, short defaultValue = 0)
        {
            string t = GetValue(key);
            short result = defaultValue;
            if (short.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public uint GetValue(string key, uint defaultValue = 0)
        {
            string t = GetValue(key);
            uint result = defaultValue;
            if (uint.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public ushort GetValue(string key, ushort defaultValue = 0)
        {
            string t = GetValue(key);
            ushort result = defaultValue;
            if (ushort.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }
        public byte GetValue(string key, byte defaultValue = 0)
        {
            string t = GetValue(key);
            byte result = defaultValue;
            if (byte.TryParse(t, out result))
            {
                return result;
            }
            else
            {
                _savevalue(key, defaultValue);
                return defaultValue;
            }
        }

        #endregion


        bool settingvalue;
        public void SetValueToJson(string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            SetValue(key, json);
        }
        public void SetValue(string key, object value)
        {
            OnPropertyChanged(key);

            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(Path))
                {
                    try
                    {
                        Document = XDocument.Load(Path);
                    }
                    catch
                    {
                        isnew = true;
                    }
                }
                else
                {
                    isnew = true;
                }

                if (isnew)
                {
                    new XElement("setting").Save(Path);
                    Document = XDocument.Load(Path);
                }
            }
            if(value == null)
            {
                value = "";
            }
            Type tp = value.GetType();
            if (tp.IsValueType || tp.IsClass)
                _savevalue(key, value);
        }
        private void _savevalue(string key, object value)
        {
            //延时保存，减少存盘频率
            XElement root = Document.Root;
            var v = root.Element(key);
            if (value == null) value = "";
            if (v != null)
            {
                v.Value = value.ToString();
            }
            else
            {
                root.Add(new XElement(key) { Value = value.ToString() });
            }
            Task.Run(() =>
            {
                if (settingvalue)
                {
                    return;
                }
                settingvalue = true;
                Task.Delay(100).Wait();
                settingvalue = false;

                Document.Root.Save(Path);
            });
        }
    }
}
