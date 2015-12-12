using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSPControlTasks.Sync {

	public class TaskLog: johnshope.Sync.Log {
		public override void Text(string text) {
			base.Text(text);
			Log.LogMessage(text);
		}
	}
}
