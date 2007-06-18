// $Id$
using System;
using Microsoft.Win32;

namespace MSBuild.Community.Tasks.Services
{
    /// <summary>
    /// The contract for a service that will provide access to the registry.
    /// </summary>
    /// <exclude />
    public interface IRegistry
    {
        /// <summary>
        /// Returns the names of the subkeys under the provided key.
        /// </summary>
        /// <param name="hive">The hive where <paramref name="key"/> is located.</param>
        /// <param name="key">The key to search.</param>
        /// <returns>A list of subkeys.</returns>
        string[] GetSubKeys(RegistryHive hive, string key);
        /// <summary>
        /// Returns the value of an entry in the registry.
        /// </summary>
        /// <param name="key">The key of the registry entry to return.</param>
        /// <returns>The value of the regisry entry.</returns>
        object GetValue(string key);
    }

    /// <summary>
    /// Provides access to the Windows registry.
    /// </summary>
    /// <exclude />
    public class Win32Registry : IRegistry
    {
        /// <summary>
        /// Returns the names of the subkeys under the provided key.
        /// </summary>
        /// <param name="hive">The hive where <paramref name="key"/> is located.</param>
        /// <param name="key">The key to search.</param>
        /// <returns>A list of subkeys.</returns>
        public string[] GetSubKeys(RegistryHive hive, string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the value of an entry in the registry.
        /// </summary>
        /// <param name="key">The key of the registry entry to return.</param>
        /// <returns>The value of the regisry entry.</returns>
        public object GetValue(string key)
        {
            throw new NotImplementedException();
        }
    }
}
