using System;
using System.Collections.Generic;
using System.Linq;

namespace RichTextBlockCompat;

public enum MatchStatus
{
    /// <summary>Member exists on subject type with an equivalent signature.</summary>
    Match,
    /// <summary>A member with the same name and kind exists, but its signature differs.</summary>
    SignatureMismatch,
    /// <summary>No member with the same name and kind exists on the subject type.</summary>
    Missing,
}

public sealed record MemberCheck(ApiMember Reference, ApiMember? Found, MatchStatus Status);

public sealed record TypeCheckResult(
    string WinUITypeName,
    string LocalTypeName,
    List<MemberCheck> Members)
{
    public int Total => Members.Count;
    public int Matched => Members.Count(m => m.Status == MatchStatus.Match);
    public int Mismatched => Members.Count(m => m.Status == MatchStatus.SignatureMismatch);
    public int Missing => Members.Count(m => m.Status == MatchStatus.Missing);

    public double Percentage => Total == 0 ? 100.0 : Matched * 100.0 / Total;

    /// <summary>Gap members broken down by kind, useful for "where to focus next".</summary>
    public Dictionary<MemberKind, int> GapsByKind() =>
        Members.Where(m => m.Status != MatchStatus.Match)
               .GroupBy(m => m.Reference.Kind)
               .ToDictionary(g => g.Key, g => g.Count());
}

public sealed class CompatChecker
{
    private readonly ApiSurfaceExtractor _winUIExtractor;
    private readonly ApiSurfaceExtractor _subjectExtractor;
    private readonly MetadataLoader _loader;

    public CompatChecker(MetadataLoader loader, ApiSurfaceExtractor winUIExtractor, ApiSurfaceExtractor subjectExtractor)
    {
        _loader = loader;
        _winUIExtractor = winUIExtractor;
        _subjectExtractor = subjectExtractor;
    }

    /// <summary>
    /// Compare WinUI 3's public API surface against the local subject, member by member.
    /// Only WinUI → Local is checked: extra members on the subject side are not violations.
    /// </summary>
    public TypeCheckResult Check(TypePair pair)
    {
        var winuiType = _loader.GetWinUIType(pair.WinUITypeName);
        var localType = _loader.GetSubjectType(pair.LocalTypeName)
                        ?? throw new InvalidOperationException(
                            $"Subject type not found in LeXtudio assemblies: {pair.LocalTypeName}");

        var winuiMembers = _winUIExtractor.Extract(winuiType);
        var localMembers = _subjectExtractor.Extract(localType);

        var byNameKind = localMembers
            .GroupBy(m => (m.Name, m.Kind))
            .ToDictionary(g => g.Key, g => g.ToList());

        var checks = new List<MemberCheck>();
        foreach (var w in winuiMembers)
        {
            if (!byNameKind.TryGetValue((w.Name, w.Kind), out var candidates))
            {
                checks.Add(new MemberCheck(w, null, MatchStatus.Missing));
                continue;
            }

            var exact = candidates.FirstOrDefault(c => c.Signature == w.Signature);
            if (exact is not null)
            {
                checks.Add(new MemberCheck(w, exact, MatchStatus.Match));
            }
            else
            {
                checks.Add(new MemberCheck(w, candidates[0], MatchStatus.SignatureMismatch));
            }
        }

        return new TypeCheckResult(
            WinUITypeName: pair.WinUITypeName,
            LocalTypeName: pair.LocalTypeName,
            Members: checks);
    }
}
