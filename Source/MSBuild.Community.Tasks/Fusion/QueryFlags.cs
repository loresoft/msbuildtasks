using System;

namespace MSBuild.Community.Tasks.Fusion
{
    [Flags]
    internal enum QueryFlags
    {
        None = 0,
        Validate = 1,
        GetSize = 2
    }
}
