using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class Advert : Post
    {
        public static readonly new string ImagesDirName = Path.Combine("data", "AdvertImages");

        public static Advert[] Samples = {
            new Advert(DateTime.Parse("10/10/2021"), "Searching for volunteers", "Our hospital needs additional helping hands.", 
                Path.Combine(ImagesDirName, "lig1.jpg"), "Call us: 852365000"),
            new Advert(DateTime.Parse("10/01/2021"), "UNICEF", "Join us to make a direct impact in the lives of the world’s most vulnerable children.",
                Path.Combine(ImagesDirName, "unicef.png"), "Become a regular donator")
        };


        public Advert(DateTime date, string title, string description, string fileName, string emphasized) 
            : base(-1, "?", false, date, title, 0, description, fileName, true)
        {
            EmphasizedMessage = emphasized;
        }

        public string EmphasizedMessage { get; private set; }

    }
}
