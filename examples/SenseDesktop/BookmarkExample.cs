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
            return ((IActLikeProxy)@this)?.Original; ;
        }
    }

    //public class BookmarkListLayout : Qlik.EngineAPI.GenericObjectLayout
    //{        
    //    public BookmarkList qBookmarkList { get; set; }
    //}

    //public class BookmarkListProperties : Qlik.EngineAPI.GenericObjectProperties
    //{
    //    public BookmarkListDef qBookmarkListDef { get; set; }
    //}

    public class BookmarkExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public BookmarkExample(IGlobal global, string appName) : base(global, appName) { }

        public void ListBookmarks(string type = "bookmark")
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            try
            {
                // TODO: add more examples to show different possibilities
                //var pr = new BookmarkListProperties
                //{
                //    qInfo = new NxInfo() {
                //        qId = "myId",
                //        qType = "BookmarkList"
                //    },
                //    qBookmarkListDef = new BookmarkListDef() {
                //        qType = "bookmark",
                //        qData = new JObject()
                //    },
                //    qMetaDef = new NxMetaDef()
                //    {

                //    }                    
                //};
                //var obj = ((IActLikeProxy)res.Result).Original;
                //return (Task<JObject>)obj.GetLayoutAsync();

                // Define the Properties as JSON from a anonymous class
                var pr2 = JObject.FromObject(new
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
                });

                Global.OpenDocAsync(AppName)
                .ContinueWith((newApp) =>
                {
                    var doc = newApp.Result;
                    doc.CreateSessionObjectAsync(pr2)
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
                });
             
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"The method \"{nameof(ListBookmarks)}\" was failed.");           
            }
        }
    }
}
