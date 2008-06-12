using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MSBuild.Community.Tasks.Fusion
{
    [ComImport]
    [Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CLSCompliant(false)]
    internal interface IAssemblyCache
    {
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        int UninstallAssembly(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)] string assemblyName,
            [MarshalAs(UnmanagedType.LPArray)] InstallReference[] references,
            [MarshalAs(UnmanagedType.U4)] out UninstallStatus disposition);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        int QueryAssemblyInfo(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)] string assemblyName,
            ref AssemblyInfo assemblyInfo);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        int CreateAssemblyCacheItem(
            int flags,
            IntPtr reserved,
            out IAssemblyCacheItem assemblyCacheItem,
            [MarshalAs(UnmanagedType.LPWStr)] string assemblyName);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        int CreateAssemblyScavenger(
            [MarshalAs(UnmanagedType.IUnknown)] out object assemblyScavenger);

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        int InstallAssembly(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)] string manifestFilePath,
            [MarshalAs(UnmanagedType.LPArray)] InstallReference[] references);
    }
}
