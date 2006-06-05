// $Id$

using System;
using System.Text;

namespace MSBuild.Community.Tasks.Math
{
    /// <summary>
    /// Performs the modulo operation on numbers.
    /// </summary>
    /// <remarks>
    /// The modulo operation finds the remainder of the division of one number by another.
    /// <para>When the second number (modulus) is a fractional value, the result can be a fractional value.</para>
    /// <para>
    /// Equivalent to the % operator in C# or the Mod operator in Visual Basic.
    /// </para>
    /// </remarks>
    /// <example>Numbers evenly divide:
    /// <code><![CDATA[
    /// <Math.Modulo Numbers="12;4">
    ///     <Output TaskParameter="Result" PropertyName="Result" />
    /// </Math.Modulo>
    /// <Message Text="12 modulo 4 = $(Result)"/>
    /// ]]></code>
    /// Above example will display:
    /// <code>12 modulo 4 = 0</code>
    /// </example>
    /// <example>Division on the numbers produces a remainder:
    /// <code><![CDATA[
    /// <Math.Modulo Numbers="14;4">
    ///     <Output TaskParameter="Result" PropertyName="Result" />
    /// </Math.Modulo>
    /// <Message Text="14 modulo 4 = $(Result)"/>
    /// ]]></code>
    /// Above example will display:
    /// <code>14 modulo 4 = 2</code>
    /// </example>
    /// <example>Modulus is a fractional value:
    /// <code><![CDATA[
    /// <Math.Modulo Numbers="12;3.5">
    ///     <Output TaskParameter="Result" PropertyName="Result" />
    /// </Math.Modulo>
    /// <Message Text="12 modulo 3.5 = $(Result)"/>
    /// ]]></code>
    /// Above example will display:
    /// <code>12 modulo 3.5 = 1.5</code>
    /// </example>
    public class Modulo : MathBase
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
            logger.Append("Modulo numbers: ");

            foreach (decimal number in numbers)
            {
                if (total.HasValue)
                {
                    logger.AppendFormat(" modulo {0}", number);
                    total %= number;
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
