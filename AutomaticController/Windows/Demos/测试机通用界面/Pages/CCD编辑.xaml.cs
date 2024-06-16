using AutomaticController.Windows.Demos.测试机通用界面.Datas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VM.Core;
using VMControls.Interface;
using VMControls.WPF.Release;

namespace AutomaticController.Windows.Demos.测试机通用界面.Pages
{
    /// <summary>
    /// CCD编辑.xaml 的交互逻辑
    /// </summary>
    public partial class CCD编辑 : Page
    {
         
        public CCD编辑()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                if (VmSolution.Instance.SolutionPath == null)
                {
                    LoadCCD();
                }
            };
            this.Unloaded += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                SaveCCD();
            };
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
        }
        //判断应用是否以管理员身份运行
        public static bool IsRunningAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static event Action CCDLoadedEvent;
        public static string OutImgPath { get; set; }
        public static void ExecuteCCD()
        {
            //执行拍照
            if (CCDExecuting == false && 
                VmSolution.Instance.IsReady == true && VmSolution.Instance.IsRunning == false)
            {
                var pro_list = VmSolution.Instance.GetAllProcedureList().astProcessInfo;
                var vmProcedure = (VmProcedure)VmSolution.Instance[pro_list[0].strProcessName];
                CCDExecuting = true;
                try
                {
                    vmProcedure.Run();
                }
                catch { }
                CCDExecuting = false;
            }
        }
        static bool CCDExecuting;
        //static bool CCDLoading;
        //static bool CCDSaveing = false;

        public static void LoadCCD()
        {
            var param = Parameters_XMLFile.SelectItem;
            if (param == null) return;
            string path = param.CCDPath;
            if (IsRunningAsAdministrator() == false)
            {
                throw new Exception("应用权限不足，请以管理员身份运行此应用");
            }
            if (VmSolution.DogState != Apps.Data.DogStateEnum.Default)
            {
                throw new Exception("未检测到看门狗");
            }
            if (File.Exists(path) == false)
            {
                return;
            }
            if (VmSolution.Instance.SolutionPath == path) return;
            Task.Run(() =>
            {

                VmSolution.Instance.CloseSolution();
                VmSolution.Load(path);
                var pro_list = VmSolution.Instance.GetAllProcedureList().astProcessInfo;
                if(pro_list.Length > 0)
                {
                    var vmProcedure = (VmProcedure)VmSolution.Instance[pro_list[0].strProcessName];
                    vmProcedure.OnWorkEndStatusCallBack += (s, e) => {
                        try
                        {
                            string p1 = "";
                            string p2 = "";
                            foreach (var item in vmProcedure.Outputs)
                            {
                                if (item.Name.Contains("本地原图保存路径"))
                                {
                                    foreach (var _item in item.Value)
                                    {
                                        p1 = _item.GetType().GetField("strValue")?.GetValue(_item)?.ToString();
                                        break;
                                    }
                                    continue;
                                }
                                if (item.Name.Contains("渲染图名称"))
                                {
                                    foreach (var _item in item.Value)
                                    {
                                        p2 = _item.GetType().GetField("strValue")?.GetValue(_item)?.ToString();
                                        break;
                                    }
                                    continue;
                                }
                            }
                            OutImgPath = System.IO.Path.Combine(p1, p2);
                        }
                        catch
                        {

                        }

                    };
                }
                App.Current.Dispatcher.Invoke(() => CCDLoadedEvent?.Invoke());
                
            });

        }
        public static void SaveCCD()
        {
            if (File.Exists(VmSolution.Instance.ModuleFilePath))
            {
                Task.Run(() => {
                    try
                    {
                        VmSolution.Save();
                        //VmSolution.Save();
                        //App.Current.Dispatcher.Invoke(() => CCDLoadedEvent?.Invoke());
                        //LoadCCD();
                    }
                    catch { }
                    //CCDSaveing = false;
                });
            }
        }
        public static void CloseCCD()
        {
            try
            {
                VmSolution.Instance.CloseSolution();
                VmSolution.Instance.Dispose();
            }
            catch { }
        }
    }
}
