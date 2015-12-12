using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using johnshope.Sync;


namespace MSPControlTasks.Sync {

	public class FileSync: Microsoft.Build.Utilities.Task {

		protected FileSync() { }

		protected FileSync(ResourceManager taskResources) : base(taskResources) { }

		protected FileSync(ResourceManager taskResources, string helpKeywordPrefix) : base(taskResources, helpKeywordPrefix) { }

		[Required]
		public ITaskItem[] Sources { get; set; }

		[Required]
		public ITaskItem[] Destinations { get; set; }

		public string Mode { get; set; }
		public bool Verbose { get; set; }
		public bool Quiet { get; set; }
		public string LogFile { get; set; }
		public string Exclude { get; set; }

		public override bool Execute() {
			if (Sources.Length != Destinations.Length) {
				Log.LogMessage("Sync: Number of sources must match destinations.");
				return false;
			}
			var job = new SyncJob();
			job.ExcludePatterns = Exclude;
			job.LogFile = LogFile;
			job.Mode = (CopyMode)Enum.Parse(typeof(CopyMode), Mode);
			job.Quiet = Quiet;
			job.Verbose = Verbose;


			for (int i = 0; i < Sources.Length; i++) {
				var src = Sources[i].ItemSpec;
				var dest = Destinations[i].ItemSpec;
				Log.LogMessage("Syncing {0} to {1}...", src, dest);
				job.Directory(new Uri(src), new Uri(dest));
			}
			return true;
		}
	}
}
