using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutomaticController.Windows.Demos.吸尘器空气性能测试
{

    public partial class Parameters
    {
        public string TemperatureName { get => GetValue("温度采集端口", "COM1"); set => SetValue("温度采集端口", value); }
        public string ASName { get => GetValue("美标设备端口", "COM2"); set => SetValue("美标设备端口", value); }
        public string ENName { get => GetValue("欧标设备端口", "COM3"); set => SetValue("欧标设备端口", value); }
    }
    public partial class Parameters : INotifyPropertyChanged
    {
        public static Parameters Instance { get; set; } = new Parameters("Parameters.xml");

        public Parameters(string path)
        {
            Path = path;
        }
        public string Path { get; set; }

        private XDocument Document;

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetValue(string key)
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
                    new XElement("Parameters").Save(Path);
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
            return "";
        }
        public string GetValue(string key, string defaultValue)
        {
            string r = GetValue(key);
            if (r == null || r == "")
            {
                SetValue(key, defaultValue);
                return defaultValue;
            }
            return r;
        }

        public TEnum GetValue<TEnum>(string key, TEnum defaultValue = default(TEnum)) where TEnum : struct
        {
            TEnum @enum = defaultValue;
            if (Enum.TryParse<TEnum>(GetValue(key), out @enum))
            {
                return @enum;
            }
            return defaultValue;
        }
        public double GetValue(string key, double defaultValue = 0)
        {
            string t = GetValue(key);
            double result = defaultValue;
            if (double.TryParse(t, out result))
            {
                return result;
            }
            return defaultValue;
        }
        public int GetValue(string key, int defaultValue = 0)
        {
            string t = GetValue(key);
            int result = defaultValue;
            if (int.TryParse(t, out result))
            {
                return result;
            }
            return defaultValue;
        }
        public byte GetValue(string key, byte defaultValue = 0)
        {
            string t = GetValue(key);
            byte result = defaultValue;
            if (byte.TryParse(t, out result))
            {
                return result;
            }
            return defaultValue;
        }



        bool Parametersvalue;
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
                    new XElement("Parameters").Save(Path);
                    Document = XDocument.Load(Path);
                }
            }

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
            //延时保存，减少存盘频率
            Task.Run(() =>
            {
                if (Parametersvalue)
                {
                    return;
                }
                Parametersvalue = true;
                Task.Delay(100).Wait();
                Parametersvalue = false;
                root.Save(Path);
            });
        }

    }
}
