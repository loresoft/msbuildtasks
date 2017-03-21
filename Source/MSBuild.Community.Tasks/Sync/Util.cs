using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace johnshope.Sync {

	public static class Streams {

		const int Size = 8*1024;

		public static void Copy(Stream src, Stream dest) {

#if NET4
			src.CopyTo(dest, Size);
#else
			byte[] buffer = new byte[Size];

			var len = src.Read(buffer, 0, Size);
			while (len > 0) {
				dest.Write(buffer, 0, len);
				len = src.Read(buffer, 0, Size);
			}
#endif
		}

		public static string Path(this Uri url) {
			string name = url.PathAndQuery;
			int i = name.IndexOf("%3F");
			if (i >= 0) name = name.Substring(0, i);
			return HttpUtility.UrlDecode(name);
		}

		public static string PathWithSlash(this Uri url) {
			string name = url.Path();
			if (!name.StartsWith("/")) name = "/" + name;
			if (!name.EndsWith("/")) name = name + "/";
			return name;
		}

		public static string File(this Uri url) {
			string file = Path(url);
			int i = file.LastIndexOf('/');
			if (i >= 0) file = file.Substring(i + 1);
			return file;
		}

		public static Hashtable Query(this Uri url) {
			string query = url.PathAndQuery;
			int i = query.IndexOf("%3F");
			if (i < 0) return new Hashtable();
			query = HttpUtility.UrlDecode(query.Substring(i+3));
			var parameters = query.Split('&');
			var table = new Hashtable();
			foreach (var p in parameters) {
				var tokens = p.Split('=');
				if (tokens.Length == 1) table[p] = true;
				else table[tokens[0]] = tokens[1];
			}
			return table;
		}

        public static Uri Relative(this Uri url, string subpath) {
            var path = url.PathWithSlash();
            if (subpath.StartsWith("/")) subpath = subpath.Substring(1);
            path = path + subpath;
            var query = url.PathAndQuery;
            int i = query.IndexOf("%3F");
            if (i >= 0) query = query.Substring(i+3);
            else query = url.Query;
            var urlroot = url.Scheme + "://";
            if (!string.IsNullOrEmpty(url.UserInfo)) urlroot += url.UserInfo + "@";
            urlroot += url.Authority;
            return new Uri(urlroot + path + "?" + query);
        }
	}
}
