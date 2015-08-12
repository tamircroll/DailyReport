using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily
{
    static class BuildOutputHelper
    {
        public static int linesToAddToGetError(List<string> lines, int i)
        {
            if (lines[i + 4].Contains("RuntimeException: Test initialization failed: Unable to provision, see the following errors"))
            {
                return 8;
            }
            if (lines[i + 5].Contains("exception: "))
            {
                return 5;
            }

            return 4;
        }
    }
}
