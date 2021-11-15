using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class LogEvents
    {
        // Post events
        public const int GetPost = 1001;
        public const int CreatePost = 1002;
        public const int EditPost = 1003;
        public const int DeletePost = 1004;
        public const int ChangeState = 1005;

        // User events
        public const int UserNotAuthenticated = 2001;
        public const int UserLoggedIn = 2002;
        public const int UserLockedOut = 2003;

        // File events
        public const int AddImage = 3001;
    }
}
