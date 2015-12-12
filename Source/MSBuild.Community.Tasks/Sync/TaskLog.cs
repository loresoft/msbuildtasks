using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;

namespace johnshope.Sync {

	public class TaskLog: johnshope.Sync.Log {

		public TaskLog(Microsoft.Build.Utilities.Task task) { Task = task; }

		public Microsoft.Build.Utilities.Task Task { get; set; }

		public override void Text(string text) {
			base.Text(text);
			Task.Log.LogMessage(text);
		}
	}
}
