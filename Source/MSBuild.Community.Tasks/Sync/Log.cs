using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace johnshope.Sync {

	public class Log {

		public SyncJob Job = null;

		public int Uploads = 0;
		public int Downloads = 0;
		public long UploadSize = 0;
		public long DownloadSize = 0;
		public int Errors = 0;

		const int KB = 1024;
		const int MB = 1024 * KB;
		const int GB = 1024 * MB;
		const int MaxLogSize = 10*MB;

		bool checkDir = true;

		StringBuilder buffer = new StringBuilder();

		public virtual string Size(long size) {
			if (size > GB) return string.Format("{0:F2} GB", size / (1.0 * GB));
			if (size > 100 * KB) return string.Format("{0:F2} MB", size / (1.0 * MB));
			return string.Format("{0:F2} KB", size / (1.0 * KB));
		}

		public virtual void Debug(string text) {
			if (Job.Verbose) Text(text);
		}

		public virtual void Flush() {
			if (Job.LogFile != null) {
				try {
					if (checkDir) {
						checkDir = false;
						var dir = Path.GetDirectoryName(Job.LogFile);
						if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
					}
					System.IO.File.AppendAllText(Job.LogFile, buffer.ToString(), UTF8Encoding.UTF8);
#if NET4
					buffer.Clear();
#else
					buffer = new StringBuilder();
#endif
				} catch (Exception ex) {
					Console.WriteLine("Error writing to the logfile " + Job.LogFile);
					Console.WriteLine(ex.Message);
				}
			}
		}

		public virtual void LogText(string text, bool newline) {
			lock (this) {
				if (Job.LogFile != null) {
					if (newline) buffer.AppendLine(text);
					else buffer.Append(text);
				}
			}
		}
		
		public virtual void Text(string text) { lock (this) { Console.WriteLine(text); LogText(text, true); } }
		public virtual void RedText(string text) { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Red; Text(text); Console.ForegroundColor = oldc; } }
		public virtual void CyanText(string text) { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Cyan; Text(text); Console.ForegroundColor = oldc; } }
		public virtual void GreenText(string text) { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Green; Text(text); Console.ForegroundColor = oldc; } }
		public virtual void YellowText(string text) { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Yellow; Text(text); Console.ForegroundColor = oldc; } }
		public virtual void YellowLabel(string text) { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Yellow; Console.Write(text); LogText(text, false); Console.ForegroundColor = oldc; } }
		public virtual void RedLabel(string text) { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Red; Console.Write(text); LogText(text, false); Console.ForegroundColor = oldc; } }
		public virtual void Label(string text) { lock (this) { Console.Write(text); LogText(text, false); } }
		public virtual void Dot() { lock (this) { var oldc = Console.ForegroundColor; Console.ForegroundColor = ConsoleColor.Green; Console.Write("."); LogText(".", false); Console.ForegroundColor = oldc; } }

		public virtual void Exception(Exception e) { 
			if (e is System.Threading.ThreadAbortException) throw e;
			lock (this) {
				Errors++;
				RedText("Error"); RedText(e.Message);
				if (Job.Verbose) { RedText(e.StackTrace); }
				System.Diagnostics.Debugger.Break();
			}
		}
		public virtual void Exception(FtpClient ftp, Exception e) {
			if (e is System.Threading.ThreadAbortException) throw e;
			if (ftp == null) Exception(e);
			else {
				lock (this) {
					var prefix = Job.Connections.FTPTag(ftp.Index) + "! ";
					Errors++; RedLabel(prefix);  RedText("Error");
					var lines = e.Message.Split('\n');
					foreach (var line in lines) { RedLabel(prefix); RedText(line); }
					if (Job.Verbose) {
						lines = e.StackTrace.Split('\n');
						foreach (var line in lines) { RedLabel(prefix); RedText(line); }
					}
					System.Diagnostics.Debugger.Break();
				}
			}
		}
		public virtual void Upload(string path, long size, TimeSpan time) {
			if (Job.Quiet) Dot();
			else GreenText(string.Format("Uploaded {0}    =>    {1} at {2:F3}/s.", path, Size(size), Size((long)(size / time.TotalSeconds + 0.5))));
			Uploads++; UploadSize += size;
		}
		public virtual void Download(string path, long size, TimeSpan time) {
			if (Job.Quiet) Dot();
			else GreenText(string.Format("Downloaded {0}    =>    {1} at {2:F3}/s.", path, Size(size), Size((long)(size / time.TotalSeconds + 0.5))));
			Downloads++; DownloadSize += size;
		}
		public virtual void Progress(string path, long size, long part, TimeSpan time) {
			if (Job.Quiet) Dot();
			else GreenText(string.Format("Transfer of {0}    =>    {1:F1}% at {2:F3}/s.", path, (part * 100.0 / size), Size((long)(part / time.TotalSeconds + 0.5))));
		}


		public virtual void Summary(TimeSpan t) {
			Text("");
			GreenText(string.Format("####    =>    {0} Files and {1} transfered in {2:F3} seconds at {3}/s. {4} Errors.",
				Math.Max(Uploads, Downloads), Size(UploadSize + DownloadSize), t.TotalSeconds, Size((long)(Math.Max(UploadSize, DownloadSize) / t.TotalSeconds + 0.5)), Errors));
			Text("");
			Text("");
			Text("");

			if (Job.LogFile != null) {
				var log = new FileInfo(Job.LogFile);
				if (log.Length > MaxLogSize) {
					var loglines = File.ReadAllLines(Job.LogFile).ToList();
					loglines.RemoveRange(0, loglines.Count / 2);
					File.WriteAllLines(Job.LogFile, loglines.ToArray());
				}
			}
		}

	}
}
