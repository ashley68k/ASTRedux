namespace ASTRedux.Utils;

internal static class FileExtensions
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
