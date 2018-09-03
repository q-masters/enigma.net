namespace SenseDesktop
{
    using Newtonsoft.Json.Linq;
    #region Usings
    using NLog;
    using Qlik.EngineAPI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class DimensionExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public DimensionExample(IDoc app) : base(app) { }

        public async Task ListDimensionsAsync()
        {
            try
            {
                // Define the Properties as JSON from a anonymous class
                var request = JObject.FromObject(new
                {
                    qProp = new
                    {
                        qInfo = new
                        {
                            qType = "DimensionList"
                        },
                        qDimensionListDef = new
                        {
                            qType = "dimension",
                            qData = new { }
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
                    var list = ret.qDimensionList;
                    foreach (var qitem in list.qItems)
                    {
                        logger.Info($"ID: {qitem.qInfo.qId} Title: {qitem.qMeta.title}");
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"The method \"{nameof(ListDimensionsAsync)}\" was failed.");
            }
        }
    }
}