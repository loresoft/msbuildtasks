// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Math
{
    /// <summary>
    /// Add numbers
    /// </summary>
    /// <example>Adding numbers:
    /// <code><![CDATA[
    /// <Math.Add Numbers="4;3">
    ///     <Output TaskParameter="Result" PropertyName="Result" />
    /// </Math.Add>
    /// <Message Text="Add 4+3= $(Result)"/>
    /// ]]></code>
    /// </example>
    public class Add : MathBase
    {

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            decimal[] numbers = StringArrayToDecimalArray(this.Numbers);
            decimal total = 0;
            
            StringBuilder logger = new StringBuilder();
            logger.Append("Add numbers: ");

            foreach (decimal number in numbers)
            {
                logger.AppendFormat("{0} + ", number);
                total += number;
            }

            logger.Replace('+', '=', logger.Length - 2, 1);
            logger.Append(total);
            base.Log.LogMessage(logger.ToString());

            this.Result = total.ToString(this.NumericFormat ?? string.Empty);
            return true;
        }
    }
}
