using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace johnshope.Sync {

	class LocalDirectory: FileOrDirectory, IDirectory {

		public SyncJob Job { get; set; }
		public Log Log { get { return Job.Log; } }

		public Uri Url { get; set; }

		public string Path { get { return HttpUtility.UrlDecode(Url.LocalPath); } }

		public LocalDirectory(FileOrDirectory parent, Uri url, SyncJob job) {
			Job = job;
            Parent = parent;
            if (!url.ToString().Contains(':')) {
                if (url.ToString() == ".") url = new Uri(Environment.CurrentDirectory);
                else url = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, url.ToString()));
            }
			if (!url.IsFile) throw new NotSupportedException("url is no local file.");
			Url = url;
			var info = new DirectoryInfo(Path);
			Name = info.Name;
			Class = ObjectClass.Directory;
			ChangedUtc = info.LastWriteTimeUtc;
		}

		public IDirectory Source { get; set; }
		public IDirectory Destination { get; set; }

		public DirectoryListing List() {
			try {
				var info = new DirectoryInfo(Path);
				if (info.Exists) {
					var finfos = info.GetFileSystemInfos();
					var infos = info.GetFileSystemInfos()
						.Select(fi => fi is FileInfo ? new FileOrDirectory { Name = fi.Name, Class = ObjectClass.File, ChangedUtc = fi.LastWriteTimeUtc, Size = ((FileInfo)fi).Length, Parent = this } : new LocalDirectory(this, new Uri(fi.FullName), Job));
                    return new DirectoryListing(infos);
				} else {
					return new DirectoryListing();
				}
			} catch (Exception ex) {
				Job.Failure(this, ex);
			}
			return new DirectoryListing();
		}

		public void WriteFile(Stream sstream, FileOrDirectory src) {
			if (sstream == null) return;
			try {
				var path = System.IO.Path.Combine(Path, src.Name);
				using (var dstream = File.Create(path)) {
					if (sstream is PipeStream) {
						((PipeStream)sstream).Read(dstream);
					} else {
						Streams.Copy(sstream, dstream);
					}
				}
				File.SetLastAccessTimeUtc(path, src.ChangedUtc);
			} catch (Exception ex) {
				Job.Failure(src, ex);
			}
		}

		public Stream ReadFile(FileOrDirectory src) {
			try {
				var path = System.IO.Path.Combine(Path, src.Name);
				return File.OpenRead(path);
			} catch (Exception ex) {
				Job.Failure(src, ex);
			}
			return null;
		}

		public void DeleteFile(FileOrDirectory dest) {
			try {
				var path = System.IO.Path.Combine(Path, dest.Name);
				if (new FileInfo(path).FullName != new FileInfo(Job.LogFile).FullName) System.IO.File.Delete(path);
			} catch (Exception ex) {
				Job.Failure(dest, ex);
			}
		}

		public void DeleteDirectory(FileOrDirectory dest) {
			try {
				System.IO.Directory.Delete(((LocalDirectory)dest).Path, true);
			} catch (Exception ex) {
				Job.Failure(dest, ex);
			}
		}

		public void Delete(FileOrDirectory dest) {
			try {
				var path = System.IO.Path.Combine(Path, dest.Name);
				if (dest.Class == ObjectClass.File) {
					if (new FileInfo(path).FullName != new FileInfo(Job.LogFile).FullName) System.IO.File.Delete(path);
				}  else System.IO.Directory.Delete(path, true);
			} catch (Exception ex) {
				Job.Failure(dest, ex);
			}
		}

		public IDirectory CreateDirectory(FileOrDirectory src) {
			try {
				string path;
				if (src == null) path = Path;
				else path = System.IO.Path.Combine(Path, src.Name);
				System.IO.Directory.CreateDirectory(path);
				return new LocalDirectory(this, new Uri(path), Job);
			} catch (Exception ex) {
				Job.Failure(src, ex);
			}
			return null;
		}
	}
}
