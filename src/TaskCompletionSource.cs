namespace enigma
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    #region TaskCompletionSource
    /// <summary>
    /// TaskCompletionSource adapter Class that allow to define the
    /// Type in the Constructor as parameter
    /// </summary>
    public class TaskCompletionSource
    {
        private readonly object taskCompletionSource = null;
        private Type gTCS;

        /// <summary>
        /// The underlaying Task for the TaskCompletionSource
        /// </summary>
        public Task Task
        {
            get
            {
                return gTCS.GetProperty("Task").GetValue(taskCompletionSource) as Task;
            }
        }

        /// <summary>
        /// Set the result on the Task
        /// </summary>
        /// <param name="result">Result for of the Task</param>
        public void SetResult(object result)
        {
            gTCS.GetMethod("SetResult").Invoke(taskCompletionSource, new object[] { result });
        }

        /// <summary>
        /// Set an exception on the Task
        /// </summary>
        /// <param name="exception">Exception that occured</param>
        public void SetException(Exception exception)
        {
            gTCS.GetMethod("SetException", new[] { typeof(Exception) }).Invoke(taskCompletionSource, new object[] { exception });
        }

        /// <summary>
        /// Set an exception on the Task
        /// </summary>
        /// <param name="exceptions">IEnumerable of Exceptions that occured</param>
        public void SetException(IEnumerable<Exception> exceptions)
        {
            gTCS.GetMethod("SetException", new[] { typeof(IEnumerable<Exception>) }).Invoke(taskCompletionSource, new object[] { exceptions });
        }

        /// <summary>
        /// Set an Cancel on the Task
        /// </summary>        
        public void SetCanceled()
        {
            gTCS.GetMethod("SetCanceled").Invoke(taskCompletionSource, null);
        }

        /// <summary>
        /// Constructor of TaskCompletionSource
        /// </summary>
        /// <param name="genericArgument">The generic type of the complement to TaskCompletionSource&lt;Type&gt;</param>
        public TaskCompletionSource(Type genericArgument)
        {
            gTCS = typeof(TaskCompletionSource<>).MakeGenericType(genericArgument);
            taskCompletionSource = Activator.CreateInstance(gTCS);
        }
    } 
    #endregion
}
