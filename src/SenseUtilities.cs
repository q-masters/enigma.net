namespace enigma
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    #endregion

    /// <summary>
    /// The Sense Utilities is a helper class for enigma.net.
    /// </summary>
    public class SenseUtilities
    {
        #region Private Methods
        private static string GetQueryString(Dictionary<string, string> urlParams)
        {
            var builder = new StringBuilder();
            foreach (var urlParam in urlParams)
                builder.Append($"{urlParam.Key}={urlParam.Value}&");
            return builder.ToString().TrimEnd('&');
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Build a Url from url configuration parameter class
        /// </summary>
        /// <param name="urlConfig">Url configurations parameter class</param>
        /// <returns>The created url</returns>
        public static string BuildUrl(UrlConfig urlConfig = null)
        {
            if (urlConfig == null)
                urlConfig = new UrlConfig();

            var urlBuilder = new StringBuilder();
            urlBuilder.Append(urlConfig.Secure ? "wss" : "ws");
            urlBuilder.Append("://");
            urlBuilder.Append($"{urlConfig.Host}");

            if (String.IsNullOrEmpty(urlConfig.AppId) && String.IsNullOrEmpty(urlConfig.Route))
                urlConfig.Route = "app/engineData";

            urlBuilder.Append(urlConfig?.Port > 0 ? $":{urlConfig?.Port}" : "");
            urlBuilder.Append(String.IsNullOrEmpty(urlConfig?.Prefix) ? $"{urlConfig?.Prefix?.Trim('/')}" : "");
            urlBuilder.Append(String.IsNullOrEmpty(urlConfig?.SubPath) ? $"{urlConfig?.SubPath?.Trim('/')}" : "");

            if (!String.IsNullOrEmpty(urlConfig?.Route))
                urlBuilder.Append($"/{urlConfig?.Route?.Trim('/')}");
            else if (!String.IsNullOrEmpty(urlConfig?.AppId))
                urlBuilder.Append($"/app/{HttpUtility.UrlEncode(urlConfig?.AppId)}");
            if (!String.IsNullOrEmpty(urlConfig?.Identity))
                urlBuilder.Append($"/identity/{HttpUtility.UrlEncode(urlConfig?.Identity)}");
            if (urlConfig.Ttl >= 0)
                urlBuilder.Append($"/ttl/{urlConfig?.Ttl}/");
            if (urlConfig.UrlParams.Count > 0)
                urlBuilder.Append($"?{GetQueryString(urlConfig.UrlParams)}");
            return urlBuilder.ToString();
        }

        /// <summary>
        /// Build a full path from the qlik app path.
        /// </summary>
        /// <param name="appName">Name or Id of the qlik app</param>
        /// <returns>The full path to the qlik app</returns>
        public static string GetFullAppName(string appName)
        {
            if (String.IsNullOrEmpty(appName))
                return null;
            if (Guid.TryParse(appName, out var myguid))
                return appName;
            var fullPath = appName;
            if (!appName.ToLowerInvariant().StartsWith("%userprofile%") && !appName.Contains(":") &&
                !appName.StartsWith("\\\\") && !appName.StartsWith("/"))
                fullPath = $"%USERPROFILE%\\Documents\\Qlik\\Sense\\Apps\\{appName.Trim('\\')}";
            if (!Path.HasExtension(fullPath))
                fullPath = $"{fullPath}.qvf";
            fullPath = Environment.ExpandEnvironmentVariables(fullPath);
            return fullPath;
        }
        #endregion
    }
}