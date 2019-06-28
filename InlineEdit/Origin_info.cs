using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEdit
{
    class Origin_info
    {
        private int startline;
        private int endline;
        private String origin_url;

        public Origin_info(int startline, int endline, string origin_origin_url)
        {
            startline = startline;
            this.endline = endline;
            this.origin_url = origin_url;
        }

        public int startline { get => startline; set => startline = value; }
        public int endline { get => endline; set => endline = value; }
        public string origin_url { get => origin_url; set => origin_url = value; }
    }
}
