using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEdit.JsonEntities
{
    class FragInfo
    {
        [JsonProperty("startline")]
        public int StartLine { get; set; }
        [JsonProperty("endline")]
        public int Endline { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
