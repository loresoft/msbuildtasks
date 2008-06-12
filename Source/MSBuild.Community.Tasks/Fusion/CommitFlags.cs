using System;

namespace MSBuild.Community.Tasks.Fusion
{
    [Flags]
    internal enum CommitFlags
    {
        None = 0,
        Refresh = 1,
        Force = 2
    }
}
