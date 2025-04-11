using System;

namespace Riverside.Extensions.WinUI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public partial class InitializeComponentAttribute : Attribute
    {
        public InitializeComponentAttribute()
        {
        }
    }
}
