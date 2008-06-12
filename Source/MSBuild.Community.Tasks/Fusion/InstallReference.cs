using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MSBuild.Community.Tasks.Fusion
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [CLSCompliant(false)]
    internal struct InstallReference
    {
        public uint Size;
        public uint Flags;
        public Guid Scheme;
        public string Identifier;
        public string NonCannonicalData;
    }
}
