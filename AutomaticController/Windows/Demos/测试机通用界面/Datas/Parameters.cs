using Apps.Data.ShapeDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace AutomaticController.Windows.Demos.测试机通用界面.Datas
{
   //在这里设定需要备份的参数
    public partial class Parameters
    {
        public double 启动时间 { get; set; }
        public double 检测时间 { get; set; }
        public double 流量上限 { get; set; }
        public double 流量下限 { get; set; }
        public double 流量系数 { get; set; }
        public string CCDPath { get; set; }
    }
    public partial class Parameters
    {
        public string FileName {  get; set; }


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
            if(newName == FileName) return false;
            var names = Parameters_XMLFile.Instance.GetNames();
            if(names.Contains(newName)) return false;
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
        public Parameters this[string name] { get {
                var par =  GetValue<Parameters>(name);
                if (par == null) return null;
                par.FileName = name;
                return par;
            } set => SetValue(name, value); }
        public event Action<Parameters> LoadParamEvent;
        public event Action<Parameters> SaveParamEvent;
        //载入关联的参数
        public void LoadParam()
        {
            var param = Parameters_XMLFile.SelectItem;
            if (param == null) return;
         
            PLC1.产品启动时间.Value = param.启动时间;
            PLC1.检测时间.Value = param.检测时间;
            PLC1.流量上限.Value = param.流量上限;
            PLC1.流量下限.Value = param.流量下限;
            PLC1.流量系数.Value = param.流量系数;

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
            param.启动时间 = PLC1.产品启动时间.Value;
            param.检测时间 = PLC1.检测时间.Value;
            param.流量上限 = PLC1.流量上限.Value;
            param.流量下限 = PLC1.流量下限.Value;
            param.流量系数 = PLC1.流量系数.Value;
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
                Instance[name] = param;
            }
            Parameters_XMLFile.Instance.LoadParam();
            return param;
        }
        public static Parameters SelectFirst()
        {
            var names = Instance.GetNames();
            if(names.Length > 0)
            {
                Setting.Instance.ParametersSelectName = names[0];
                return SelectItem;
            }
            else
            {
                return NewParameters();
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
