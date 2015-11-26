using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Framework
{
    public static class IoUtils
    {
        public static string CompleteDirectoryPath(string directoryPath)
        {
            string result = directoryPath;

            if(result != null && result.Length > 0)
            {
                result += Path.DirectorySeparatorChar;
            }

            return result;
        }
    }
}
