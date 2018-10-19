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
    using ImpromptuInterface;
    #endregion

    // in einen anderen Namespace packen, so dass man es sich hinzufügen kann.
    public static class GeneratedAPIExtensions
    {
        public static dynamic Original(this IObjectInterface @this)
        {            
            return ((IActLikeProxy)@this)?.Original;
        }
    }

    public class BookmarkExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public BookmarkExample(IDoc app) : base(app) { }

        public async Task ListBookmarksAsync(string type = "bookmark")
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            try
            {
                // Define the Properties as JSON from a anonymous class
                var request = JObject.FromObject(new
                {
                    qProp = new
                    {
                        qInfo = new
                        {
                            qType = "BookmarkList"
                        },
                        qBookmarkListDef = new
                        {
                            qType = type, // default use "bookmark"
                            qData = new
                            {
                                title = "/qMetaDef/title"
                            }
                        }
                    }
                });
                
                await App.CreateSessionObjectAsync(request)
                .ContinueWith((res) =>
                {
                    return res.Result.GetLayoutAsync<JObject>();
                })
                .Unwrap()
                .ContinueWith((res2) =>
                {
                    var ret = res2.Result as dynamic;
                    var list = ret.qBookmarkList;
                    foreach (var qitem in list.qItems)
                    {
                        logger.Info($"ID: {qitem.qInfo.qId} Title: {qitem.qMeta.title}");
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"The method \"{nameof(ListBookmarksAsync)}\" was failed.");
            }
        }
    }
}