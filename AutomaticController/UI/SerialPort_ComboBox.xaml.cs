using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AutomaticController.UI
{
    /// <summary>
    /// SerialPort_ComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPort_ComboBox : ComboBox
    {
        public SerialPort_ComboBox()
        {
            InitializeComponent();
            comRefresh(this);
        }
        public new string Text
        {
            get
            {
                if(base.Text == "")
                {
                    comRefresh(this);
                }
                return base.Text;
            }
            set
            {
                comRefresh(this);
                base.Text = value;
            }
        }
        /// <summary>
        ///产品端口下拉打开时刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComText_DropDownOpened(object sender, EventArgs e)
        {
            comRefresh((ComboBox)sender);
        }
        //产品端口下拉打开时刷新
        void comRefresh(ComboBox combo)
        {
            string st = combo.Text;
            combo.Items.Clear();
            string[] coms = System.IO.Ports.SerialPort.GetPortNames();
            int i = 0;
            int s = 0;
            foreach (string com in coms)
            {
                if (com == st)
                {
                    s = i;
                }
                combo.Items.Add(com);
                i++;
            }
            combo.SelectedIndex = s;
        }

    }
}
