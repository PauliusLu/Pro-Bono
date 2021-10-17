using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Karma
{
    public static class Utils
    {
        public static bool IsValidExtension(this string ext, string[] validExtensions = null)
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
