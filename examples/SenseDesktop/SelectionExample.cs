namespace SenseDesktop
{ 
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using NLog;
    using Qlik.EngineAPI;
    #endregion

    public class SelectionExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public SelectionExample(IDoc app) : base(app) { }

        public async Task ListCurrentSelectionsAsync()
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
                            qType = "CurrentSelection"
                        },
                        qSelectionObjectDef = new { }
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
                    var list = ret.qSelectionObject;
                    foreach (var qitem in list.qSelections)
                    {
                        logger.Info($"Field: {qitem.qInfo.qField} Selected: {qitem.qMeta.qSelected}");
                    }
                });

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"The method \"{nameof(ListCurrentSelectionsAsync)}\" was failed.");
            }
        }
    }
}