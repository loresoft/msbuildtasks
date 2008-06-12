using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MSBuild.Community.Tasks.Fusion
{
    [ComImport]
    [Guid("9E3AAEB4-D1CD-11D2-BAB9-00C04F8ECEAE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CLSCompliant(false)]
    internal interface IAssemblyCacheItem
    {
        [return: MarshalAs(UnmanagedType.Error)]
        int CreateStream(
            uint flags,
            [MarshalAs(UnmanagedType.LPWStr)] string streamName,
            uint format,
            uint formatFlags,
            out IStream stream,
            ref long maxSize);

        [return: MarshalAs(UnmanagedType.Error)]
        int Commit(
            uint flags,
            out long disposition);

        [return: MarshalAs(UnmanagedType.Error)]
        int AbortItem();
    }
}
