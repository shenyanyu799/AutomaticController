using LiteDB;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace AutomaticController.Windows.SanSheng.数据库扫码查重项目
{
    /// <summary>
    /// 表的内容
    /// </summary>
    public class UserData
    {
        #region 功能

        public UserData() { }

        public static string[] GetNames()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string[] names = new string[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                names[i] = ps[i].Name;
            }
            return names;
        }
        public override string ToString()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string r = "";
            foreach (var item in ps)
            {
                r += item.GetValue(this) + ",";
            }
            return r.TrimEnd(',');
        }
        public string ToString2()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string r = "";
            foreach (var item in ps)
            {
                r += "'" + item.GetValue(this) + "',";
            }
            return r.TrimEnd(',');
        }
        /// <summary>
        /// 导出CSV时用
        /// </summary>
        /// <returns></returns>
        public string ToString3()
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            string r = "";
            foreach (var item in ps)
            {
                r += "\"" + item.GetValue(this) + "\",";
            }
            return r.TrimEnd(',');
        }

        public UserData(string[] args)
        {
            var ps = typeof(UserData).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            int len = ps.Length;
            if (len > args.Length)
            {
                len = args.Length;
            }

            for (int i = 0; i < len; i++)
            {

                if (ps[i].PropertyType == typeof(string))
                {
                    ps[i].SetValue(this, args[i]);
                    continue;
                }
                if (ps[i].PropertyType == typeof(DateTime))
                {
                    ps[i].SetValue(this, DateTime.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Double))
                {
                    ps[i].SetValue(this, Double.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Single))
                {
                    ps[i].SetValue(this, Single.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Int32))
                {
                    ps[i].SetValue(this, Int32.Parse(args[i]));
                    continue;
                }

                if (ps[i].PropertyType == typeof(Int16))
                {
                    ps[i].SetValue(this, Int16.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(Int64))
                {
                    ps[i].SetValue(this, Int64.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(UInt32))
                {
                    ps[i].SetValue(this, UInt32.Parse(args[i]));
                    continue;
                }

                if (ps[i].PropertyType == typeof(UInt16))
                {
                    ps[i].SetValue(this, UInt16.Parse(args[i]));
                    continue;
                }
                if (ps[i].PropertyType == typeof(UInt64))
                {
                    ps[i].SetValue(this, UInt64.Parse(args[i]));
                    continue;
                }
            }
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="args"></param>
        public static void Add(string[] args)
        {
            UserData user = new UserData(args);
            try
            {
                DBAdd(user);
            }
            catch
            {
                MessageBox.Show("数据库存储失败");
            }

        }

        public static void DBAdd(UserData data)
        {
            using (LiteDatabase lite = new LiteDatabase(DBPath))
            {
                var lst = lite.GetCollection<UserData>(TableName);
                lst.Insert(data);
            }
        }
        public static void DBAdd(IEnumerable<UserData> datas)
        {
            using (LiteDatabase lite = new LiteDatabase(DBPath))
            {
                var lst = lite.GetCollection<UserData>(TableName);
                lst.Insert(datas);
            }
        }
        public static List<UserData> DBFindAll()
        {
            List<UserData> userDatas = new List<UserData>();
            using (LiteDatabase lite = new LiteDatabase(DBPath))
            {
                var lst = lite.GetCollection<UserData>(TableName);
                userDatas.AddRange(lst.FindAll());
            }
            return userDatas;
        }
        public static string DBPath => "data/" + MainWindow1.Module;
        public static string TableName => "生产记录";
        #endregion


        public int ID { get; set; }
        public DateTime 扫码时间 { get; set; }
        public string 产品型号 { get; set; }
        public string SN码 { get; set; }

    }

}
