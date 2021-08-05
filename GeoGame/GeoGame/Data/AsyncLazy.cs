using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GeoGame.Data
{
    public class AsyncLazy<T>
    {
        #region Fields

        private readonly Lazy<Task<T>> instance;

        #endregion Fields

        #region Constructors

        public AsyncLazy(Func<T> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public AsyncLazy(Func<Task<T>> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        #endregion Constructors

        #region Methods

        public TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }

        #endregion Methods
    }
}