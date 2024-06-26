﻿using System.Windows;

namespace AutomaticController.Windows.SanSheng.数据库扫码查重项目
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            PasswordBox1.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string pass = PasswordBox1.Password;
            string user = ComboBox1.Text.ToLower();
            switch (user)
            {
                case "user":
                    if (pass == Setting.Instance.UserPassword || Setting.Instance.UserPassword == null)
                    {
                        MainWindow1.Logined = UserPermissions.Operator;
                        this.Close();
                    }
                    else
                    {
                        PasswordBox1.Password = "";
                    }
                    break;
                case "administrator":
                    if (pass == "&ss2671")
                    {
                        MainWindow1.Logined = UserPermissions.Administrator;
                        this.Close();
                    }
                    else
                    {
                        PasswordBox1.Password = "";
                    }
                    break;
            }

        }
    }
}
