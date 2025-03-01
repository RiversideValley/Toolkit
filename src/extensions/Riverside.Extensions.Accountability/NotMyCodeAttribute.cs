namespace Riverside.Extensions.Accountability;

[AttributeUsage(AttributeTargets.All)]
public class NotMyCodeAttribute : Attribute
{
    /// <summary>
    /// Generic indication that the code is not owned by you.
    /// </summary>
    public NotMyCodeAttribute()
    {
    }
/*
    public NotMyCodeAttribute(Spdx license)
    {
        License = license;
    }*/

    public string License { get; }
}
