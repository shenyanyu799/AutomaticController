using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomaticController.Function
{
    public class LogManager
    {
        public static string OutPath => "log.txt";
        public static Encoding Encoding { get; set; } = Encoding.Default;
        static FileStream fileStream = new FileStream(OutPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        public static void Log(string content, LogLevel level = LogLevel.Message) 
        {
            lock (fileStream)
            {
                fileStream.Position = fileStream.Length;
                byte[] bytes = Encoding.GetBytes($"[{level} {DateTime.Now.ToString()}]:{content}\n");
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
            //if (content == null) return;
            //values.Enqueue((content, level));
            //if (outing) return;
            //outing = true;
            //Task.Run(() =>
            //{
            //    if (fileStream == null) fileStream = new FileStream(OutPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            //    lock (fileStream)
            //    {
            //        fileStream.Position = fileStream.Length;
            //        do
            //        {
            //            var c = values.Dequeue();
            //            byte[] bytes = Encoding.UTF8.GetBytes($"[{c.level}]{DateTime.Now}:{c.content}\n");
            //            fileStream.Write(bytes, 0, bytes.Length);
            //        } while (values.Count > 0);
            //        fileStream.Flush();
            //        //fileStream.Close();
            //        //fileStream = null;
            //    }
                
            //    //Task.Delay(500).Wait();
            //    Console.WriteLine(DateTime.Now);
            //    outing = false;
            //});
        }

        public static void LogShow(string content, LogLevel level = LogLevel.Message)
        {
            Log(content, level);
            MessageBox.Show(content, level.ToString());
        }

    }
    public enum LogLevel
    {
        Message = 0,
        Warn = 1,
        Error = 2,
    }
}
