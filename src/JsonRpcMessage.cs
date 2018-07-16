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

    #region JsonRpcRequestMessage
    [JsonObject(MemberSerialization.OptIn)]
    internal class JsonRpcRequestMessage
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
    #endregion

    #region JsonRpcGeneratedAPIRequestMessage
    [JsonObject(MemberSerialization.OptIn)]
    internal class JsonRpcGeneratedAPIRequestMessage : JsonRpcRequestMessage
    {
        #region Properties
        [JsonProperty("handle")]
        public int Handle { get; set; }

        [DefaultValue(false)]
        [JsonProperty("delta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Delta { get; set; }
        #endregion
    }
    #endregion

    #region JsonRpcResponseMessage
    [JsonObject(MemberSerialization.OptIn)]
    internal class JsonRpcResponseMessage
    {
        #region Properties
        [JsonProperty("jsonrpc")]
        [DefaultValue("2.0")]
        public string JsonRpcVersion { get; set; }

        [JsonProperty("result")]
        public JToken Result { get; private set; }

        [JsonProperty("error")]
        public JToken Error { get; set; }

        [JsonProperty("id")]
        public int Id { get; private set; }
        #endregion
    }
    #endregion

    #region JsonRpcGeneratedAPIResponseMessage
    [JsonObject(MemberSerialization.OptIn)]
    internal class JsonRpcGeneratedAPIResponseMessage : JsonRpcResponseMessage
    {
        #region Properties
        [JsonProperty("change")]
        public List<int> Change { get; set; }

        [JsonProperty("closed")]
        public List<int> Closed { get; set; }
        #endregion
    } 
    #endregion
}
