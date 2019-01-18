namespace tests
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using enigma;
    using ImpromptuInterface;
    using Newtonsoft.Json.Linq;
    using Qlik.EngineAPI;
    using Xunit;
    #endregion

    public class UnitTest1
    {
        IGlobal global;
        IDoc doc;

        public UnitTest1()
        {
            var session = Enigma.Create(new EnigmaConfigurations()
            {
                Url = $"ws://127.0.0.1:4848/app/engineData/identity/{Guid.NewGuid()}",
            });

            // connect to the engine
            var globalTask = session.OpenAsync().Result;
            global = Impromptu.ActLike<IGlobal>(globalTask);

            var appName = SenseUtilities.GetFullAppName("Executive Dashboard");
            //doc = global.OpenDocAsync(appName).Result;

            // TODO fix CreateSessionAppAsync
            doc = global.CreateSessionAppAsync().Result;
        }

        [Fact]
        public async Task Global_EngineVersion()
        {
            var engineVersion = await global.EngineVersionAsync();
            Assert.Equal("12", engineVersion.qComponentVersion.Substring(0, 2));
        }

        [Fact]
        public async Task Global_IsDesktop()
        {
            Assert.True(await global.IsDesktopModeAsync());
        }

        [Fact]
        async Task Doc_Evaluate()
        {
            Assert.Equal("42", await doc.EvaluateAsync("=40+2"));
        }

        //[Fact]
        //async Task Doc_Changed()
        //{
        //    bool changed = false;
        //    void Doc_Changed1(object sender, EventArgs e)
        //    {
        //        changed = true;
        //    }

        //    Assert.False(changed);
        //    doc.Changed += Doc_Changed1;
        //    await doc.SetScriptAsync(Guid.NewGuid().ToString());
        //    await Task.Delay(5000);
        //    Assert.True(changed);
        //    doc.Changed -= Doc_Changed1;
        //}



        //[Fact]
        //public async Task ReloadTest()
        //{
        //    var script = "";

        //    await doc.EvaluateAsync()

        //    await doc.SetScriptAsync(script);
        //    await doc.DoSaveAsync();
        //    Assert.Equal(script, await doc.GetScriptAsync());
        //    await doc.DoReloadAsync();

        //    Assert.True(await global.IsDesktopModeAsync());
        //}


    }
}
