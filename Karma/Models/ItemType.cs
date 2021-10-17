using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class ItemType
    {
        public static ItemTypes Types = new ItemTypes();

        int _id;
        string _name;
        string _imagePath;

        public override string ToString()
        {
            return Name;
        }
        public ItemType(string name, string imagePath, int id = -1)
        {
            Id = id;
            Name = name;
            ImagePath = imagePath;
        }

        public int Id { get => _id;
                        private set => _id = (value == -1) ? ItemTypes.Types.Max(it => it.Value._id) + 1 : _id = value; }
        public string Name { get => _name;
                             private set => _name = value; }

        public string ImagePath { get => _imagePath;
                                  private set => _imagePath = value; }

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
            return ItemTypes.Types.First(type => type.Value.Name == name).Value;
        }

        public static void CreateType(string name, string imagePath, int id = -1)
        {
            ItemTypes.Types.Add(id, new ItemType(name, imagePath, id));
        }
    }
}
