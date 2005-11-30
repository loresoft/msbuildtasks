// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Get Version information from file.
    /// </summary>
    /// <example>Get version information and increment revision.
    /// <code><![CDATA[
    /// <Version VersionFile="number.txt" RevisionType="Increment">
    ///     <Output TaskParameter="Major" PropertyName="Major" />
    ///     <Output TaskParameter="Minor" PropertyName="Minor" />
    ///     <Output TaskParameter="Build" PropertyName="Build" />
    ///     <Output TaskParameter="Revision" PropertyName="Revision" />
    /// </Version>
    /// <Message Text="Version: $(Major).$(Minor).$(Build).$(Revision)"/>
    /// ]]></code>
    /// </example>
    public class Version : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Version"/> class.
        /// </summary>
        public Version()
        {
            _major = 1;
            _minor = 0;
            _build = 0;
            _revision = 0;
        }
        
#region Properties
        private int _major;

        /// <summary>
        /// Gets or sets the major version number.
        /// </summary>
        /// <value>The major version number.</value>
        [Output]
        public int Major
        {
            get { return _major; }
            set { _major = value; }
        }

        private int _minor;

        /// <summary>
        /// Gets or sets the minor version number.
        /// </summary>
        /// <value>The minor version number.</value>
        [Output]
        public int Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        private int _build;

        /// <summary>
        /// Gets or sets the build version number.
        /// </summary>
        /// <value>The build version number.</value>
        [Output]
        public int Build
        {
            get { return _build; }
            set { _build = value; }
        }

        private int _revision;

        /// <summary>
        /// Gets or sets the revision version number.
        /// </summary>
        /// <value>The revision version number.</value>
        [Output]
        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        private string _versionFile;

        /// <summary>
        /// Gets or sets the version file.
        /// </summary>
        /// <value>The version file.</value>
        [Required]
        public string VersionFile
        {
            get { return _versionFile; }
            set { _versionFile = value; }
        }

        private string _buildType;

        /// <summary>
        /// Gets or sets the type of the build.
        /// </summary>
        /// <value>The type of the build.</value>
        /// <remarks>
        /// Possible values include Automatic, Increment, NonIncrement.
        /// </remarks>
        public string BuildType
        {
            get { return _buildType; }
            set { _buildType = value; }
        }

        private string _revisionType;

        /// <summary>
        /// Gets or sets the type of the revision.
        /// </summary>
        /// <value>The type of the revision.</value>
        /// <remarks>
        /// Possible values include Automatic, Increment, NonIncrement.
        /// </remarks>
        public string RevisionType
        {
            get { return _revisionType; }
            set { _revisionType = value; }
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
            ReadVersionFromFile();
            CalculateBuildNumber();
            CalculateRevisionNumber();
            return WriteVersionToFile();
        }

        private void ReadVersionFromFile()
        {
            string textVersion = null;
            System.Version version = null;

            if (!System.IO.File.Exists(_versionFile))
            {
                Log.LogWarning("Version file \"{0}\" not found .", _versionFile);
                return;
            }

            try
            {
                using (StreamReader reader = new StreamReader(_versionFile))
                {
                    textVersion = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Unable to read version number from \"{0}\". {1}", 
                    _versionFile, ex.Message);
                return;
            }

            Log.LogMessage("Version \"{0}\" read from file \"{1}\".", 
                textVersion, _versionFile);

            try
            {
                version = new System.Version(textVersion);
            }
            catch (Exception ex)
            {
                Log.LogError("Invalid version string \"{0}\" in file \"{1}\". {2}",
                    version, _versionFile, ex.Message);
                return;                
            }

            if (version != null)
            {
                _major = version.Major;
                _minor = version.Minor;
                _build = version.Build;
                _revision = version.Revision;
            }
        }

        private bool WriteVersionToFile()
        {
            System.Version version = new System.Version(_major, _minor, _build, _revision);

            try
            {
                using (StreamWriter writer = System.IO.File.CreateText(_versionFile))
                {
                    writer.Write(version.ToString());
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Unable to write version number to \"{0}\". {1}",
                    _versionFile, ex.Message);
                return false;
            }

            Log.LogMessage("Version \"{0}\" wrote to file \"{1}\".", 
                version.ToString(), _versionFile);

            return true;
        }

        private int CalculateDaysSinceMilenium()
        {
            DateTime today = DateTime.Now;
            DateTime startDate = new DateTime(2000, 1, 1);
            TimeSpan span = today.Subtract(startDate);
            return (int)span.TotalDays;
        }

        private void CalculateBuildNumber()
        {
            if (string.Compare(_buildType, "Automatic", true) == 0)
            {
                _build = CalculateDaysSinceMilenium();
            }
            else if (string.Compare(_buildType, "Increment", true) == 0)
            {
                _build++;
            }
        }

        private void CalculateRevisionNumber()
        {
            if (string.Compare(_revisionType, "Automatic", true) == 0)
            {
                _revision = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            }
            else if (string.Compare(_revisionType, "Increment", true) == 0)
            {
                _revision++;
            }
        }

    }
}
