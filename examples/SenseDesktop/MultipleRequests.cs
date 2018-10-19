using Newtonsoft.Json.Linq;
using NLog;
using Qlik.EngineAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SenseDesktop
{
    public class MultipleRequests : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public MultipleRequests(IDoc app) : base(app) { }

        public async Task FireMultipleRequestsAsync()
        {
            #region Requests
            List<JObject> requests = new List<JObject>();
            requests.Add(JObject.FromObject(new
            {
                qProp = new
                {
                    qInfo = new
                    {
                        qType = "FieldList"
                    },
                    qFieldListDef = new
                    {
                        qShowSystem = false,
                        qShowHidden = true,
                        qShowDerivedFields = true,
                        qShowSemantic = true,
                        qShowSrcTables = true,
                        qShowImplicit = true
                    }
                }
            }));

            requests.Add(JObject.FromObject(new
            {
                qProp = new
                {
                    qInfo = new
                    {
                        qType = "BookmarkList"
                    },
                    qBookmarkListDef = new
                    {
                        qType = "bookmark"

                    }
                }
            }));
            requests.Add(JObject.FromObject(new
            {
                qProp = new
                {
                    qInfo = new
                    {
                        qType = "CurrentSelection"
                    },
                    qSelectionObjectDef = new
                    {
                        qStateName = "$"                    
                    }
                }
            }));
            requests.Add(JObject.FromObject(new
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
                        qData = new
                        {
                            info = "/qDimInfos",
                            qDimInfo = "/qDim"
                        }
                    }
                }
            }));

            #endregion
            foreach (var request in requests)
            {
                await App.CreateSessionObjectAsync(request)
                    .ContinueWith((res) =>
                    {
                        Console.WriteLine($"SesionObject for for {request["qProp"]["qInfo"]["qType"]} created");
                        return res.Result.GetLayoutAsync<JObject>();
                    })
                    .Unwrap()
                    .ContinueWith((res2) =>
                    {
                        if (res2.IsFaulted)
                            Console.WriteLine($"GetLayout for {request["qProp"]["qInfo"]["qType"]} Failed");
                        else
                            Console.WriteLine($"GetLayout for for {request["qProp"]["qInfo"]["qType"]} Successfull");
                    });
            }
        }
    }
}
