

using System;
using System.Collections;
using System.Text;
using Microsoft.Build.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Build Engine Used for Testing Tasks
    /// </summary>
    public class MockBuild : IBuildEngine
    {
        public MockBuild()
        {
            _errorCount = 0;
            _warningCount = 0;
            _messageCount = 0;
            _customCount = 0;
        }

#region Properties
        private StringBuilder _errorLog = new StringBuilder();
        
        public string ErrorLog
        {
            get { return _errorLog.ToString(); }
        }

        private StringBuilder _warningLog = new StringBuilder();
        
        public string WarningLog
        {
            get { return _warningLog.ToString(); }
        }

        private StringBuilder _messageLog = new StringBuilder();
        
        public string MessageLog
        {
            get { return _messageLog.ToString(); }
        }

        private StringBuilder _customLog = new StringBuilder();
        
        public string CustomLog
        {
            get { return _customLog.ToString(); }
        }

        private int _errorCount;

        public int ErrorCount
        {
            get { return _errorCount; }
        }

        private int _warningCount;

        public int WarningCount
        {
            get { return _warningCount; }
        }

        private int _messageCount;

        public int MessageCount
        {
            get { return _messageCount; }
        }

        private int _customCount;

        public int CustomCount
        {
            get { return _customCount; }
        }

        public int LogCount
        {
            get { return _errorCount + _warningCount + _messageCount + _customCount; }
        } 
#endregion

#region IBuildEngine Members
        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return false;
        }

        public int ColumnNumberOfTaskNode
        {
            get { return 0; }
        }

        public bool ContinueOnError
        {
            get { return false; }
        }

        public int LineNumberOfTaskNode
        {
            get { return 0; }
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            _customCount++;
            _customLog.AppendLine(e.Message);
            Console.WriteLine("Custom: {0}", e.Message);            
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            _errorCount++;
            _errorLog.AppendLine(e.Message);
            Console.WriteLine("Error: {0}", e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            _messageCount++;
            _messageLog.AppendLine(e.Message);
            Console.WriteLine("Message: {0}", e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            _warningCount++;
            _warningLog.AppendLine(e.Message);
            Console.WriteLine("Warning: {0}", e.Message);
        }

        public string ProjectFileOfTaskNode
        {
            get { return string.Empty; }
        }        
#endregion
    }
}
