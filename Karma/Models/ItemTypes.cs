using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public struct ItemTypes
    {
        private static Dictionary<int, ItemType> _types;

        static ItemTypes()
        {
            if (_types == null)
            {
                _types = new Dictionary<int, ItemType>();
            }
        }

        public ItemType this[int i]
        {
            get { return _types[i]; }
            set { _types[i] = value; }
        }

        public static Dictionary<int, ItemType> Types { get => _types; set => _types = value; }
    }
}
