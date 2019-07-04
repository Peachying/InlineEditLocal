using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEdit.JsonEntities
{
    public class CreateTreeRequest
    {
        [JsonProperty("base_tree")]
        public string BaseTree { get; set; }
        [JsonProperty("tree")]
        public TreeNode[] Tree { get; set; }
    }
}
