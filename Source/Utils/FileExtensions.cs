using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils
{
    static class FileExtensions
    {
        public static HashSet<string> ASTExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".ast", ".rSoundAst"
        };

        public static HashSet<string> SoundExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".rSoundSnd"
        };

        public static HashSet<string> InputExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3", ".aac", ".wav"
        };

        public static HashSet<string> OutputExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".wav"
        };
    }
}
