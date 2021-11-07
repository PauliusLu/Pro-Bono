using Newtonsoft.Json;
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

        /// <summary>
        /// Returns ItemType by name. It's resource heavy operation. If possible, use id.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ItemType GetItemType(string name)
        {
            return _types.First(type => type.Value.Name == name).Value;
        }

        /// <summary>
        /// Returns ItemType by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ItemType GetItemType(int id)
        {
            return Types[id];
        }

        public static void CreateType(string name, string imagePath, int id = -1)
        {
            ItemTypes.Types.Add(id, new ItemType(name, imagePath, id));
        }

        public static void LoadItemTypes(string path)
        {
            string types = System.IO.File.ReadAllText(path);
            Types = JsonConvert.DeserializeObject<Dictionary<int, ItemType>>(types);
        }

        public static void SaveItemTypes(string path)
        {
            string types = JsonConvert.SerializeObject(Types);
            System.IO.File.WriteAllText(path, types);
        }
    }
}
