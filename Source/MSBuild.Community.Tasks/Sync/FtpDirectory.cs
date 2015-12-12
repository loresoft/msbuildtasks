using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Starksoft.Net.Ftp;

namespace johnshope.Sync {

	public class FtpDirectory : FileOrDirectory, IDirectory {

		public SyncJob Job { get; set; }
		public Log Log { get { return Job.Log; } }

		Uri url;
		public Uri Url { get { return url; } set { url = value; } }

		public bool TransferProgress { get; set; }

		public FtpDirectory(FileOrDirectory parent, Uri url, SyncJob job) {
			Job = job;
			Parent = parent;
			if (url.Scheme != "ftp" && url.Scheme != "ftps") throw new NotSupportedException();
			Url = url;
			Name = url.File();
			Class = ObjectClass.Directory;
			Changed = DateTime.Now.AddDays(2);
			if (parent is FtpDirectory) TransferProgress = ((FtpDirectory)parent).TransferProgress;
		}

		public IDirectory Source { get; set; }
		public IDirectory Destination { get; set; }

		bool UseCompression { get { return Url.Query.Contains("compress"); } }

		public DirectoryListing List() {
			var ftp = Job.Connections.Open(ref url);
			try {
				ftp.FileTransferType = TransferType.Binary;
				var list = ftp.GetDirList()
					.Select(fi =>
						fi.ItemType == FtpItemType.Directory ?
							new FtpDirectory(this, Url.Relative(fi.Name), Job) :
							new FileOrDirectory { Name = fi.Name, Class = ObjectClass.File, Changed = fi.Modified, Size = fi.Size, Parent = this })
					.ToList();
				return new DirectoryListing(list);
			} catch (Exception ex) {
				Job.Failure(this, ex, ftp);
			} finally {
				Job.Connections.Pass(ftp);
			}
			return new DirectoryListing();
		}

		static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

		class ProgressData {
			public string Path;
			public long Size;
			public long Transferred;
			public TimeSpan ElapsedTime;
		}

		Dictionary<FtpClient, ProgressData> progress = new Dictionary<FtpClient, ProgressData>();
		public void ShowProgress(object sender, TransferProgressEventArgs a) {
			if (TransferProgress) {
				try {
					var ftp = (FtpClient)sender;
					var p = progress[ftp];
					p.Transferred += a.BytesTransferred;
					if (a.ElapsedTime - p.ElapsedTime > Interval) {
						Log.Progress(p.Path, p.Size, p.Transferred, a.ElapsedTime);
						//Log.Text(ftp.dw.TotalMilliseconds.ToString());
						//Log.Text(ftp.dr.TotalMilliseconds.ToString());
						p.ElapsedTime = a.ElapsedTime;
					}
				} catch { }
			}
		}

		public void WriteFile(System.IO.Stream file, FileOrDirectory src) {
			if (file == null) return;

			var ftp = Job.Connections.Open(ref url);
			try {
				if (ftp.FileTransferType != TransferType.Binary) ftp.FileTransferType = TransferType.Binary;
				var path = Url.Path() + "/" + src.Name;
				if (TransferProgress) {
					progress.Add(ftp, new ProgressData { ElapsedTime = new TimeSpan(0), Path = path, Size = src.Size });
					ftp.TransferProgress += ShowProgress;
				}
				var start = DateTime.Now;
				ftp.PutFile(file, src.Name, FileAction.Create);
				ftp.SetDateTime(src.Name, src.ChangedUtc);

				Log.Upload(path, src.Size, DateTime.Now - start);
			} catch (Exception e) {
				Job.Failure(src, e, ftp);
			} finally {
				if (TransferProgress) {
					ftp.TransferProgress -= ShowProgress;
					progress.Remove(ftp);
				}
				Job.Connections.Pass(ftp);
			}
		}

		public System.IO.Stream ReadFile(FileOrDirectory src) {
			var file = new FtpStream(Job);
			Job.Threads.DoAsync(() => {
				var ftp = Job.Connections.Open(ref url);
				try {
					if (ftp.FileTransferType != TransferType.Binary) ftp.FileTransferType = TransferType.Binary;
					file.Client = ftp;
					file.Path = Url.Path() + "/" + src.Name;
					file.Size = src.Size;
					if (TransferProgress) {
						progress.Add(ftp, new ProgressData { ElapsedTime = new TimeSpan(0), Path = file.Path, Size = src.Size });
						ftp.TransferProgress += ShowProgress;
					}
					file.Client.GetFile(src.Name, file, false);
				} catch (Exception ex) {
					Job.Failure(src, ex, ftp);
					file.Exception(ex);
				} finally {
					file.Close();
					if (TransferProgress) {
						file.Client.TransferProgress -= ShowProgress;
						progress.Remove(file.Client);
					}
					Job.Connections.Pass(file.Client);
				}
			});
			return file;
		}

		public void DeleteFile(FileOrDirectory dest) {
			var ftp = Job.Connections.Open(ref url);
			try {
				ftp.DeleteFile(dest.Name);
			} catch (Exception ex) {
				Job.Failure(dest, ex, ftp);
			} finally {
				Job.Connections.Pass(ftp);
			}
		}

		public void DeleteDirectory(FileOrDirectory dest) { DeleteDirectory(dest, null); }

		public void DeleteDirectory(FileOrDirectory dest, EventHandler onFinished = null) {
			var dir = (FtpDirectory)dest;
			int con = Job.Connections.Count(dir.url);
			if (con == 0) con = 1;
			var list = dir.List();
			var subdirs =  list.OfType<FtpDirectory>();

			var n = new SyncJob.Counter();
			n.N = subdirs.Count();

			var finished = (EventHandler)((sender, args) => {
				lock (n) {
					n.N--;
					if (n.N <= 0) {
						var ftp = Job.Connections.Open(ref dir.url);
						try {
							foreach (var file in list.Where(f => f.Class == ObjectClass.File)) ftp.DeleteFile(file.Name);
							ftp.ChangeDirectoryUp();
							ftp.DeleteDirectory(dest.Name);
						} catch (Exception ex) {
							Job.Failure(dest, ex);
						} finally {
							Job.Connections.Pass(ftp);
						}
						if (onFinished != null) onFinished(this, EventArgs.Empty);
					}
				}
			});
			foreach (var item in subdirs) {
				var d = item;
				var t = new Task(() => { d.DeleteDirectory(d, finished); });
				//t.Finished += finished;
				Job.Threads.Do(t);
			}
			if (subdirs.Count() == 0) finished(this, EventArgs.Empty);
		}

		public void Delete(FileOrDirectory dest) {
			if (dest.Class == ObjectClass.File) DeleteFile(dest);
			else DeleteDirectory(dest);
		}

		public IDirectory CreateDirectory(FileOrDirectory dest) {
			var ftp = Job.Connections.Open(ref url);
			try {
				var path = ftp.CorrectPath(Url.Path());
				if (dest != null) path = path + "/" + dest.Name;
				//var curpath = ftp.CurrentDirectory;
				//var ps = path.Split('/');
				//var cs = curpath.Split('/');
				//var j = cs.Length-1;	
				//var i = Math.Min(ps.Length, j+1);
				//while (j > i-1) { ftp.ChangeDirectoryUp(); j--; }
				//while (j > 0 && ps[j] != cs[j]) { ftp.ChangeDirectoryUp(); j--; i = j+1; }

				//while (i < ps.Length) { str.Append("/"); str.Append(ps[i++]); }

				//var dir = str.ToString();
				ftp.MakeDirectory(path);

				//if (url.Query()["old"] != null) ftp.ChangeDirectoryMultiPath(path);
				//else ftp.ChangeDirectory(path);

				if (dest != null) return new FtpDirectory(this, Url.Relative(dest.Name), Job);
				else return this;
			} catch (Exception ex) {
				Job.Failure(dest, ex, ftp);
			} finally {
				Job.Connections.Pass(ftp);
			}
			return null;
		}

	}
}
