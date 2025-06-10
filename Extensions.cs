using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTeroid
{
    static class Extensions
    {
        public static HashSet<string> ASTExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ast", "rSoundAst"
        };

        public static HashSet<string> CommonExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mp3", "wav", "ogg", "flac"
        };
    }
}
