using System;

namespace MSBuild.Community.Tasks.Fusion
{
    public enum UninstallStatus
    {
        None = 0,
        Uninstalled = 1,
        StillInUse = 2,
        AlreadyUninstalled = 3,
        DeletePending = 4,
        HasInstallReferences = 5,
        ReferenceNotFound = 6
    }
}
