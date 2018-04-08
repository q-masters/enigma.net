namespace enigma
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Text; 
    #endregion

    public class EnigmaConfigurations
    {
        

    }

    public class Enigma
    {
        public static Session Create(EnigmaConfigurations config)
        {
            var session = new Session();
            // ToDo -> set config to Session
            return session;
        }
    }
}
