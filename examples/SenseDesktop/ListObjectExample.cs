namespace SenseDesktop
{
    #region Usings
    using Qlik.EngineAPI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using NLog;
    #endregion

    public class ListObjectExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public ListObjectExample(IDoc app) : base(app) { }

        public async Task ListListObjectDataAsync(string filterText = null)
        {
            try
            {
                if (String.IsNullOrEmpty(filterText))
                    filterText = "Region";

                // Define the Properties as JSON from a anonymous class
                var request = JObject.FromObject(new
                {
                    qInfo = new
                    {
                        qType = "ListObject"
                    },
                    qListObjectDef = new
                    {
                        qInitialDataFetch = new List<NxPage>
                        {
                            new NxPage() { qTop = 0, qHeight = 3, qLeft = 0, qWidth = 1 }
                        },
                        qDef = new
                        {
                            qFieldDefs = new List<string>
                            {
                                filterText,
                            },
                            qFieldLabels = new List<string>
                            {
                                Guid.NewGuid().ToString(),
                            },
                            qSortCriterias = new List<SortCriteria>
                            {
                                new SortCriteria() { qSortByState = 1 },
                            }
                        },
                        qShowAlternatives = false,
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
                    var list = ret.qListObject;
                    foreach (var qpage in list.qDataPages)
                    {
                        var matrix = qpage.qMatrix;
                        foreach (var cellRows in matrix)
                        {
                            foreach (var cellRow in cellRows)
                            {
                                logger.Info($"Text: {cellRow.qText} - State: {cellRow.qState}");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"The method \"{nameof(ListListObjectDataAsync)}\" was failed.");
            }
        }
    }
}
