using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils.Helpers;

internal static class BASSHelpers
{
    public static bool IsBassPresent(string dir)
    {
        var bass = Directory.EnumerateFiles(dir)
            .Where(file => Path.GetFileName(file) == $"{OSUtils.GetBassLibraryName()}.{OSUtils.LibraryExtension()}");

        return bass.Any();
    }
}
