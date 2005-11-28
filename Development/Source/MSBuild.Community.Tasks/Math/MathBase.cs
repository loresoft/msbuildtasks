// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace MSBuild.Community.Tasks.Math
{
    /// <summary>
    /// Math task base class
    /// </summary>
    public abstract class MathBase : Task
    {
        private string[] _numbers;

        /// <summary>
        /// Gets or sets the numbers to work with.
        /// </summary>
        /// <value>The numbers.</value>
        [Required]
        public string[] Numbers
        {
            get { return _numbers; }
            set { _numbers = value; }
        }

        private string _result;

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        [Output]
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private string _numericFormat;

        /// <summary>
        /// Gets or sets the numeric format.
        /// </summary>
        /// <value>The numeric format.</value>
        public string NumericFormat
        {
            get { return _numericFormat; }
            set { _numericFormat = value; }
        }
        

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected decimal[] StringArrayToDecimalArray(string[] numbers)
        {
            decimal[] result = new decimal[numbers.Length];

            for(int x = 0; x < numbers.Length; x++)
            {
                decimal converted;
                bool isParsed = decimal.TryParse(numbers[x], out converted);
                if (!isParsed)
                {
                    Log.LogError("\"{0}\" is not a number.", numbers[x]);
                    result[x] = 0;
                }
                else
                {
                    result[x] = converted;
                }
            }
            return result;
        }
    }
}
