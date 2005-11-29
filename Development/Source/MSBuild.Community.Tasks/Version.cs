// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;

namespace MSBuild.Community.Tasks
{
    public class Version : Task
    {
        public Version()
        {
            _major = 1;
            _minor = 0;
            _build = 0;
            _revision = 0;
        }
        
#region Properties
        private int _major;

        [Output]
        public int Major
        {
            get { return _major; }
            set { _major = value; }
        }

        private int _minor;

        [Output]
        public int Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        private int _build;

        [Output]
        public int Build
        {
            get { return _build; }
            set { _build = value; }
        }

        private int _revision;

        [Output]
        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        private string _file;

        [Required]
        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        private string _buildType;

        public string BuildType
        {
            get { return _buildType; }
            set { _buildType = value; }
        }

        private string _revisionType;

        public string RevisionType
        {
            get { return _revisionType; }
            set { _revisionType = value; }
        } 
#endregion
        
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

            if (!System.IO.File.Exists(_file))
            {
                Log.LogWarning("Version file \"{0}\" not found .", _file);
                return;
            }

            // read the version string
            try
            {
                using (StreamReader reader = new StreamReader(_file))
                {
                    textVersion = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Unable to read version number from \"{0}\". {1}", 
                    _file, ex.Message);
                return;
            }

            Log.LogMessage("Version \"{0}\" read from file \"{1}\".", 
                textVersion, _file);

            try
            {
                version = new System.Version(textVersion);
            }
            catch (Exception ex)
            {
                Log.LogError("Invalid version string \"{0}\" in file \"{1}\". {2}",
                    version, _file, ex.Message);
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
                using (StreamWriter writer = System.IO.File.CreateText(_file))
                {
                    writer.Write(version.ToString());
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Unable to write version number to \"{0}\". {1}",
                    _file, ex.Message);
                return false;
            }

            Log.LogMessage("Version \"{0}\" wrote to file \"{1}\".", 
                version.ToString(), _file);

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
