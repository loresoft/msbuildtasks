using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Starksoft.Net.Ftp;

namespace johnshope.Sync {

	public class FtpConnections {

		public SyncJob Job { get; set; }
		public Log Log { get { return Job.Log; } }

		Dictionary<string, ResourceQueue<FtpClient>> Queue = new Dictionary<string, ResourceQueue<FtpClient>>();
		Dictionary<string, int?> TimeOffsets = new Dictionary<string, int?>();
		Dictionary<string, string> Features = new Dictionary<string, string>();

		string Key(FtpClient ftp) { return ftp.Host + ":" + ftp.Port.ToString(); }
		string Key(Uri uri) { return uri.Host + ":" + uri.Port.ToString(); }

		int? Connections(Uri url) {
			var query = url.Query();
			int con;
			if (int.TryParse((query["connections"] ?? "").ToString(), out con)) return con;
			else return null;
		}

		string Proxy(Uri url) {
			var query = url.Query();
			string proxy = (query["proxy"] ?? "").ToString();
			return proxy;
		}

		int? TimeOffset(Uri url) {
			var query = url.Query();
			int zone = 0;
			string zonestr = (string)query["time"];
			if (string.IsNullOrEmpty(zonestr)) return null;
			zonestr = zonestr.ToLower();
			if (zonestr == "z" || zonestr == "utc") return 0;
			if (!int.TryParse(zonestr, out zone)) return null;
			return zone;
		}

		int clientIndex = 0;

		public string FTPTag(int n) { return "FTP" + n.ToString(); }

		public FtpClient Open(ref Uri url) {
			var queue = Queue[Key(url)];
			var path = url.Path();
			var ftp = queue.DequeueOrBlock(client => client.CurrentDirectory == client.CorrectPath(path));
			try {
				if (ftp == null) {
					ftp = new johnshope.Sync.FtpClient(url.Host, url.Port, url.Scheme == "ftps" ? FtpSecurityProtocol.Tls1Explicit : FtpSecurityProtocol.None, ++clientIndex);
					ftp.Job = Job;
					//ftp.IsLoggingOn = Sync.Verbose;
					if (Job.Verbose) {
						ftp.ClientRequest += new EventHandler<FtpRequestEventArgs>((sender, args) => {
							lock (Log) { Log.YellowLabel(FTPTag(ftp.Index) + "> "); Log.Text(args.Request.Text); }
						});
						ftp.ServerResponse += new EventHandler<FtpResponseEventArgs>((sender, args) => {
							lock (Log) { Log.Label(FTPTag(ftp.Index) + ": "); Log.Text(args.Response.RawText); }
						});
					}
					if (url.Query()["passive"] != null || url.Query()["active"] == null) ftp.DataTransferMode = TransferMode.Passive;
					else ftp.DataTransferMode = TransferMode.Passive;
					ftp.AutoChecksumValidation = HashingFunction.None;
					if (url.Query()["md5"] != null) ftp.AutoChecksumValidation = HashingFunction.Md5;
					else if (url.Query()["sha"] != null) ftp.AutoChecksumValidation = HashingFunction.Sha1;
					else if (url.Query()["crc"] != null) ftp.AutoChecksumValidation = HashingFunction.Crc32;
				} else {
					if (!ftp.IsConnected) ftp.Reopen();
				}
				ftp.Clients++;
				if (ftp.Clients != 1) throw new Exception("FTP connection is opened by multiple clients.");
				if (!ftp.IsConnected) {
					if (!string.IsNullOrEmpty(url.UserInfo)) {
						if (url.UserInfo.Contains(':')) {
							var user = url.UserInfo.Split(':');
							ftp.Open(user[0], user[1]);
						} else {
							ftp.Open(url.UserInfo, string.Empty);
						}
					} else {
						ftp.Open("Anonymous", "anonymous");
					}
					// set encoding
					string features;
					if (!Features.TryGetValue(Key(url), out features)) {
						features = ftp.GetFeatures();
						Features.Add(Key(url), features);
					}
					// set encoding
					if (url.Query()["old"] == null) {
						if (features.Contains("UTF8")) {
							ftp.CharacterEncoding = System.Text.Encoding.UTF8;
							ftp.Quote("OPTS UTF8 ON");
						} else if (features.Contains("UTF7")) {
							ftp.CharacterEncoding = System.Text.Encoding.UTF7;
							ftp.Quote("OPTS UTF7 ON");
						} else {
							ftp.CharacterEncoding = System.Text.Encoding.ASCII;
						}
					} else {
						ftp.CharacterEncoding = System.Text.Encoding.ASCII;
					}
				}
				// get server local time offset
				var offset = TimeOffset(url);
				if (offset.HasValue) ftp.TimeOffset = offset;
				else if (!ftp.TimeOffset.HasValue) {
					lock (queue) {
						var offsetclient = queue.FirstOrDefault(client => client != null && client.TimeOffset.HasValue);
						if (offsetclient != null) ftp.TimeOffset = offsetclient.TimeOffset;
						ftp.TimeOffset = ftp.ServerTimeOffset;
					}
				}
				// change path
				path = ftp.CorrectPath(url.Path());
				if (url.Query()["raw"] != null && ftp.IsCompressionEnabled) ftp.CompressionOff();
				if (url.Query()["zip"] != null && ftp.IsCompressionEnabled) ftp.CompressionOn();
				if (ftp.CurrentDirectory != path) {
					try {
						if (url.Query()["old"] != null) ftp.ChangeDirectoryMultiPath(path);
						else ftp.ChangeDirectory(path);
					} catch (Exception ex) {
						ftp.MakeDirectory(path);
						if (url.Query()["old"] != null) ftp.ChangeDirectoryMultiPath(path);
						else ftp.ChangeDirectory(path);
					}
					if (ftp.CurrentDirectory != ftp.CorrectPath(url.Path()))
						throw new Exception(string.Format("Cannot change to correct path {0}.", url.Path()));
				}
			} catch (FtpDataConnectionException ex) {
				if (url.Query()["passive"] == null) {
					url = new Uri(url.ToString() + (url.Query().Count > 0 ? "&" : "%3F") + "passive", true);
					ftp.Close();
					ftp.DataTransferMode = TransferMode.Passive;
					ftp.Clients++;
					Pass(ftp);
					return Open(ref url);
				} else {
					Log.Exception(ex);
				}
			} catch (Exception e) {
				Log.Exception(e);
			}
			if (ftp.Clients != 1 || ftp.CurrentDirectory != ftp.CorrectPath(url.Path())) 
				throw new Exception("FTP connection open postcondition failed.");
			return ftp;
		}

		public void Pass(FtpClient client) {
			if (client == null || client.Clients != 1) throw new Exception("FTP connection pass precondition failed.");
			client.Clients--;
			Queue[Key(client)].Enqueue(client);
		}

		public int Count(Uri url) { if (url.IsFile) return 1; else return Queue[Key(url)].Count; }

		public int Allocate(Uri url) { if (!url.IsFile) { Queue[Key(url)] = new ResourceQueue<FtpClient>(); var n = Connections(url) ?? 10; var i = n; while (i-- > 0) Queue[Key(url)].Enqueue(null); return n; } return 1; }

		public void Close() {
			foreach (var queue in Queue.Values) {
				while (queue.Count > 0) {
					var ftp = queue.Dequeue();
					if (ftp != null) {
						if (ftp.IsConnected) ftp.Close();
						ftp.Dispose();
					}
				}
			}
		}
	}
}
