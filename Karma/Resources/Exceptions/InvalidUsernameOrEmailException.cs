using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Resources.Exceptions
{
    [Serializable]
    public class InvalidUsernameOrEmailException : Exception
    {

        public InvalidUsernameOrEmailException()
            : base("Invalid username or email")
        {

        }

        public InvalidUsernameOrEmailException(string message)
            : base(message)
        {
        }

        public InvalidUsernameOrEmailException(string message, Exception inner)
            : base(message, inner)
        {
        }



    }
}
