using System;
using System.Linq;

namespace AutomaticController.Windows.Demos.测试机通用界面.Datas
{
    //在这里设定需要备份的参数
    public partial class Parameters
    {
        public double 信号检测延时 { get; set; }
        public double 气缸升降时间 { get; set; }
        public double 推料动作时间 { get; set; }
        public double 重量下限 { get; set; }
        public double 重量上限 { get; set; }
        public string CCDPath { get; set; }
        public string SN码前缀 { get; set; }
        public bool 电机方向 { get; set; }
        public bool 扫码启用 { get; set; }
        public bool 拍照启用 { get; set; }
        public bool 重码检测 { get; set; }
    }
    public partial class Parameters
    {
        public string FileName;


        public void Save()
        {
            Parameters_XMLFile.Instance[FileName] = this;
        }
        public void Delete()
        {
            Parameters_XMLFile.Instance.Remove(FileName);
            Setting.Instance.ParametersSelectName = null;
        }
        /// <summary>
        /// 重命名，修改成功返回true
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool Rename(string newName)
        {
            if (newName == FileName) return false;
            var names = Parameters_XMLFile.Instance.GetNames();
            if (names.Contains(newName)) return false;
            //Parameters_XMLFile.Instance.Remove(FileName);
            //Parameters_XMLFile.Instance[newName] = this;
            //FileName = newName;
            try
            {
                Parameters_XMLFile.Instance.Rekey(FileName, newName);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
    /// <summary>
    /// 实现各种设备之间的用户参数统一管理
    /// </summary>
    public class Parameters_XMLFile : XMLFile
    {
        public Parameters this[string name]
        {
            get
            {
                var par = GetValue<Parameters>(name);
                if (par == null) return null;
                par.FileName = name;
                return par;
            }
            set
            {
                SetValue(name, value);
            }
        }
        public event Action<Parameters> LoadParamEvent;
        public event Action<Parameters> SaveParamEvent;
        //载入关联的参数
        public void LoadParam()
        {
            var param = Parameters_XMLFile.SelectItem;
            if (param == null) return;

            PLC1.信号检测延时.Value = param.信号检测延时;
            PLC1.气缸升降时间.Value = param.气缸升降时间;
            PLC1.推料动作时间.Value = param.推料动作时间;
            PLC1.重量下限.Value = param.重量下限;
            PLC1.重量上限.Value = param.重量上限;
            PLC1.电机方向.Value = param.电机方向;
            PLC1.扫码启用.Value = param.扫码启用;
            PLC1.拍照启用.Value = param.拍照启用;
            LoadParamEvent?.Invoke(param);


        }
        /// <summary>
        /// 保存并关联参数
        /// </summary>
        public void SaveParam()
        {
            var param = Parameters_XMLFile.SelectItem;
            if (param == null) return;
            param.CCDPath = param.CCDPath;
            param.信号检测延时 = PLC1.信号检测延时.Value;
            param.气缸升降时间 = PLC1.气缸升降时间.Value;
            param.推料动作时间 = PLC1.推料动作时间.Value;
            param.重量下限 = PLC1.重量下限.Value;
            param.重量上限 = PLC1.重量上限.Value;
            param.电机方向 = PLC1.电机方向.Value;
            param.扫码启用 = PLC1.扫码启用.Value;
            param.拍照启用 = PLC1.拍照启用.Value;
            SaveParamEvent?.Invoke(param);
            param.Save();
        }
        public static Parameters_XMLFile Instance { get; set; } = new Parameters_XMLFile("parameters.xml");
        public static Parameters SelectItem => Instance[Setting.Instance.ParametersSelectName];
        //选择参数并自动加载数据
        public static Parameters Select(string name)
        {
            var param = Instance[name];
            Setting.Instance.ParametersSelectName = name;
            if (param == null)
            {
                param = new Parameters();
                param.FileName = name;
                Instance[name] = param;
            }
            Parameters_XMLFile.Instance.LoadParam();
            return param;
        }
        public static Parameters SelectFirst()
        {
            var names = Instance.GetNames();
            if (names.Length > 0)
            {
                Setting.Instance.ParametersSelectName = names[0];
                return SelectItem;
            }
            else
            {
                var par = NewParameters();
                Setting.Instance.ParametersSelectName = par.FileName;
                return par;
            }

        }
        /// <summary>
        /// 自动创建一个参数
        /// </summary>
        /// <returns></returns>
        public static Parameters NewParameters()
        {
            string[] names = Instance.GetNames();
            string name = "";
            for (int i = 1; i < int.MaxValue; i++)
            {
                name = "Modbule" + i;
                if (names.Contains(name) == false)
                {
                    return Select(name);
                }
            }
            return new Parameters();
        }
        public Parameters_XMLFile(string path) : base(path) { }
    }
}
