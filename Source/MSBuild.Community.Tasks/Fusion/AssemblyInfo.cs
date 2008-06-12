using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MSBuild.Community.Tasks.Fusion
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [CLSCompliant(false)]
    internal struct AssemblyInfo
    {
        public uint cbAssemblyInfo;
        public uint dwAssemblyFlags;
        public ulong uliAssemblySizeInKB;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszCurrentAssemblyPathBuf;
        public uint cchBuf;
    }
}
