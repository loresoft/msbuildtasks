// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Math
{
    /// <summary>
    /// Multiple numbers
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// <Math.Multiple Numbers="10;3">
    ///     <Output TaskParameter="Result" PropertyName="Result" />
    /// </Math.Multiple>
    /// <Message Text="Multiple 10*3= $(Result)"/>
    /// ]]></code>
    /// </example>
    public class Multiple : MathBase
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
            decimal? total = null;

            StringBuilder logger = new StringBuilder();
            logger.Append("Multiple numbers: ");

            foreach (decimal number in numbers)
            {
                if (total.HasValue)
                {
                    logger.AppendFormat(" * {0}", number);
                    total *= number;
                }
                else
                {
                    logger.Append(number);
                    total = number;
                }
            }

            decimal actualTotal = total ?? 0;

            logger.AppendFormat(" = {0}", actualTotal);
            base.Log.LogMessage(logger.ToString());

            this.Result = actualTotal.ToString(this.NumericFormat ?? string.Empty);
            return true;
        }
    }
}
