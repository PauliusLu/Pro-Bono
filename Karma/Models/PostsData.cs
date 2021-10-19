using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class PostsData
    {
        public static double AvaragePost;
        public void PostAvarage(int PostCount, DateTime FirstPost)
        {
            DateTime currentTime = DateTime.UtcNow;
            double totalDays = (currentTime - FirstPost).TotalDays;
            AvaragePost = PostCount / totalDays;
            Console.WriteLine(AvaragePost);
        }
        public static int AvarageInt()
        {
            int AvarageInt = (int)AvaragePost;
            return AvarageInt;
        }
        public static string AvarageText()
        {
            string text;
            if (AvarageInt() == 0)
                text = "Less than one";
            else
                text = AvarageInt() + "+";
            return text;
        }
    }
}
