// $Id$

using System;
using System.Collections;
using System.Collections.Generic;
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
            Console.WriteLine("Custom: {0}", e.Message);            
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            _errorCount++;
            Console.WriteLine("Error: {0}", e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            _messageCount++;
            Console.WriteLine("Message: {0}", e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            _warningCount++;
            Console.WriteLine("Warning: {0}", e.Message);
        }

        public string ProjectFileOfTaskNode
        {
            get { return string.Empty; }
        }        
#endregion
    }
}
