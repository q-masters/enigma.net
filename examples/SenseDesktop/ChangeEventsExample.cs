using Newtonsoft.Json.Linq;
using Qlik.EngineAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseDesktop
{
    public class SelectionEventArgs : EventArgs
    {
        public string State { get; set; }
    }

    public class ChangeEventsExample : BaseExample
    {
        private ConcurrentDictionary<string, IGenericObject> CachedCurrentSelection = null;

        public ChangeEventsExample(IDoc app) : base(app)
        {
            app?.GetAppLayoutAsync()
               .ContinueWith(async t =>
               {
                   #region Handle AlternateStates                 
                   var res = t.Result;
                   if (res != null)
                   {
                       var states = res.qStateNames?.ToList() ?? new List<string>();
                       states.Add("$");

                       if (CachedCurrentSelection == null)
                           CachedCurrentSelection = new ConcurrentDictionary<string, IGenericObject>();

                       var delList = from c in CachedCurrentSelection.Keys where !states.Contains(c) select c;
                       var addList = from c in states where !CachedCurrentSelection.Keys.Contains(c) select c;

                       foreach (var stateToAdd in addList)
                       {
                           var req = JObject.FromObject(new
                           {
                               qProp = new
                               {
                                   qInfo = new
                                   {
                                       qType = "CurrentSelection"
                                   },
                                   qSelectionObjectDef = new
                                   {
                                       qStateName = stateToAdd

                                   }
                               }
                           });

                           app?.CreateSessionObjectAsync(req)
                           .ContinueWith(ts =>
                           {
                               if (ts.Result != null)
                               {
                                   if (CachedCurrentSelection.TryAdd(stateToAdd, ts.Result))
                                   {
                                       ts.Result.Changed += CachedCurrentSelection_Changed;
                                       Console.WriteLine("Call manualy");
                                       CachedCurrentSelection_Changed(ts.Result, new SelectionEventArgs() { State = stateToAdd });
                                   }
                               }
                           }).Wait();
                       }
                   }
                   #endregion
               }).Wait();
        }

        public void RunExample()
        {
            var listbox = CreateListBox(App, "Region");
            listbox.SelectListObjectValuesAsync("/qListObjectDef", new List<int>() { 1 }, true).Wait();
        }

        public void CachedCurrentSelection_Changed(object sender, EventArgs e)
        {
            var obj = sender as IGenericObject;
            Console.WriteLine(obj.ToString());
            //this cast should work!! So GetLayout can be called.
            obj.GetLayoutAsync().Wait();
        }

        private IGenericObject CreateListBox(IDoc appIDoc, string FieldName)
        {
            IGenericObject listbox = null;
            var request = JObject.FromObject(new
            {
                qProp = new
                {
                    qInfo = new
                    {
                        qType = "ListObject"
                    },
                    qListObjectDef = new
                    {
                        qInitialDataFetch = new List<NxPage>
                        {
                            new NxPage() { qTop = 0, qHeight = 4000, qLeft = 0, qWidth = 1 }
                        },
                        qDef = new
                        {
                            qFieldDefs = new List<string>
                            {
                                FieldName,
                            },
                            qFieldLabels = new List<string>
                            {
                                FieldName,
                            }
                        },
                        qShowAlternatives = false,
                    }
                }
            });

            appIDoc.CreateSessionObjectAsync(request).ContinueWith((res) =>
                {
                    listbox = res.Result;
                }).Wait();

            return listbox;
        }
    }
}
