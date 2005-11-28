// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks.Math
{
    /// <summary>
    /// Divide numbers
    /// </summary>
    /// <example>
    /// <code><![CDATA[
    /// <Math.Divide Numbers="1;2">
    ///     <Output TaskParameter="Result" PropertyName="Result" />
    /// </Math.Divide>
    /// <Message Text="Divide 1/2= $(Result)"/>
    /// ]]>
    /// </code>
    /// </example>
    public class Divide : MathBase
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
            logger.Append("Divide numbers: ");

            foreach (decimal number in numbers)
            {
                if (total.HasValue)
                {
                    logger.AppendFormat(" / {0}", number);
                    total /= number;
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
