﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text;
using Octokit;
using FileMode = System.IO.FileMode;
using System.Threading.Tasks;
using System.Net;
using InlineEdit.JsonEntities;
using System.Data.SqlTypes;

namespace InlineEdit
{
      
    
    class Program
    {
        private static string path = @"C:\Users\t-yucxu\Desktop\testInlineEdit\middlefile";
        private static string sourceFile = path + @"\origin.md";
        
        private static string url_fork = @"https://api.github.com/repos/Peachying/testinlineedit/forks";
        private static string url_getRef = @"https://api.github.com/repos/GraceXu96/testinlineedit/git/refs/heads/master";
        private static string url_createBlob = @"https://api.github.com/repos/GraceXu96/testinlineedit/git/blobs";
        private static string url_createTree = @"https://api.github.com/repos/GraceXu96/testinlineedit/git/trees";
        private static string url_getCommit = @"https://api.github.com/repos/GraceXu96/testinlineedit/git/commits/";
        private static string url_createCommit = @"https://api.github.com/repos/GraceXu96/testinlineedit/git/commits";
        private static string url_updateRef = @"https://api.github.com/repos/GraceXu96/testinlineedit/git/refs/heads/master";
        private static string url_pullRequest = @"https://api.github.com/repos/Peachying/testinlineedit/pulls";

        static async Task Main(string[] args)
        {
            //batchPandoc();
            //modifyMdfile();
            Commit();
            PullRequest();
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
                string fragFile = path + @"\frag_" + fileNum + @".md";
                fileNum += 1;
                JObject fragInfo = (JObject)info;
                EditFile((int)fragInfo["startline"], (int)fragInfo["endline"], sourceFile, fragFile);
                Console.WriteLine(@"Startline is {0}, endline is {1}, origin_url is {2}", fragInfo["startline"], fragInfo["endline"], fragInfo["origin_url"]);
            }
        }

        private static void EditFile(int startLine, int endLine, string sourcePath, string newPath)
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

        private static void PullRequest()
        {
            CreatePullRequest pullRequestBody = new CreatePullRequest
            {
                Title = "test PR with Github API",
                Head = "GraceXu96:master",
                Base = "master",
                Body = "Please pull this in!"
            };
            string reqBody = JsonConvert.SerializeObject(pullRequestBody);
            string pr_res = JObject.Parse(Post(url_pullRequest, reqBody)).ToString();
            Console.WriteLine(pr_res);
        }

        private static void Commit()
        {
            Console.WriteLine("******************Six steps for Commit***************************");
            //get reference & tree to commit 
            string parent_sha = JObject.Parse(Get(url_getRef, new Dictionary<string, string>()))["object"]["sha"].ToString();
            string baseTree_sha = JObject.Parse(Get(url_getCommit + parent_sha, new Dictionary<string, string>()))["tree"]["sha"].ToString();

            //create a  blob
            StreamReader sr = new StreamReader(sourceFile, Encoding.GetEncoding("utf-8"));
            CreateBlobRequest createBlobRequest = new CreateBlobRequest
            {
                Content = sr.ReadToEnd(),
                Encoding = "utf-8"
            };
            string createBlobBody = JsonConvert.SerializeObject(createBlobRequest);
            string blob_sha = JObject.Parse(Post(url_createBlob, createBlobBody))["sha"].ToString();

            //create a new tree for commit
            CreateTreeRequest createTreeRequest = new CreateTreeRequest
            {
                BaseTree = baseTree_sha,
                Tree = new TreeNode[] {
                    new TreeNode{
                        Path = @"node-azure-tools.md",
                        Mode = "100644",
                        Type = "blob",
                        Sha = blob_sha
                    }
                }
            };
            string createTreeBody = JsonConvert.SerializeObject(createTreeRequest);
            string treeSubmit_sha = JObject.Parse(Post(url_createTree, createTreeBody))["sha"].ToString();

            //create a  new commit
            CreateCommitRequest createCommitRequest = new CreateCommitRequest
            {
                Message = "Commit automatically!",
                Parents = new string[] { parent_sha },
                Tree = treeSubmit_sha
            };
            string createCommitBody = JsonConvert.SerializeObject(createCommitRequest);
            string createSubmit_sha = JObject.Parse(Post(url_createCommit, createCommitBody))["sha"].ToString();

            //update reference
            UpdateReferenceRequest updateReferenceRequest = new UpdateReferenceRequest {
                Sha = createSubmit_sha,
                Force = true
            };
            string updateReferenceBody = JsonConvert.SerializeObject(updateReferenceRequest);
            string updateRef_res = Post(url_updateRef, updateReferenceBody).ToString();
        }

        private static string Post(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/vnd.github.v3+json";
            req.Headers.Add("Authorization", "token b908f03792a8a7e90b924d6b643e87b2ad14d69b");
            req.UserAgent = "Code Sample Web Client";            
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(content);
            }                      

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();           
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


    
        private static string Get(string url, Dictionary<string, string> dic)
        {
            string result = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(url);

            if (dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
            req.ContentType = "application/vnd.github.v3+json";
            req.Headers.Add("Authorization", "token b908f03792a8a7e90b924d6b643e87b2ad14d69b");
            req.UserAgent = "Code Sample Web Client";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();
            try
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
    }
}
