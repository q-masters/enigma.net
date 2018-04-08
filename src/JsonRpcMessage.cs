namespace enigma
{   
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #endregion

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcRequestMessage
    {
        #region Constructor
        internal JsonRpcRequestMessage()
        {
            this.JsonRpcVersion = "2.0";
        }
        #endregion

        #region Properties
        [JsonProperty("jsonrpc")]
        public string JsonRpcVersion { get; private set; }
        
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public JToken Parameters { get; set; }
        
        [JsonProperty("id", Order = 1)]
        public int Id { get; set; }
        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcGeneratedAPIRequestMessage: JsonRpcRequestMessage
{
        #region Properties
        [JsonProperty("handle")]
        public int Handle { get; set; }

        [DefaultValue(false)]
        [JsonProperty("delta", DefaultValueHandling=DefaultValueHandling.Ignore)]
        public bool Delta { get; set; }
        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcResponseMessage
    {
        #region Properties
        [JsonProperty("jsonrpc")]
        public string JsonRpcVersion { get; set; }

        [JsonProperty("result")]
        public JToken Result { get; private set; }

        [JsonProperty("error")]
        public JToken Error { get; set; }

        [JsonProperty("id")]
        public int Id { get; private set; }
        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRpcGeneratedAPIResponseMessage: JsonRpcResponseMessage
    {
        #region Properties
        [JsonProperty("change")]
        public List<int> Change { get; set; }

        [JsonProperty("closed")]
        public List<int> Closed { get; set; }
        #endregion
    }
}
