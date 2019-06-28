using System;
using System.IO;

namespace InlineEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\t-yucxu\Desktop\testInlineEdit\middlefile";            
            string head = @"pandoc -f html -t markdown ";
            string tail = @" -o C:\Users\t-yucxu\Desktop\testInlineEdit\middlefile\frag_";
            
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();            

            //DirectoryInfo dir = new DirectoryInfo(path);
            //FileInfo[] f = dir.GetFiles();
            string[] filenames = Directory.GetFiles(path, "*.html");
            foreach (string file in filenames)
            {
                string exec = head + file + tail + file.Substring(file.IndexOf("_") + 1, 1) + ".md\n";
                p.StandardInput.WriteLine(exec + "&exit");
                p.StandardInput.AutoFlush = true;
                
            }
            string output = p.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            //p.WaitForExit();
            p.Close();
            
            Console.ReadKey();
        }
    }
}
