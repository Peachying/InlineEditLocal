using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text;

namespace InlineEdit
{
    class Program
    {
        private static string path = @"C:\Users\t-yucxu\Desktop\testInlineEdit\middlefile";


        static void Main(string[] args)
        {
            batchPandoc();
            modifyMdfile();
            Console.ReadKey();
        }


        public static void batchPandoc() {
            string head = @"pandoc -f html -t markdown ";
            string tail = @" -o " + path + @"\frag_";
            string[] filenames = Directory.GetFiles(path, "*.html");
            foreach (string file in filenames)
            {
                string exec = head + file + tail + file.Substring(file.IndexOf("_") + 1, 1) + ".md\n";
                RunCmd(exec);
            }
        }
        public static void RunCmd(string cmd)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;
            string output = p.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            p.WaitForExit();
            p.Close();
        }

        public static void modifyMdfile() {
            StreamReader sr = new StreamReader(path + @"\editinfo.txt");
            string infoStr = sr.ReadToEnd();
            JArray infoArray = (JArray)JsonConvert.DeserializeObject(infoStr);
            int fileNum = 0;
            foreach (Object info in infoArray)
            {
                string sourceFile = path + @"\origin.md";
                string fragFile = path + @"\frag_" + fileNum + @".md";
                fileNum += 1;
                JObject fragInfo = (JObject)info;
                EditFile((int)fragInfo["startline"], (int)fragInfo["endline"], sourceFile, fragFile);
                Console.WriteLine(@"Startline is {0}, endline is {1}, origin_url is {2}", fragInfo["startline"], fragInfo["endline"], fragInfo["origin_url"]);
            }
        }
        public static void EditFile(int startLine, int endLine, string sourcePath, string newPath)
        {
            StreamReader sr_new = new StreamReader(newPath);
            string newLines = sr_new.ReadToEnd().Replace("\r\n", " ");
            
            FileStream fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8"));            
            string line = sr.ReadLine();            
            string text = "";

            for (int i = 1; line != null; i++)
            {
                if (i != startLine)
                    text += line + "\r\n";
                else

                    text += newLines + "\r\n";
                line = sr.ReadLine();
            }
            sr.Close();
            FileStream fs1 = new FileStream(sourcePath, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs1, Encoding.GetEncoding("utf-8"));
            sw.Write(text);
            sw.Close();
            sr_new.Close();
            fs1.Close();
        }
    }
}
