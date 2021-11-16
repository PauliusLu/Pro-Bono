using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Resources.Exceptions
{
    public class Class : Exception
    {

        public Class()
        {
        }

        public Class(string message)
            : base(message)
        {
        }

        public Class(string message, Exception inner)
            : base(message, inner)
        {
        }



    }
}
