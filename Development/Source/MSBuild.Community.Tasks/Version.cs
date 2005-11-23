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
        /// <summary>
        /// Defines possible algorithms to generate the build number.
        /// </summary>
        public enum BuildNumberAlgorithm
        {
            /// <summary>
            /// Use the number of months since start of project * 100 + current 
            /// day in month as build number.
            /// </summary>
            MonthDay,

            /// <summary>
            /// Increment an existing build number.
            /// </summary>
            Increment,

            /// <summary>
            /// Use an existing build number (and do not increment it).
            /// </summary>
            NoIncrement
        }

        /// <summary>
        /// Defines possible algorithms to generate the revision number.
        /// </summary>
        public enum RevisionNumberAlgorithm
        {
            /// <summary>
            /// Use the number of seconds since the start of today / 10.
            /// </summary>
            Automatic,

            /// <summary>
            /// Increment an existing revision number.
            /// </summary>
            Increment
        }

        private int _Major;

        [Output]
        public int Major
        {
            get { return _Major; }
            set { _Major = value; }
        }

        private int _Minor;

        [Output]
        public int Minor
        {
            get { return _Minor; }
            set { _Minor = value; }
        }

        private int _Build;

        [Output]
        public int Build
        {
            get { return _Build; }
            set { _Build = value; }
        }

        private int _Revision;

        [Output]
        public int Revision
        {
            get { return _Revision; }
            set { _Revision = value; }
        }

        private string _File;

        [Required]
        public string File
        {
            get { return _File; }
            set { _File = value; }
        }

        private BuildNumberAlgorithm _BuildType = BuildNumberAlgorithm.MonthDay;

        public BuildNumberAlgorithm BuildType
        {
            get { return _BuildType; }
            set { _BuildType = value; }
        }

        private RevisionNumberAlgorithm _RevisionType = RevisionNumberAlgorithm.Automatic;

        public RevisionNumberAlgorithm RevisionType
        {
            get { return _RevisionType; }
            set { _RevisionType = value; }
        }

        private DateTime _StartDate;

        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        
                
        public override bool Execute()
        {
            System.Version version = ReadVersionFromFile();
            if (version == null)
                return false;

            int newBuildNumber = CalculateBuildNumber(version.Build);
            int newRevisionNumber = CalculateRevisionNumber(version, newBuildNumber);

            version = new System.Version(version.Major, version.Minor, newBuildNumber, newRevisionNumber);
            _Major = version.Major;
            _Minor = version.Minor;
            _Build = version.Build;
            _Revision = version.Revision;

            return WriteVersionToFile(version);

        }

        private System.Version ReadVersionFromFile()
        {
            string version = null;

            // read the version string
            try
            {
                using (StreamReader reader = new StreamReader(_File))
                {
                    version = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Unable to read version number from \"{0}\". {1}", 
                    _File, ex.Message);
            }

            // instantiate a Version instance from the version string
            try
            {
                return new System.Version(version);
            }
            catch (Exception ex)
            {
                Log.LogError("Invalid version string \"{0}\" in file \"{1}\". {2}",
                    version, _File, ex.Message);
                
            }

            return null;
        }

        private bool WriteVersionToFile(System.Version version)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_File))
                {
                    writer.Write(version.ToString());
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Unable to write version number to \"{0}\". {1}",
                    _File, ex.Message);
                return false;
            }

            return true;
        }

        private int CalculateMonthDayBuildNumber()
        {
            // we need to have a start date defined!
            if (StartDate == DateTime.MinValue)
            {
                StartDate = DateTime.Now;
            }

            DateTime today = DateTime.Now;
            if (StartDate > today)
            {
                StartDate = today;
            }

            // Calculate difference in years
            int years = today.Year - StartDate.Year;

            // Calculate difference in months
            int months;
            if (today.Month < StartDate.Month)
            {
                --years;  // borrow from years
                months = (today.Month + 12) - StartDate.Month;
            }
            else
            {
                months = today.Month - StartDate.Month;
            }

            months += years * 12;

            // The days is simply today's day
            int days = today.Day;

            return months * 100 + days;
        }

        private int CalculateSecondsSinceMidnight()
        {
            DateTime today = DateTime.Now;
            return (today.Hour * 3600 + today.Minute * 60 + today.Second) / 10;
        }

        private int CalculateBuildNumber(int currentBuildNumber)
        {
            switch (BuildType)
            {
                case BuildNumberAlgorithm.MonthDay:
                    return CalculateMonthDayBuildNumber();
                case BuildNumberAlgorithm.Increment:
                    return currentBuildNumber + 1;
                case BuildNumberAlgorithm.NoIncrement:
                    return currentBuildNumber;
                default:
                    return currentBuildNumber;
            }
        }

        private int CalculateRevisionNumber(System.Version version, int newBuildNumber)
        {
            int newRevsionNumber;

            // modify revision number according to revision type setting
            switch (RevisionType)
            {
                case RevisionNumberAlgorithm.Automatic:
                    newRevsionNumber = CalculateSecondsSinceMidnight();
                    break;
                case RevisionNumberAlgorithm.Increment:
                    if (newBuildNumber != version.Build)
                    {
                        // reset revision number to zero if the build number has changed
                        newRevsionNumber = 0;
                    }
                    else
                    {
                        // increment the revision number if this is a revision of the same build
                        newRevsionNumber = version.Revision + 1;
                    }
                    break;
                default:
                    newRevsionNumber = CalculateSecondsSinceMidnight();
                    break;

            }

            return newRevsionNumber;
        }

    }
}
