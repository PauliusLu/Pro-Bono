using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public struct ItemTypes
    {
        public static readonly string ItemTypesPath = "Data";
        public static readonly string ItemTypesFileName = "ItemTypes.txt";

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

        public static void OnChangedEventHandler(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            Startup.LoadItemTypes(Path.Combine(ItemTypesPath, ItemTypesFileName));
        }
    }
}
