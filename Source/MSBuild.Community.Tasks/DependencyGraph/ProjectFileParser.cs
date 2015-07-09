using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MSBuild.Community.Tasks.DependencyGraph
{
    /// <summary>
    /// Very simple parser that gets reference and assembly name information from project files
    /// </summary>
    public class ProjectFileParser
    {
        private XmlDocument xml;

        /// <summary>
        /// Creates new parser, based on project file specified by stream
        /// </summary>
        /// <param name="stream">A stream pointing to the project file content</param>
        public ProjectFileParser(Stream stream)
        {
            ParseXml(stream);
        }

        private void ParseXml(Stream stream)
        {
            this.xml = new XmlDocument();
            this.xml.Load(stream);            
        }

        /// <summary>
        /// Returns the Assembly Name for the project file
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyName()
        {
            return xml.DocumentElement.ChildNodes
                .Cast<XmlNode>()
                .Where(n => n.Name == "PropertyGroup")
                .SelectMany(n => n.ChildNodes.Cast<XmlNode>())
                .Where(n => n.Name == "AssemblyName")
                .FirstOrDefault().InnerText;            
        }

        public IEnumerable<ProjectReference> GetReferences()
        {
            List<ProjectReference> result = new List<ProjectReference>();

            xml.DocumentElement.ChildNodes
                .Cast<XmlNode>()
                .Where(n => n.Name == "ItemGroup")
                .SelectMany(n => n.ChildNodes.Cast<XmlNode>())
                .Where(n => n.Name == "Reference")
                .ToList()
                .ForEach(n => {
                    result.Add(NodeToReference(n));
                });

            return result;              
        }

        private ProjectReference NodeToReference(XmlNode node)
        {
            string hintPath = String.Empty;
            if (node.ChildNodes.Count > 0) { 
                var hintPathNode = node.ChildNodes
                    .Cast<XmlNode>()
                    .Where(n => n.Name == "HintPath")
                    .FirstOrDefault();
                if (hintPathNode != null)
                    hintPath = hintPathNode.InnerText;
            }                    

            return new ProjectReference(
                node.Attributes["Include"].InnerText,
                hintPath
                );
        }


        /// <summary>
        /// Given an assembly name in the form "Ionic.Zip.Reduced, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c, processorArchitecture=MSIL"
        /// returns only the Ionic.Zip.Reduced part.
        /// </summary>
        /// <param name="fullAssemblyName"></param>
        /// <returns></returns>
        public string GetAssemblyNameFromFullName(string fullAssemblyName)
        {
            return fullAssemblyName.Split(',')[0].Trim();
        }
    }
}
