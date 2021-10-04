using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma
{
    public class Utils
    {
        public static bool IsValidExtension(string ext, string[] validExtensions = null)
        {
            validExtensions = validExtensions ?? new string[] { ".jpg", ".png", ".jpeg", ".bmp"};

            foreach (string validExt in validExtensions)
            {
                if (validExt == ext)
                    return true;
            }

            return false;
        }
    }
}
