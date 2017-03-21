using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

namespace johnshope.Sync {

	public enum ObjectClass { File, Directory }

	public class FileOrDirectory {
		public SyncJob Job { get; set; }
		public FileOrDirectory Parent { get; set; }
		public string RelativePath { get { if (Parent == null) return Name; else return Parent.RelativePath + "/" + Name; } }
		public string Name;
		public ObjectClass Class;
		public DateTime Changed;
		public DateTime ChangedUtc { get { return Changed.ToUniversalTime(); } set { Changed = value.ToLocalTime(); } }
		public long Size;
	}

	public class DirectoryListing : KeyedCollection<string, FileOrDirectory> {
		public DirectoryListing() : base() { }
		public DirectoryListing(IEnumerable<FileOrDirectory> list): this() {
			foreach (var e in list) {
				try { Add(e); } catch { }
			}
		}

		protected override string GetKeyForItem(FileOrDirectory item) {
			return item.Name;
		}
	}

	public interface IDirectory {
		SyncJob Job { get; set; }
		Uri Url { get; set; }
		DirectoryListing List();
		void WriteFile(Stream file, FileOrDirectory src);
		Stream ReadFile(FileOrDirectory src);
		void Delete(FileOrDirectory dest);
		void DeleteFile(FileOrDirectory dest);
		void DeleteDirectory(FileOrDirectory dest);
		IDirectory CreateDirectory(FileOrDirectory src);
		IDirectory Source { get; set; }
		IDirectory Destination { get; set; }
	}
}
