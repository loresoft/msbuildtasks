// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Win32;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Writes a value to the Registry
    /// </summary>
    /// <example>Write a value to Registry
    /// <code><![CDATA[
    /// <RegistryWrite 
    ///     KeyName="HKEY_CURRENT_USER\SOFTWARE\MSBuildTasks"
    ///     ValueName="RegistryWrite"
    ///     Value="Test Write" />
    /// ]]></code>
    /// </example>
    public class RegistryWrite : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RegistryWrite"/> class.
        /// </summary>
        public RegistryWrite()
        {

        }

        #region Properties

        private string _keyName;

        /// <summary>
        /// Gets or sets the full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
        /// </summary>
        /// <value>The name of the key.</value>
        [Required]
        public string KeyName
        {
            get { return _keyName; }
            set { _keyName = value; }
        }

        private string _valueName;

        /// <summary>
        /// Gets or sets the name of the name/value pair.
        /// </summary>
        /// <value>The name of the value.</value>
        public string ValueName
        {
            get { return _valueName; }
            set { _valueName = value; }
        }

        private string _value;

        /// <summary>
        /// Gets or sets the value to be stored.
        /// </summary>
        /// <value>The value.</value>
        [Required]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            Registry.SetValue(_keyName, _valueName, _value);

            Log.LogMessage(Properties.Resources.RegistryWrite);
            Log.LogMessage("[{0}]", _keyName);
            if (string.IsNullOrEmpty(_valueName))
                Log.LogMessage("@=\"{0}\"", _value);
            else
                Log.LogMessage("\"{0}\"=\"{1}\"", _valueName, _value);

            return true;
        }
    }
}

