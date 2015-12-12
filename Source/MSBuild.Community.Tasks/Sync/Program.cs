using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace johnshope.Sync {

	public enum CopyMode { Update, Clone, Add }

	public class Program {

#if NET4
		public const string WelcomeMsg = "johnshope's ftp and folder sync v1.4 for .NET 4";
#else
		public const string WelcomeMsg = "johnshope's ftp and folder sync v1.4 for .NET 3.5";
#endif

		static void Main(string[] argsarr) {

			var args = new Arguments(argsarr);

			bool wait = false;

			var job = new SyncJob();

			try {


				if (args.Count < 2 || 7 < args.Count) {
					Console.Write(WelcomeMsg);
					Console.WriteLine("");
					Console.WriteLine("Usage: sync sourceurl desturl [/update | /clone | /add] [/x excludepatterns] [logfilename] [/v] [/q]");
					Console.WriteLine();
					Console.WriteLine("- Urls can be either ftp urls or local paths");
					Console.WriteLine("- Ftp urls are of the form protocol://username:password@server:port/path?ftp-options");
					Console.WriteLine("  Ftp urls must be url encoded. For example a space must be written as %20.");
					Console.WriteLine("- Protocol can be either ftp or ftps");
					Console.WriteLine("- Ftp-options are delimited by a & and are of the form parameter or parameter=value");
					Console.WriteLine("- Available ftp-options are:");
					Console.WriteLine("  - \"passive\" for passive ftp mode");
					Console.WriteLine("  - \"active\" for active ftp mode");
					Console.WriteLine("  - \"connections\" for the number of concurrent connections");
					Console.WriteLine("  - \"zip\" for compression if the server supports it");
					Console.WriteLine("  - \"raw\" for no compression");
					Console.WriteLine("  - \"old\" to use the most compatible ftp command set for old ftp servers");
					Console.WriteLine("  - \"time\" for the server's time offset to utc time. If you omit time, sync");
					Console.WriteLine("    will autodetect the time offset (The MDTM command must be supported by the server for this).");
					Console.WriteLine("  - \"crc\" for auto CRC32 data transfer checksum check.");
					Console.WriteLine("  - \"md5\" for auto MD5 data transfer checksum check.");
					Console.WriteLine("  - \"sha\" for auto SHA1 data transfer checksum check.");
					Console.WriteLine("- The connections option reqires an int value that limits the maximum concurent connections.");
					Console.WriteLine("- The default options are passive&connections=10");
					Console.WriteLine("- The /update option tells sync to keep newer files in the destination.");
					Console.WriteLine("- The /clone option tells sync to clone the source and discard all changes in the destination. This is the default.");
					Console.WriteLine("- The /add option tells sync to add all files that are not present or outdated in the destination, but not");
					Console.WriteLine("  to delete any files or overwrite newer files.");
					Console.WriteLine("- With the /x you can specify a comma or semicolon separated list of exclude file patterns.");
					Console.WriteLine("  Use / instead of \\ in patterns. Only one asterisk (*) is allowed per pattern, so instead of ");
					Console.WriteLine("  myfolder/*.* you would write myfolder/* .");
					Console.WriteLine("- You can redirect output to a logfile by specifying a logfile name.");
					Console.WriteLine("- You can set the output to verbose mode with the /v switch or to quiet mode with the /q switch.");
					return;
				}

				Uri src, dest;
				var srcarg = args.Pop();
				var destarg = args.Pop();
				if (srcarg.Contains(':')) src = new Uri(srcarg);
				else src = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, srcarg));
				if (destarg.Contains(':')) dest = new Uri(destarg);
				else dest = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, destarg));
				job.LogFile = null;
				job.Verbose = false;

				job.Mode = CopyMode.Clone;
				if (args.Has("/update")) job.Mode = CopyMode.Update;
				if (args.Has("/add")) job.Mode = CopyMode.Add;
				if (args.Has("/clone")) job.Mode = CopyMode.Clone;
				string ExcludePatterns = null;
				args.Has("/x", out ExcludePatterns);
				job.ExcludePatterns = ExcludePatterns ?? "";
				job.Verbose = args.Has("/v");
				job.Quiet = args.Has("/q");
				wait = args.Has("/w");
				//if (args.Has("/t")) SpeedTest.Test();
				job.LogFile = args.Pop();

				var now = DateTime.Now;
				job.Log.Text("######  " + now.ToShortDateString() + " " + now.ToShortTimeString() + "   " + WelcomeMsg);
				job.Log.Text("########################   Run sync without parameters for help.");
				job.Log.Text("");

				job.Directory(src, dest);
			} catch (Exception ex) {
				job.Log.Exception(ex);
			}

			if (wait) { Console.WriteLine("Press any key..."); Console.ReadKey(); }
		}
	}
}
