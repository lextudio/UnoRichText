using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RichTextBlockCompat;

public enum MemberKind
{
    Property,
    Method,
    Event,
    DependencyPropertyField,
}

/// <summary>
/// Normalized representation of a single public API member, suitable for cross-type comparison.
/// Signature uses canonical short type names so equivalent members compare equal regardless of
/// which assembly defines the type referenced in the signature.
/// </summary>
public sealed record ApiMember(
    string Name,
    MemberKind Kind,
    string Signature,
    string DeclaringType,
    bool IsStatic);

/// <summary>
/// Extracts the public API surface of a Type loaded via <see cref="MetadataLoadContext"/>.
/// Includes inherited members, excluding members declared on <see cref="object"/>, obsolete
/// members, and accessor methods (those surface as properties/events instead).
/// </summary>
public sealed class ApiSurfaceExtractor
{
    private const BindingFlags AllInstanceAndStatic =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

    private readonly string? _dependencyPropertyFullName;

    public ApiSurfaceExtractor(Type? dependencyPropertyType)
    {
        _dependencyPropertyFullName = dependencyPropertyType?.FullName;
    }

    public List<ApiMember> Extract(Type type)
    {
        var members = new List<ApiMember>();

        foreach (var p in type.GetProperties(AllInstanceAndStatic))
        {
            if (IsObsolete(p) || ShouldSkip(p.DeclaringType)) continue;
            members.Add(new ApiMember(
                Name: p.Name,
                Kind: MemberKind.Property,
                Signature: $"{FormatType(p.PropertyType)} {p.Name} {{ {AccessorSummary(p)} }}",
                DeclaringType: p.DeclaringType?.FullName ?? "?",
                IsStatic: (p.GetMethod ?? p.SetMethod)?.IsStatic ?? false));
        }

        foreach (var e in type.GetEvents(AllInstanceAndStatic))
        {
            if (IsObsolete(e) || ShouldSkip(e.DeclaringType)) continue;
            members.Add(new ApiMember(
                Name: e.Name,
                Kind: MemberKind.Event,
                Signature: $"event {FormatType(e.EventHandlerType!)} {e.Name}",
                DeclaringType: e.DeclaringType?.FullName ?? "?",
                IsStatic: e.AddMethod?.IsStatic ?? false));
        }

        foreach (var m in type.GetMethods(AllInstanceAndStatic))
        {
            if (m.IsSpecialName) continue;            // property/event accessors
            if (IsObsolete(m) || ShouldSkip(m.DeclaringType)) continue;
            if (m.DeclaringType?.FullName == "System.Object") continue;
            members.Add(new ApiMember(
                Name: m.Name,
                Kind: MemberKind.Method,
                Signature: $"{FormatType(m.ReturnType)} {m.Name}({FormatParams(m.GetParameters())})",
                DeclaringType: m.DeclaringType?.FullName ?? "?",
                IsStatic: m.IsStatic));
        }

        foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (IsObsolete(f) || ShouldSkip(f.DeclaringType)) continue;
            if (_dependencyPropertyFullName is null) continue;
            if (f.FieldType.FullName != _dependencyPropertyFullName) continue;
            members.Add(new ApiMember(
                Name: f.Name,
                Kind: MemberKind.DependencyPropertyField,
                Signature: $"static DependencyProperty {f.Name}",
                DeclaringType: f.DeclaringType?.FullName ?? "?",
                IsStatic: true));
        }

        return members.OrderBy(m => m.Kind).ThenBy(m => m.Name).ToList();
    }

    // ── helpers ───────────────────────────────────────────────────────

    private static bool ShouldSkip(Type? declaringType)
    {
        if (declaringType is null) return true;
        return declaringType.FullName == "System.Object";
    }

    private static bool IsObsolete(MemberInfo m)
    {
        // Use full-name match — MetadataLoadContext Type objects aren't reference-equal to
        // typeof(ObsoleteAttribute) in this runtime.
        foreach (var attr in m.GetCustomAttributesData())
        {
            if (attr.AttributeType.FullName == "System.ObsoleteAttribute") return true;
        }
        return false;
    }

    private static string AccessorSummary(PropertyInfo p)
    {
        var parts = new List<string>(2);
        if (p.GetMethod is { IsPublic: true }) parts.Add("get;");
        if (p.SetMethod is { IsPublic: true }) parts.Add("set;");
        return string.Join(" ", parts);
    }

    private static string FormatParams(ParameterInfo[] ps) =>
        string.Join(", ", ps.Select(p => $"{FormatType(p.ParameterType)} {p.Name}"));

    /// <summary>
    /// Canonical short type form. Two members compare as having the same signature if their
    /// types render to the same short name — keeping the comparison resilient to assembly
    /// differences (WinAppSDK vs LeXtudio) for types that share a simple name.
    /// </summary>
    public static string FormatType(Type t)
    {
        if (t.IsByRef) return $"ref {FormatType(t.GetElementType()!)}";
        if (t.IsArray) return $"{FormatType(t.GetElementType()!)}[]";
        if (t.IsGenericType)
        {
            var name = t.Name;
            var tick = name.IndexOf('`');
            if (tick >= 0) name = name[..tick];
            var args = t.GetGenericArguments().Select(FormatType);
            return $"{name}<{string.Join(", ", args)}>";
        }
        return t.Name;
    }
}
