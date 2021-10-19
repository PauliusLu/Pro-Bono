using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class Advert : Post
    {
        public bool NoAdverts = true;
        public Advert()
        {
            UserId = "Admin";
            IsDonation = false;
            Date = DateTime.UtcNow;
            Title = "Advertisement";
            ItemType = 5;
            IsVisible = true;
            Description = "This could be your advertisement";
        }

        public void ChangeVisible()
        {
            IsVisible = false;
        }
        public void ChangeNoAdverts()
        {
            NoAdverts = false;
        }
        

    }
}
