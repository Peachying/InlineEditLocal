using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEdit.JsonEntities
{
    public class UpdateReferenceRequest
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }
        [JsonProperty("force")]
        public Boolean Force;
    }
}
