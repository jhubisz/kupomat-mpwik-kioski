using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcComunication
{
    public static class Logger
    {
        public static string filePath = @"C:\Users\pawelr\Desktop\KioskAppWpf\AquanetLog.txt";

        public static void Log(string text)
        {
            using (StreamWriter streamWriter = File.AppendText(filePath))
            {
                streamWriter.WriteLine(DateTime.Now.ToString() + " - " + text);
                streamWriter.Close();
            }
        }
    }
}
