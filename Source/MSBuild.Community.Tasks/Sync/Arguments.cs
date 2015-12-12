using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace johnshope.Sync {
    
    public class Arguments: List<string> {

        public Arguments(string[] args) : base(args.ToList()) { }

        public bool Has(string option) {
            int i = IndexOf(option);
            if (i >= 0) {
                RemoveAt(i);
                return true;
            }
            return false;
        }

        public bool Has(string option, out string parameter) {
            int i = IndexOf(option);
            if (i >= 0 && i < Count-1) {
                parameter = this[i+1];
                RemoveRange(i, 2);
                return true;
            }
            parameter = null;
            return false;
        }

        public string Pop() { if (Count > 0) { var p = this[0]; RemoveAt(0); return p; } return null; }
 
    }
}
