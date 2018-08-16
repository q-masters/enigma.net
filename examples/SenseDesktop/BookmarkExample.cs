namespace SenseDesktop
{
    #region Usings
    using Qlik.EngineAPI;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;
    using Newtonsoft.Json.Linq;
    #endregion

    public class BookmarkListProperties : Qlik.EngineAPI.GenericObjectProperties
    {
        public BookmarkListDef qBookmarkListDef { get; set; }
    }

    public class BookmarkExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public BookmarkExample(IGlobal global, string appName) : base(global, appName) { }

        public Task<string> GetBookmarkId(string value, string type = "bookmark")
        {
            try
            {
                var requestBuilder = new StringBuilder();
                requestBuilder.Append("{");
                requestBuilder.AppendLine("\"qInfo\": {");
                requestBuilder.AppendLine("\"qType\": \"BookmarkList\"");
                requestBuilder.AppendLine("},");
                requestBuilder.AppendLine("\"qBookmarkListDef\": {");
                requestBuilder.AppendLine($"\"qType\": \"{type}\" ");
                requestBuilder.AppendLine("}");
                requestBuilder.AppendLine("}");
                var request = requestBuilder.ToString();

                var pr = new BookmarkListProperties();
                pr.qInfo = new NxInfo() { qId = "myId", qType = "BookmarkList" };
                pr.qBookmarkListDef = new BookmarkListDef() { qType = "bookmark", qData = new JObject() };

                Global.OpenDocAsync(AppName)
                .ContinueWith((newApp) =>
                {
                    var doc = newApp.Result;
                    doc.CreateSessionObjectAsync(pr)
                    .ContinueWith((res) =>
                    {
                        return res.Result.GetLayoutAsync();
                    });
                    //    .Unwrap()
                    //    .ContinueWith<string>((res2) =>
                    //    {
                    //        var list = (res2.Result as dynamic).qBookmarkList as BookmarkList;
                    //        foreach (var qitem in list.qItems)
                    //        {
                    //            var bid = qitem.qInfo.qId;
                    //            if (bid.ToLowerInvariant() == value.ToLowerInvariant())
                    //                return bid;
                    //            var bname = qitem.qMeta.qName.ToLowerInvariant();
                    //            if (bname.ToLowerInvariant() == value.ToLowerInvariant())
                    //                return bid;
                    //        }
                    //        return null;
                    //    })
                });

                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"The method \"{nameof(GetBookmarkId)}\" was failed.");
                return null;
            }
        }
    }
}
