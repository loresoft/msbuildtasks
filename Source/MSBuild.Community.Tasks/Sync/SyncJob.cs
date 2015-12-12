using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace johnshope.Sync {

	public class SyncJob {

		static readonly TimeSpan dt = TimeSpan.FromMinutes(1); // minimal file time resolution

		public CopyMode Mode { get; set; }
		public string LogFile { get; set; }
		public bool Verbose { get; set; }
		public bool Quiet { get; set; }
		public Log Log = null;
		public string ExcludePatterns { get; set; }
		public Threads Threads = new Threads();
		public FtpConnections Connections;

		public class FailureInfo { public FileOrDirectory File; public Exception Exception; }

		public Queue<FailureInfo> Failures = new Queue<FailureInfo>();

		public void Failure(FileOrDirectory file, Exception ex) { lock (Failures) Failures.Enqueue(new FailureInfo { File = file, Exception = ex }); Log.Exception(ex); }
		public void Failure(FileOrDirectory file, Exception ex, FtpClient ftp) { lock (Failures) Failures.Enqueue(new FailureInfo { File = file, Exception = ex }); Log.Exception(ftp, ex); }

		public IDirectory Root(FileOrDirectory fd) { if (fd.Parent == null) return (IDirectory)fd; else return Root(fd.Parent); }

		public SyncJob() {
			Log = new Log { Job = this };
			Connections = new FtpConnections { Job = this };
			ExcludePatterns = "";
		}

		List<FailureInfo> MyFailures(IDirectory sroot, IDirectory droot) {
			List<FailureInfo> list = new List<FailureInfo>();

			lock (Failures) {
				int n = Failures.Count;
				while (n-- > 0) {
					var failure = Failures.Dequeue();
					var root = Root(failure.File);
					if (root == sroot || root == droot) list.Add(failure);
					else Failures.Enqueue(failure);
				}
			}
			return list;
		}

		public void RetryFailures(IDirectory sroot, IDirectory droot) {

			var list = MyFailures(sroot, droot);

			if (list.Count > 0) {
				Log.YellowText("####    Retry failed transfers...");
				var set = new HashSet<IDirectory>();
				foreach (var failure in list) {
					IDirectory dir;
					if (failure is IDirectory) dir = (IDirectory)failure.File;
					else dir = (IDirectory)failure.File.Parent;

					if (!set.Contains(dir.Source)) {
						set.Add(dir.Source);
						Directory(dir.Source, dir.Destination);
					}
				}

				Log.YellowText("####    Summary of errors:");
				foreach (var failure in list) Log.Exception(failure.Exception);

				Log.YellowText("####    Failed transfers:");
				foreach (var failure in list) Log.RedText("Failed transfer: " + failure.File.RelativePath);
			}

			list = MyFailures(sroot, droot); // dequeue recurrant failures.
		}

		public class Counter {
			public int N = 0;
		}

		public void Directory(IDirectory sdir, IDirectory ddir) {

			if (ddir == null || sdir == null) return;

			sdir.Source = ddir.Source = sdir;
			sdir.Destination = ddir.Destination = ddir;

			if (sdir is FtpDirectory) {
				((FtpDirectory)sdir).TransferProgress = true;
			}
			if (ddir is FtpDirectory) {
				((FtpDirectory)ddir).TransferProgress = true;
			}
			var slist = sdir.List().Where(file => !johnshope.Sync.Paths.Match(ExcludePatterns, file.RelativePath)).ToList();
			var dlist = ddir.List();
			//ddir.CreateDirectory(null);

			//Parallel.ForEach<FileOrDirectory>(list, new ParallelOptions { MaxDegreeOfParallelism = con },
			var tasks = new List<Task>();
			var n = new Counter();
			n.N = slist.Count;
			var finished = (EventHandler)((sender, args) => {
				lock(n) {
					n.N--;
					if (n.N <= 0) {
						if (Mode != CopyMode.Add) {
							foreach (var dest in dlist) 
								ddir.Delete(dest);
						}
					}
				}
			});
			foreach (var source in slist) {
				var src = source;
				var t = new Task(() => {
					FileOrDirectory dest = null;
					lock (dlist) { if (dlist.Contains(src.Name)) dest = dlist[src.Name]; }
					if (dest != null && dest.Class != src.Class && (src.Changed > dest.Changed || Mode == CopyMode.Clone))
						ddir.Delete(dest);
					if (src.Class == ObjectClass.File) {
						/*if (Verbose && dest != null) {
							johnshope.Sync.Log.CyanText(src.Name + ":    " + src.Changed.ToShortDateString() + "-" + src.Changed.ToShortTimeString() + " => " +
								dest.Changed.ToShortDateString() + "-" + dest.Changed.ToShortTimeString());
						}*/
						if (dest == null || ((Mode == CopyMode.Update || Mode == CopyMode.Add) && src.Changed > dest.Changed) || (Mode == CopyMode.Clone && (src.Changed > dest.Changed + dt))) {
							using (var s = sdir.ReadFile(src)) {
								ddir.WriteFile(s, src);
							}
						}
					} else {
						if (dest == null) Directory((IDirectory)src, ddir.CreateDirectory(src));
						else Directory((IDirectory)src, (IDirectory)dest);
					}
					lock (dlist) { dlist.Remove(src.Name); }
				});
				tasks.Add(t);
				t.Finished += finished;
				Threads.Do(t);
			}
			if (slist.Count == 0) finished(this, EventArgs.Empty);
		}

		public void Directory(Uri src, Uri dest) {
			try {
				var start = DateTime.Now;
				Connections.Allocate(src);
				Connections.Allocate(dest);

				int nsrc = 1, ndest = 1;
				// messages
				if (src.Scheme == "ftp" || src.Scheme == "ftps") {
					var ftp = Connections.Open(ref src);
					Log.Text("Source host: " + src.Authority + "    Server Time:" + ftp.ServerTimeString);
					Connections.Pass(ftp);
					nsrc = Connections.Count(src);
				}
				if (dest.Scheme == "ftp" || dest.Scheme == "ftps") {
					var ftp = Connections.Open(ref dest);
					Log.Text("Destination host: " + dest.Authority + "    Server Time:" + ftp.ServerTimeString);
					Connections.Pass(ftp);
					ndest = Connections.Count(dest);
				}
				Threads.N = Math.Max(nsrc, ndest);

				Log.Text(string.Format("Mode: {0}; Log: {1}; Verbose: {2}, Exclude: {3}", Mode, LogFile, Verbose, ExcludePatterns));
				Log.Text("");

				if (!string.IsNullOrEmpty(LogFile)) ExcludePatterns += ";" + LogFile;

				var sdir = johnshope.Sync.Directory.Parse(src, this);
				var ddir = johnshope.Sync.Directory.Parse(dest, this);

				Directory(sdir, ddir);
				Threads.Await();

				RetryFailures(sdir, ddir);
				Threads.Await();

				Connections.Close();

				Log.Summary(DateTime.Now - start);

				Log.Flush();
			} catch (Exception ex) {
				Log.Exception(ex);
			}
			Threads.Abort();

		}
	}
}
