using AutomaticController.Windows.Demos.测试机通用界面.Datas;
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
                if(VmSolution.Instance.SolutionPath == null)
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
            VmSolution.Instance.CloseSolution();
            VmSolution.Load(path);
        }
        public static void SaveCCD()
        {
            try
            {
                if (File.Exists(VmSolution.Instance.ModuleFilePath))
                {
                    Task.Run(() => VmSolution.Save());
                }
            }
            catch { }
        }
        public static void CloseCCD()
        {
            try
            {
                VmSolution.Instance.CloseSolution();
            }catch { }
        }
    }
}
