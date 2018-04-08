namespace enigma
{    
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    #endregion

    #region GeneratedAPI
    public class GeneratedAPI : DynamicObject
    {
        #region Variables & Properties
        public string Id { get; private set; }

        public string Type { get; private set; }

        public string GenericType { get; private set; }

        public Session Session { get; private set; }

        public int Handle { get; private set; }
        #endregion

        #region Events
        public event EventHandler Changed;

        public event EventHandler Closed;

        internal void OnChanged()
        {
            Changed?.BeginInvoke(this, new EventArgs(), null, null);
        }

        internal void OnClosed()
        {
            Closed?.BeginInvoke(this, new EventArgs(), null, null);
        }
        #endregion

        #region Constructor
        public GeneratedAPI(string Id, string Type, string GenericType, Session Session, int Handle)
        {
            // ToDo: check if all Parameters are okay?
            this.Id = Id;
            this.Type = Type;
            this.GenericType = GenericType;
            this.Session = Session;
            this.Handle = Handle;
        }
        #endregion

        #region Dynamic Methods
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            CancellationToken cts = CancellationToken.None;
            JToken jToken = null;

            // ToDo: enhance parametercheck for enigma Parametermode with loaded schema file
            foreach (var item in args)
            {
                if (item is CancellationToken innerCTS)
                {
                    if (cts != CancellationToken.None)
                        throw new Exception();

                    cts = innerCTS;
                }

                if (item is JToken innerJToken)
                {
                    if (jToken != null)
                        throw new Exception();

                    jToken = innerJToken;
                }

                if (item is string JTokenString)
                {
                    // ToDo: if parameter mode is working Check for valid JToken and if not valid check if parameters mode function fits                    
                    if (jToken != null)
                        throw new Exception();

                    jToken = JToken.Parse(JTokenString);
                }
            }

            result = "";//Session.SendAsync(jToken, this.Handle, cts);

            return true;
        }
        #endregion
    } 
    #endregion
}
