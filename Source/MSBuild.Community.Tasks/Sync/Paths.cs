using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace johnshope.Sync {

    public class Paths {

        private static bool MatchSingle(string pattern, string path) {
            var star = pattern.IndexOf('*');
            var slash = path.LastIndexOf('/');
            if (!pattern.Contains('/') && slash != -1) path = path.Substring(slash+1);
            if (star != -1) {
                return (star == 0 || path.StartsWith(pattern.Substring(0, star))) && (star == pattern.Length-1 || path.EndsWith(pattern.Substring(star+1)));
            } else {
                return pattern == path;
            }
        }
        /// <summary>
        /// Checks wether the path matches one of a comma or semicolon separated list of file patterns or a single file pattern.
        /// </summary>
        /// <param name="patterns">A comma or semicolon separared list of patterns or a single pattern</param>
        /// <param name="path">The path to check.</param>
        /// <returns>True if one of the patterns matches the path.</returns>
        public static bool Match(string patterns, string path) {
            if (patterns == null || path == null) return false;
            path = path.Replace('\\', '/');
            foreach (var p in patterns.Split(';', ',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s))) {
                if (MatchSingle(p, path)) return true;
            }
            return false;
        }

    }
}
