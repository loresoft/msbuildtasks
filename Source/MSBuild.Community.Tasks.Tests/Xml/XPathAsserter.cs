
using System;
using NUnit.Framework;
using System.Xml;


namespace MSBuild.Community.Tasks.Tests.Xml
{
    public class XPathAsserter : AbstractAsserter
    {
        string message;
        string actualValue;
        string expectedValue;

        public XPathAsserter(XmlDocument document, string xpath, string expectedValue, string message, params object[] args) : base(message, args)
        {
            XmlNode node = document.SelectSingleNode(xpath);
            if (node == null)
            {
                actualValue = null;
            }
            else
            {
                actualValue = node.Value;
            }
            this.expectedValue = expectedValue;
            this.message = message;
        }

        public override string Message
        {
            get
            {
                base.FailureMessage.DisplayExpectedValue(this.Expectation);
                base.FailureMessage.DisplayActualValue(this.actualValue);
                return base.FailureMessage.ToString();
            }
        }

        protected virtual string Expectation
        {
            get
            {
                return string.Format("<\"{0}\">", this.expectedValue);
            }
        }

        public override bool Test()
        {
            return (expectedValue == actualValue);           
        }

    }

}
