using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;

namespace MSBuild.Community.Tasks.Fusion
{
    [CLSCompliant(false)]
    public static class FusionWrapper
    {
        private static string[] proccessors = new string[] { "MSIL", "x86", "AMD64" };

        public static void InstallAssembly(string assemblyPath, bool force)
        {
            IAssemblyCache assemblyCache = null;

            int flags = force ? (int)CommitFlags.Force : (int)CommitFlags.Refresh;

            ThrowOnError(NativeMethods.CreateAssemblyCache(out assemblyCache, 0));
            ThrowOnError(assemblyCache.InstallAssembly(flags, assemblyPath, null));
        }

        public static bool UninstallAssembly(string assemblyName, bool force)
        {
            UninstallStatus result = UninstallStatus.Uninstalled;
            return UninstallAssembly(assemblyName, force, out result);
        }

        public static bool UninstallAssembly(string assemblyName, bool force, out UninstallStatus result)
        {
            result = UninstallStatus.None;

            string fullName;
            string fullPath = GetAssemblyPath(assemblyName, out fullName);
            if (string.IsNullOrEmpty(fullPath))
            {
                result = UninstallStatus.ReferenceNotFound;
                return true;
            }

            IAssemblyCache cache = null;
            ThrowOnError(NativeMethods.CreateAssemblyCache(out cache, 0));

            int flags = force ? (int)CommitFlags.Force : (int)CommitFlags.Refresh;

            ThrowOnError(cache.UninstallAssembly(flags, fullName, null, out result));

            bool successful = false;

            switch (result)
            {
                case UninstallStatus.Uninstalled:
                case UninstallStatus.AlreadyUninstalled:
                case UninstallStatus.DeletePending:
                    successful = true;
                    break;
            }

            return successful;
        }

        public static string GetAssemblyPath(string assemblyName)
        {
            string fullName;
            return GetAssemblyPath(assemblyName, out fullName);
        }

        internal static string GetAssemblyPath(string assemblyName, out string fullName)
        {
            IAssemblyCache cache = null;
            ThrowOnError(NativeMethods.CreateAssemblyCache(out cache, 0));

            AssemblyInfo info = new AssemblyInfo
            {
                cbAssemblyInfo = (uint)Marshal.SizeOf(typeof(AssemblyInfo))
            };

            //ProcessorArchitecture required for this call
            fullName = assemblyName;

            if (HasProcessorArchitecture(fullName))
            {
                //getting size of string, cchBuf will be the size
                cache.QueryAssemblyInfo(3, fullName, ref info);
            }
            else
            {
                //try using possible proccessors
                foreach (string p in proccessors)
                {
                    fullName = AppendProccessor(assemblyName, p);
                    cache.QueryAssemblyInfo(3, fullName, ref info);

                    //if no size, not found, try another proccessor
                    if (info.cchBuf > 0)
                        break;
                }
            }
            //if no size, not found
            if (info.cchBuf == 0)
                return null;

            //get path
            info.pszCurrentAssemblyPathBuf = new string(new char[info.cchBuf]);
            ThrowOnError(cache.QueryAssemblyInfo(3, fullName, ref info));

            return info.pszCurrentAssemblyPathBuf;
        }

        public static AssemblyName GetAssemblyName(string assemblyName)
        {
            string filePath = GetAssemblyPath(assemblyName);
            AssemblyName result = AssemblyName.GetAssemblyName(filePath);
            return result;
        }

        private static void ThrowOnError(int hr)
        {
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);
        }

        internal static string AppendProccessor(string fullName)
        {
            return AppendProccessor(fullName, CurrentProcessorArchitecture);
        }

        internal static string AppendProccessor(string fullName, ProcessorArchitecture targetProcessor)
        {
            string processor = targetProcessor == ProcessorArchitecture.None ? "MSIL" : targetProcessor.ToString();
            return AppendProccessor(fullName, processor);
        }

        internal static string AppendProccessor(string fullName, string targetProcessor)
        {
            if (HasProcessorArchitecture(fullName))
                return fullName;

            return fullName + ", ProcessorArchitecture=" + targetProcessor;
        }

        private static bool HasProcessorArchitecture(string fullName)
        {
            return fullName.IndexOf("ProcessorArchitecture=", 0, 
                fullName.Length, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string _currentProcessorArchitecture;
        private static object _lock = new object();

        internal static string CurrentProcessorArchitecture
        {
            get
            {
                if (string.IsNullOrEmpty(_currentProcessorArchitecture))
                {
                    lock (_lock)
                    {
                        if (string.IsNullOrEmpty(_currentProcessorArchitecture))
                            _currentProcessorArchitecture = GetProcessorArchitecture();
                    }
                }

                return _currentProcessorArchitecture;
            }
        }

        private static string GetProcessorArchitecture()
        {
            SYSTEM_INFO lpSystemInfo = new SYSTEM_INFO();
            NativeMethods.GetSystemInfo(ref lpSystemInfo);

            switch (lpSystemInfo.wProcessorArchitecture)
            {
                case 0: return "x86";
                case 6: return "IA64";
                case 9: return "AMD64";
                default: return "MSIL";
            }
        }
    }
}
