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
    /// Reads a value from the Registry
    /// </summary>
    /// <example>Read .NET Framework install root from Registry.
    /// <code><![CDATA[
    /// <RegistryRead 
    ///     KeyName="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework" 
    ///     ValueName="InstallRoot">
    ///     <Output TaskParameter="Value" PropertyName="InstallRoot" />
    /// </RegistryRead>
    /// <Message Text="InstallRoot: $(InstallRoot)"/>
    /// ]]></code>
    /// </example>
    public class RegistryRead : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RegistryRead"/> class.
        /// </summary>
        public RegistryRead()
        {
            _defaultValue = string.Empty;
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

        private string _defaultValue;

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }


        private string _value;

        /// <summary>
        /// Gets the stored value.
        /// </summary>
        /// <value>The value.</value>
        [Output]
        public string Value
        {
            get { return _value; }
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
            _value = Registry.GetValue(_keyName, _valueName, _defaultValue).ToString();
            _value = _value ?? string.Empty;

            Log.LogMessage(Properties.Resources.RegistryRead);
            Log.LogMessage("[{0}]", _keyName);
            if(string.IsNullOrEmpty(_valueName))
                Log.LogMessage("@=\"{0}\"", _value);
            else
                Log.LogMessage("\"{0}\"=\"{1}\"", _valueName, _value);

            return true;
        }
    }
}
