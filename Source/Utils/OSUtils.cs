using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils;

internal static class OSUtils
{
    public static string LibraryExtension() =>
        OperatingSystem.IsWindows() ? "dll"   :
        OperatingSystem.IsLinux()   ? "so"    :
        OperatingSystem.IsMacOS()   ? "dylib" :
        string.Empty;

    public static string GetBassLibraryName() =>
        OperatingSystem.IsWindows() ? "bass" :
        OperatingSystem.IsLinux() ? "libbass" :
        OperatingSystem.IsMacOS() ? "libbass" :
        string.Empty;

    /* all are from LE BASS binaries observed in HxD */
    public static uint GetPEMagic() =>
        // ugly hack including part of the dos stub to keep compat with mac and linux using 32-bit magic over 16-bit magic
        OperatingSystem.IsWindows() ? 0x00905A4D :
        OperatingSystem.IsLinux() ? 0x464C457F :
        OperatingSystem.IsMacOS() ? 0xBEBAFECA :
        0;
}