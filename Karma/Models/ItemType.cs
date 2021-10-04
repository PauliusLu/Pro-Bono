using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class ItemType
    {
        public static Dictionary<int, ItemType> Types = new Dictionary<int, ItemType>();

        int id;
        string name;

        public ItemType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// Returns ItemType by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ItemType GetItemType(int id)
        {
            return Types[id];
        }
        /// <summary>
        /// Returns ItemType by name. It's resource heavy operation. If possible, use id.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ItemType GetItemType(string name)
        {
            return Types.First(type => type.Value.Name == name).Value;
        }

        public static void CreateType(int id, string name)
        {
            Types.Add(id, new ItemType(id, name));
        }
    }
}
