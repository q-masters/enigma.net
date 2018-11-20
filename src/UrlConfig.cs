namespace enigma
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion
    /// <summary>
    /// The Url configuration class
    /// </summary>
    public class UrlConfig
    {
        #region Properties
        /// <summary>
        /// The host machine 
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// The port to connect.
        /// Sample: 4848, 443, 80 
        /// </summary>
        public int Port { get; set; } = 4848;

        /// <summary>
        /// Set to false to use an unsecure WebSocket connection (ws://).
        /// </summary>
        public bool Secure { get; set; } = true;

        /// <summary>
        /// Additional parameters to be added to WebSocket URL.
        /// </summary>
        public Dictionary<string, string> UrlParams { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Absolute base path to use when connecting, used for proxy prefixes.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// The ID of the app intended to be opened in the session.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Initial route to open the WebSocket against, default is app/engineData.
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Subpath to use, used to connect to dataprepservice in a server environment.
        /// </summary>
        public string SubPath { get; set; }

        /// <summary>
        /// Identity (session ID) to use.
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// A value in seconds that QIX Engine should keep the session alive
        /// after socket disconnect (only works if QIX Engine supports it).
        /// </summary>
        public int Ttl { get; set; } = 30;
        #endregion
    }
}
