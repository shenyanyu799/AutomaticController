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
        public string ParametersSelectName { get => GetValue("ParametersSelectName", "Default"); set => SetValue("ParametersSelectName", value); }
        public string Sweep1_Name { get => GetValue("Sweep1_Name", "COM1"); set => SetValue("Sweep1_Name", value); }
        public int Sweep1_Baud { get => GetValue("Sweep1_Baud", 115200); set => SetValue("Sweep1_Baud", value); }
        public Parity Sweep1_Parity { get => GetValue("Sweep1_Parity", Parity.None); set => SetValue("Sweep1_Parity", value); }
        public int Sweep1_Databit { get => GetValue("Sweep1_Databit", 8); set => SetValue("Sweep1_Databit", value); }
        public StopBits Sweep1_Stopbit { get => GetValue("Sweep1_Stopbit", StopBits.One); set => SetValue("Sweep1_Stopbit", value); }
    }

    /// <summary>
    /// 以XML文件形式存储数据
    /// </summary>
    public class XMLFile : INotifyPropertyChanged
    {
        /// <summary>
        /// XML文件存储路径
        /// </summary>
        public string XMLFilePath { get; set; }

        private XDocument Document;

        public event PropertyChangedEventHandler PropertyChanged;
        public XMLFile()
        {

        }
        public XMLFile(string path)
        {
            XMLFilePath = path;
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


        public void Remove(string key)
        {
            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(XMLFilePath))
                {
                    try
                    {
                        Document = XDocument.Load(XMLFilePath);
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
                    new XElement(XMLFilePath).Save(XMLFilePath);
                    Document = XDocument.Load(XMLFilePath);
                }
            }

            //获取根节点
            XElement root = Document.Root;
           
            //查找数据
            var v = root.Element(key);
            if (v != null)
            {
                v.Remove();
                
                Task.Run(() =>
                {
                    if (settingvalue)
                    {
                        return;
                    }
                    settingvalue = true;
                    Task.Delay(100).Wait();
                    settingvalue = false;

                    Document.Root.Save(XMLFilePath);
                });
            }
        }
        public void Rekey(string key,string newKey)
        {
            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(XMLFilePath))
                {
                    try
                    {
                        Document = XDocument.Load(XMLFilePath);
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
                    new XElement(XMLFilePath).Save(XMLFilePath);
                    Document = XDocument.Load(XMLFilePath);
                }
            }

            //获取根节点
            XElement root = Document.Root;

            //查找数据
            var v = root.Element(key);
            if (v != null)
            {
                v.Name = newKey;
                Console.WriteLine(v);
                Task.Run(() =>
                {
                    if (settingvalue)
                    {
                        return;
                    }
                    settingvalue = true;
                    Task.Delay(100).Wait();
                    settingvalue = false;

                    Document.Root.Save(XMLFilePath);
                });
            }
        }
        public string[] GetNames()
        {
            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(XMLFilePath))
                {
                    try
                    {
                        Document = XDocument.Load(XMLFilePath);
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
                    new XElement(XMLFilePath).Save(XMLFilePath);
                    Document = XDocument.Load(XMLFilePath);
                }
            }
            //获取根节点
            XElement root = Document.Root;
            var atts = root.Elements();
            int len = atts.Count();
            string[] result = new string[len];
            int i = 0;
            foreach (var item in atts)
            {
                result[i] = item.Name.LocalName;
                i++;
            }
            return result;
        }
        #region GetValue

        private string _GetValue(string key)
        {
            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(XMLFilePath))
                {
                    try
                    {
                        Document = XDocument.Load(XMLFilePath);
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
                    new XElement(XMLFilePath).Save(XMLFilePath);
                    Document = XDocument.Load(XMLFilePath);
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
        //public string GetValue(string key, string defaultValue = "")
        //{
        //    string r = GetValue(key);
        //    if (r == null)
        //    {
        //        _savevalue(key, defaultValue);
        //        return defaultValue;
        //    }
        //    return r;
        //}


        public T GetValue<T>(string key, T defaultValue = default(T))
        {

            string t = _GetValue(key);
            T result = defaultValue;
            Type tp = typeof(T);
            if (tp.IsEnum)//判断是否为枚举类型
            {
                try
                {
                    result = (T)Enum.Parse(tp, t);
                }
                catch
                {
                    _savevalue(key, defaultValue);
                    return defaultValue;
                }
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _savevalue(key, defaultValue);
                return defaultValue;
            }

        }

        public long GetValue(string key, long defaultValue = 0)
        {
            string t = _GetValue(key);
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
            string t = _GetValue(key);
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

            string t = _GetValue(key);
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
            string t = _GetValue(key);
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
            string t = _GetValue(key);
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
            string t = _GetValue(key);
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
            string t = _GetValue(key);
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
            string t = _GetValue(key);
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
        public string GetValue(string key, string defaultValue = "")
        {
            string t = _GetValue(key);
            if (t == null)
            {
                return defaultValue;
            }
            else
            {
                return t;
            }
        }
        #endregion


        bool settingvalue;
        public void SetValue(string key, object value)
        {
            if (value is string)
            {
                setValue(key, value.ToString());
                return;
            }
            string json = JsonConvert.SerializeObject(value);
            setValue(key, json);
        }

        private void setValue(string key, string value)
        {
            Type tp = value.GetType();


            OnPropertyChanged(key);
            if (Document == null)
            {
                bool isnew = false;
                if (File.Exists(XMLFilePath))
                {
                    try
                    {
                        Document = XDocument.Load(XMLFilePath);
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
                    new XElement(XMLFilePath).Save(XMLFilePath);
                    Document = XDocument.Load(XMLFilePath);
                }
            }
            if (value == null)
            {
                value = "";
            }
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

                Document.Root.Save(XMLFilePath);
            });
        }
    }
}
