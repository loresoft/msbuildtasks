using System;

namespace MSBuild.Community.Tasks.Fusion
{
    [Flags]
    internal enum CacheFlags
    {
        None = 0,
        Zap = 1,
        Gac = 2,
        Download = 4
    }
}
