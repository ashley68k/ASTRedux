namespace ASTRedux.Utils.Consts;

internal static class Ext
{
    public static HashSet<string> ASTExt = new(StringComparer.OrdinalIgnoreCase)
    {
        ".ast", ".rSoundAst"
    };

    public static HashSet<string> SoundExt = new(StringComparer.OrdinalIgnoreCase)
    {
        ".rSoundSnd", ".snd"
    };
}
