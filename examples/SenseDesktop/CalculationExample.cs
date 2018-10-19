using NLog;
using Qlik.EngineAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenseDesktop
{
    public class CalculationExample : BaseExample
    {
        #region Logger
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        public CalculationExample(IDoc app) : base(app) { }

        public void CalcRandom(int count)
        {
            try
            {
                var number1 = new Random().Next(0, 10000);
                var number2 = new Random().Next(0, 10000);
                var taskList = new List<Task>();
                Parallel.For(1, count, index =>
                {
                    logger.Info($"Start task {index}");
                    var task = App.EvaluateExAsync($"{number1}+{number2}");
                    taskList.Add(task);
                    taskList.Add(App.AbortModalAsync(false));
                });

                Task.WaitAll(taskList.ToArray());
            }
            catch (Exception ex)
            {                
                logger.Error(ex, $"***** The method {nameof(CalcRandom)} was failed.", ex);
            }
        }
    }
}
