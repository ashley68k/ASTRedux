using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils
{
    static class FileExtensions
    {
        public static HashSet<string> ASTExt = new(StringComparer.OrdinalIgnoreCase)
        {
            ".ast", ".rSoundAst"
        };

        public static HashSet<string> SoundExt = new(StringComparer.OrdinalIgnoreCase)
        {
            ".rSoundSnd"
        };
    }
}
