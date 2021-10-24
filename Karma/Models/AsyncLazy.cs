using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy()
        {
        }

        public AsyncLazy(bool isThreadSafe) : base(isThreadSafe)
        {
        }

        public AsyncLazy(Func<Task<T>> valueFactory) : base(valueFactory)
        {
        }

        public AsyncLazy(LazyThreadSafetyMode mode) : base(mode)
        {
        }

        public AsyncLazy(Task<T> value) : base(value)
        {
        }

        public AsyncLazy(Func<Task<T>> valueFactory, bool isThreadSafe) : base(valueFactory, isThreadSafe)
        {
        }

        public AsyncLazy(Func<Task<T>> valueFactory, LazyThreadSafetyMode mode) : base(valueFactory, mode)
        {
        }
    }
}
