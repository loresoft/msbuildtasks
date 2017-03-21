using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace johnshope.Sync {
	public class FtpClient: Starksoft.Net.Ftp.FtpClient {

		public FtpClient(string host, int port, Starksoft.Net.Ftp.FtpSecurityProtocol protocol, int Index) : base(host, port, protocol) { this.Index = Index; Clients = 0; }

		public int Index { get; set; }

		public int Clients { get; set; }
	}
}
