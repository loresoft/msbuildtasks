// $Id$
// Copyright © 2006 Ignaz Kohlbecker

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks
{
	/// <summary>
	/// A task to merge and transform
	/// a set of xml files.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The xml files of parameter <see cref="Inputs"/>
	/// are merged into one xml document,
	/// wrapped with a root tag <see cref="RootTag"/>
	/// </p>
	/// <p>
	/// If only one input file is provided,
	/// merging and wrapping can be omitted
	/// by setting <see cref="RootTag"/> to an empty string.
	/// </p>
	/// <p>
	/// The root tag can be given any number of attributes
	/// by providing a list of semicolon-delimited name/value pairs
	/// to parameter <see cref="RootAttributes"/>.
	/// For example: <code>RootAttributes="foo=bar;date=$(buildDate)"</code>
	/// </p>
	/// <p>
	/// Parameter <see cref="RootAttributes"/> defaults to
	/// one attribute with a name specified by <see cref="CREATED_ATTRIBUTE"/>,
	/// and a local time stamp as value.
	/// To suppress the default value, an empty parameter
	/// <code>RootAttributes=""</code>
	/// must be specified explicitely.
	/// </p>
	/// <p>
	/// The xsl transformation file 
	/// specified by parameter <see cref="Xsl"/>
	/// is applied on the input.
	/// </p>
	/// <p>
	/// The <see cref="ITaskItem"/> <see cref="Xsl"/>
	/// can be given any number of metadata,
	/// which will be handed to the xsl transformation
	/// as parameters.
	/// </p>
	/// <p>
	/// The output is written to the file
	/// specified by parameter <see cref="Output"/>.
	/// </p>
	/// </remarks>
	/// <example>
	/// This example for generating a report
	/// from a set of NUnit xml results:
	/// <code><![CDATA[
	/// <ItemGroup>
	///     <nunitReportXslFile Include="$(MSBuildCommunityTasksPath)\$(nunitReportXsl)">
	///         <project>$(project)</project>
	///         <configuration>$(configuration)</configuration>
	///         <msbuildFilename>$(MSBuildProjectFullPath)</msbuildFilename>
	///         <msbuildBinpath>$(MSBuildBinPath)</msbuildBinpath>
	///         <xslFile>$(MSBuildCommunityTasksPath)\$(nunitReportXsl)</xslFile>
	///     </nunitReportXslFile>
	/// </ItemGroup>
	///
	/// <Target Name="test-report" >
	///     <Xslt Inputs="@(nunitFiles)"
	///         RootTag="mergedroot"
	///         Xsl="@(nunitReportXslFile)" 
	///         Output="$(testDir)\TestReport.html" />
	/// </Target>]]></code>
	/// 
	/// This examples shows all available task attributes:
	/// <code><![CDATA[
	/// <Time Format="yyyyMMddHHmmss">
	///     <Output TaskParameter="LocalTimestamp" PropertyName="buildDate" />
	/// </Time>
	/// 
	/// <Xslt
	///      Inputs="@(xmlfiles)"
	///      RootTag="mergedroot"
	///      RootAttributes="foo=bar;date=$(buildDate)"
	///      Xsl="transformation.xsl"
	///      Output="report.html"
	/// />]]></code>
	/// </example>
	public class Xslt : Task
	{
		#region Constants

		/// <summary>
		/// The name of the default attribute
		/// of the <see cref="RootTag"/>.
		/// The value is <c>"created"</c>,
		/// and the attribute will contain a local time stamp.
		/// </summary>
		public const string CREATED_ATTRIBUTE = @"created";

		#endregion Constants

		#region Fields
		private ITaskItem[] inputs;
		private string rootTag;
		private string rootAttributes;
		private ITaskItem xsl;
		private string output;
		#endregion Fields

		#region Input Parameters

		/// <summary>
		/// Gets or sets the xml input files.
		/// </summary>
		[Required]
		public ITaskItem[] Inputs
		{
			get { return inputs; }
			set { inputs = value; }
		}

		/// <summary>
		/// Gets or sets the xml tag name
		/// of the root tag wrapped
		/// around the merged xml input files.
		/// </summary>
		public string RootTag
		{
			get { return rootTag; }
			set { rootTag = value; }
		}

		/// <summary>
		/// Gets or sets the list of
		/// semicolon-delimited name/value pairs
		/// of the <see cref="RootTag"/>.
		/// For example: <code>RootAttributes="foo=bar;date=$(buildDate)"</code>
		/// </summary>
		public string RootAttributes
		{
			get { return rootAttributes; }
			set { rootAttributes = value; }
		}

		/// <summary>
		/// Gets or sets the path of the
		/// xsl transformation file to apply.
		/// </summary>
		/// <remarks>
		/// The property can be given any number of metadata,
		/// which will be handed to the xsl transformation
		/// as parameters.
		/// </remarks>
		[Required]
		public ITaskItem Xsl
		{
			get { return xsl; }
			set { xsl = value; }
		}

		/// <summary>
		/// Gets or sets the path of the output file.
		/// </summary>
		[Required]
		public string Output
		{
			get { return output; }
			set { output = value; }
		}

		#endregion Input Parameters

		#region Task overrides
		/// <summary>
		/// When overridden in a derived class, executes the task.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if the task successfully executed; otherwise, <c>false</c>.
		/// </returns>
		public override bool Execute()
		{
			#region Sanity checks
			if ((Inputs == null) || (Inputs.Length == 0))
			{
				Log.LogError(Properties.Resources.XsltNoInputFiles);
				return false;
			}
			#endregion Sanity checks

			#region Create and fill xml working document
			// The working document
			XmlDocument doc = new XmlDocument();

			try
			{
				if ((inputs.Length == 1) && string.IsNullOrEmpty(rootTag))
				{
					Log.LogMessage(MessageImportance.Normal, Properties.Resources.XsltNoRootTag);
					doc.Load(inputs[0].ItemSpec);

				}
				else
				{
					createRootNode(doc);

					#region Populate root node
					foreach (ITaskItem input in inputs)
					{
						// create and load a xml input file
						XmlDocument inputDocument = new XmlDocument();
						inputDocument.Load(input.ItemSpec);

						// import the root node of the xml input file
						// into the working file
						XmlNode importNode = doc.ImportNode(inputDocument.DocumentElement, true);
						doc.DocumentElement.AppendChild(importNode);
					}

					#endregion Populate root node
				}

			}
			catch (XmlException ex)
			{
				Log.LogErrorFromException(ex);
				return false;
			}
			catch (ArgumentException ex)
			{
				Log.LogErrorFromException(ex);
				return false;
			}
			catch (InvalidOperationException ex)
			{
				Log.LogErrorFromException(ex);
				return false;
			}

			#endregion Create and fill xml working document

			#region Create and execute the transform
			XmlWriter xmlWriter = null;

			XslCompiledTransform transform = new XslCompiledTransform();

			#region Parameters from Metadata

			XsltArgumentList argumentList = new XsltArgumentList();
			foreach (string metadataName in xsl.MetadataNames)
			{
				string metatdataValue = xsl.GetMetadata(metadataName);
				argumentList.AddParam(metadataName, string.Empty, metatdataValue);

				Log.LogMessage(MessageImportance.Low, Properties.Resources.XsltAddingParameter,
					metadataName, metatdataValue);
			}

			#endregion Parameters from Metadata

			try
			{
				transform.Load(xsl.ItemSpec);
				xmlWriter = XmlWriter.Create(this.output, transform.OutputSettings);

				transform.Transform(doc.DocumentElement, argumentList, xmlWriter);

			}
			catch (XsltException ex)
			{
				Log.LogErrorFromException(ex);
				return false;

			}
			catch (FileNotFoundException ex)
			{
				Log.LogErrorFromException(ex);
				return false;

			}
			catch (DirectoryNotFoundException ex)
			{
				Log.LogErrorFromException(ex);
				return false;

			}
			catch (XmlException ex)
			{
				Log.LogErrorFromException(ex);
				return false;

			}
			finally
			{
				if (xmlWriter != null) xmlWriter.Close();
			}

			#endregion Create and execute the transform

			return true;
		}

		#endregion Task overrides

		#region Private Methods

		private void createRootNode(XmlDocument doc)
		{
			// create the root element
			Log.LogMessage(MessageImportance.Normal, Properties.Resources.XsltCreatingRootTag, rootTag);
			XmlElement rootElement = doc.CreateElement(rootTag);

			if (rootAttributes == null)
			{
				// add the timestamp attribute to the root element
				string timestamp = DateTime.Now.ToString();
				Log.LogMessage(MessageImportance.Normal, Properties.Resources.XsltAddingRootAttribute,
					CREATED_ATTRIBUTE, timestamp);
				rootElement.SetAttribute(CREATED_ATTRIBUTE, timestamp);

			}
			else
			{
				foreach (string rootAttribute in rootAttributes.Split(';'))
				{
					string[] keyValuePair = rootAttribute.Split('=');

					Log.LogMessage(MessageImportance.Normal, Properties.Resources.XsltAddingRootAttribute,
						keyValuePair[0], keyValuePair[1]);

					rootElement.SetAttribute(keyValuePair[0], keyValuePair[1]);
				}
			}

			// insert the root element to the document
			doc.AppendChild(rootElement);
		}

		#endregion Private Methods

	}
}