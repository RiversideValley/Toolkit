namespace Riverside;

public struct Version : IEquatable<Version>, IComparable<Version>
{
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }
    public string PreRelease { get; }
    public string BuildMetadata { get; }

    public Version(int major, int minor, int patch, string preRelease = null, string buildMetadata = null)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
        BuildMetadata = buildMetadata;
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        var version = $"{Major}.{Minor}.{Patch}";
        if (!string.IsNullOrEmpty(PreRelease))
        {
            version += $"-{PreRelease}";
        }
        if (!string.IsNullOrEmpty(BuildMetadata))
        {
            version += $"+{BuildMetadata}";
        }
        return version;
    }

    public readonly bool Equals(Version other)
    {
        return Major == other.Major &&
               Minor == other.Minor &&
               Patch == other.Patch &&
               PreRelease == other.PreRelease &&
               BuildMetadata == other.BuildMetadata;
    }

    public override bool Equals(object obj)
    {
        return obj is Version other && Equals(other);
    }

#if !NETSTANDARD2_0
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Major, Minor, Patch, PreRelease, BuildMetadata);
    }
#endif

    public readonly int CompareTo(Version other)
    {
        int result = Major.CompareTo(other.Major);
        if (result != 0) return result;

        result = Minor.CompareTo(other.Minor);
        if (result != 0) return result;

        result = Patch.CompareTo(other.Patch);
        if (result != 0) return result;

        result = string.Compare(PreRelease, other.PreRelease, StringComparison.Ordinal);
        if (result != 0) return result;

        return string.Compare(BuildMetadata, other.BuildMetadata, StringComparison.Ordinal);
    }

    public static bool operator ==(Version left, Version right)
        => left.Equals(right);

    public static bool operator !=(Version left, Version right)
        => !(left == right);

    public static bool operator <(Version left, Version right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Version left, Version right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(Version left, Version right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(Version left, Version right)
        => left.CompareTo(right) >= 0;
}
