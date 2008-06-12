using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MSBuild.Community.Tasks.Fusion
{
	internal static class NativeMethods
	{

		[DllImport("fusion.dll", SetLastError = true, PreserveSig = false)]
		public static extern int CreateAssemblyCache(
			out IAssemblyCache assemblyCache,
			uint reserved);

		[DllImport("fusion.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern int GetCachePath(
			CacheFlags assemblyCacheFlags,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder cachePath,
			ref int cachePathSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);
 
	}
}
