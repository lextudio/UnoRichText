namespace MS.Internal.PtsHost.UnsafeNativeMethods;

public static class PTS
{
    public const double MaxPageSize = 1_000_000d;
    public const double MaxFontSize = 1_000_000d;

    public static class Restrictions
    {
        public const int tscLineInParaRestriction = 1_000_000;
    }
}
