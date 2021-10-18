﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma
{
    public class Enums
    {
        public enum ActionType
        {
            Donate = 0,
            Request
        }

        public enum ReviewState
        {
            Waiting = 0,
            InReview,
            Approved,
            Declined
        }
    }

}
