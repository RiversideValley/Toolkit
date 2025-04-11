using System.Reflection;

namespace Riverside.Extensions.Accountability;

/// <summary>
/// Indicates that a method, constructor, property, or field is unsafe. This class cannot be inherited.
/// </summary>
/// <remarks>
/// This may imply that a method should not be used, or that a method should be used with caution.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class UnsafeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeAttribute"/> class.
    /// </summary>
    public UnsafeAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeAttribute"/> class with a specified reason.
    /// </summary>
    /// <param name="reason">The reason why the member is considered unsafe.</param>
    public UnsafeAttribute(string reason)
    {
        Reason = reason;
    }

    /// <summary>
    /// The reason why the member is considered unsafe.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Determines whether the specified method is marked with the <see cref="UnsafeAttribute"/>.
    /// </summary>
    /// <param name="method">The method to check.</param>
    /// <returns><c>true</c> if the method is marked with the <see cref="UnsafeAttribute"/>; otherwise, <c>false</c>.</returns>
    public static bool IsUnsafe(MethodBase method) => method.GetCustomAttribute<UnsafeAttribute>() != null;
}
