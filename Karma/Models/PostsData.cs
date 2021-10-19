using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class PostsData
    {
        public static double AveragePost;
        public void PostAverage(int PostCount, DateTime FirstPost)
        {
            DateTime currentTime = DateTime.UtcNow;
            double totalDays = (currentTime - FirstPost).TotalDays;
            AveragePost = PostCount / totalDays;
            Console.WriteLine(AveragePost);
        }
        public static int AverageInt()
        {
            int AverageInt = (int)AveragePost;
            return AverageInt;
        }
        public static string AverageText()
        {
            string text;
            if (AverageInt() == 0)
                text = "Less than 1";
            else
                text = AverageInt() + "+";
            return text;
        }
    }
}
