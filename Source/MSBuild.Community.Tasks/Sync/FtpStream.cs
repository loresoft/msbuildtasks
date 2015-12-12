using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Starksoft.Net.Ftp;

namespace johnshope.Sync {

	public class FtpStream: PipeStream {

		public SyncJob Job { get; set; }
		public Log Log { get { return Job.Log; } }

		public FtpClient Client { get; set; }
		public string Path { get; set; }
		public long Size { get; set; }
		DateTime start;
		public FtpStream(SyncJob job) : base() { start = DateTime.Now; Job = job; }

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			//FtpConnections.Pass(Client);
			Log.Download(Path, Size, DateTime.Now - start);
		}
	}

}
