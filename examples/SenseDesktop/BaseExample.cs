using Qlik.EngineAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseDesktop
{
    public class BaseExample
    {
        public IGlobal Global { get; private set; }
        public string AppName { get; private set; }

        public BaseExample(IGlobal global, string appName)
        {
            Global = global;
            AppName = appName;
        }
    }
}
