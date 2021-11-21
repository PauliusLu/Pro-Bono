using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Resources.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException()
            : base("Invalid login attempt.")
        {

        }

        public InvalidLoginException(string message)
            : base(message)
        {
        }

        public InvalidLoginException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
