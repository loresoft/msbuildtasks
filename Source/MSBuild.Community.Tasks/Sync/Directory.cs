using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace johnshope.Sync {

	public class Directory {

		public static IDirectory Parse(Uri url, SyncJob job) {
			if (url.IsFile || !url.ToString().Contains(':')) return new LocalDirectory(null, url, job);
			else return new FtpDirectory(null, url, job);
		}

	}
}
