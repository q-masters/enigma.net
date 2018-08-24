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
        public IDoc App { get; private set; }

        public BaseExample(IDoc app)
        {
            App = app;
        }
    }
}
