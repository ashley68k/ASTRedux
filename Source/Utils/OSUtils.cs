using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils;

internal static class OSUtils
{
    // it should be impossible to trigger the os exception unless somebody goes out of their way to build this for an unsupported platform. 
    // In such cases I would hardly expect the program to even work,
    // so I think this is safe enough, and is more to satisfy the requirements of a ternary expression
    public static string LibraryExtension() =>
        OperatingSystem.IsWindows() ? "dll"   :
        OperatingSystem.IsLinux()   ? "so"    :
        OperatingSystem.IsMacOS()   ? "dylib" :
        throw new PlatformNotSupportedException("Unsupported OS platform.");

    public static string GetBassLibraryName() =>
        OperatingSystem.IsWindows() ? "bass" :
        OperatingSystem.IsLinux() ? "libbass" :
        OperatingSystem.IsMacOS() ? "libbass" :
        throw new PlatformNotSupportedException("Unsupported OS platform.");

    public static string GetBassDownloadSuffix() =>
        OperatingSystem.IsWindows() ? string.Empty :
        OperatingSystem.IsLinux() ? "-linux" :
        OperatingSystem.IsMacOS() ? "-osx" :
        throw new PlatformNotSupportedException("Unsupported OS platform.");

    // mac bass ships a fat binary, no need to navigate to x64
    public static string GetBassZipPath() =>
        OperatingSystem.IsWindows() ? "x64" :
        OperatingSystem.IsLinux() ? "libs/x86_64" :
        OperatingSystem.IsMacOS() ? "" :
        throw new PlatformNotSupportedException("Unsupported OS platform.");

    public static string GetOSString() =>
        OperatingSystem.IsWindows() ? "Windows" :
        OperatingSystem.IsLinux() ? "Linux" :
        OperatingSystem.IsMacOS() ? "Mac OS" :
        throw new PlatformNotSupportedException("Unsupported OS platform.");

    /* all are from LE BASS binaries observed in HxD */
    public static uint GetPEMagic() =>
        // ugly hack including part of the dos stub to keep compat with mac and linux using 32-bit magic over 16-bit magic
        OperatingSystem.IsWindows() ? 0x00905A4D :
        OperatingSystem.IsLinux() ? 0x464C457F :
        OperatingSystem.IsMacOS() ? 0xBEBAFECA :
        throw new PlatformNotSupportedException("Unsupported OS platform.");
}