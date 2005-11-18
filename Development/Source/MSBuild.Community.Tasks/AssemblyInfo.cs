// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;


namespace MSBuild.Community.Tasks
{
    public class AssemblyInfo : Task
    {
        private Dictionary<string, string> _attributes;

        public AssemblyInfo()
        {
            _attributes = new Dictionary<string, string>(); 
            _Imports = new string[] { "System", "System.Reflection", "System.Runtime.CompilerServices", "System.Runtime.InteropServices" };
            _OutputFile = "AssemblyInfo.cs";
        }

        private string[] _Imports;

        private string _CodeLanguage;

        [Required]
        public string CodeLanguage
        {
            get { return _CodeLanguage; }
            set { _CodeLanguage = value; }
        }

        private string _OutputFile;

        [Output]
        public string OutputFile
        {
            get { return _OutputFile; }
            set { _OutputFile = value; }
        }

        public bool ComVisible
        {
            get { return ReadBooleanAttribute("ComVisible"); }
            set { _attributes["ComVisible"] = value.ToString(); }
        }

        public bool CLSCompliant
        {
            get { return ReadBooleanAttribute("CLSCompliant"); }
            set { _attributes["CLSCompliant"] = value.ToString(); }
        }

        public string Guid
        {
            get { return ReadAttribute("Guid"); }
            set { _attributes["Guid"] = value; }
        }

        public string AssemblyTitle
        {
            get { return ReadAttribute("AssemblyTitle"); }
            set { _attributes["AssemblyTitle"] = value; }
        }

        public string AssemblyDescription
        {
            get { return ReadAttribute("AssemblyDescription"); }
            set { _attributes["AssemblyDescription"] = value; }
        }

        public string AssemblyConfiguration
        {
            get { return ReadAttribute("AssemblyConfiguration"); }
            set { _attributes["AssemblyConfiguration"] = value; }
        }

        public string AssemblyCompany
        {
            get { return ReadAttribute("AssemblyCompany"); }
            set { _attributes["AssemblyCompany"] = value; }
        }

        public string AssemblyProduct
        {
            get { return ReadAttribute("AssemblyProduct"); }
            set { _attributes["AssemblyProduct"] = value; }
        }

        public string AssemblyCopyright
        {
            get { return ReadAttribute("AssemblyCopyright"); }
            set { _attributes["AssemblyCopyright"] = value; }
        }

        public string AssemblyTrademark
        {
            get { return ReadAttribute("AssemblyTrademark"); }
            set { _attributes["AssemblyTrademark"] = value; }
        }

        public string AssemblyCulture
        {
            get { return ReadAttribute("AssemblyCulture"); }
            set { _attributes["AssemblyCulture"] = value; }
        }

        public string AssemblyVersion
        {
            get { return ReadAttribute("AssemblyVersion"); }
            set { _attributes["AssemblyVersion"] = value; }
        }

        public string AssemblyFileVersion
        {
            get { return ReadAttribute("AssemblyFileVersion"); }
            set { _attributes["AssemblyFileVersion"] = value; }
        }

        public override bool Execute()
        {
            if (_attributes.Count == 0)
            {
                Log.LogError("No assembly parameter were set for file \"{0}\".", _OutputFile);
                return false;
            }

            using (StreamWriter writer = File.CreateText(_OutputFile))
            {
                GenerateFile(writer);
                writer.Flush(); 
                writer.Close();
                Log.LogMessage("Created AssemblyInfo file \"{0}\".", _OutputFile);
            }
            return true;
        }

        private void GenerateFile(TextWriter writer)
        {
            CodeDomProvider provider = null;

            if (string.Compare(_CodeLanguage, "VB", true) == 0)
            {
                provider = new Microsoft.VisualBasic.VBCodeProvider();
                _OutputFile= Path.ChangeExtension(_OutputFile, ".vb");
            }
            else
            {
                provider = new Microsoft.CSharp.CSharpCodeProvider();
                _OutputFile = Path.ChangeExtension(_OutputFile, ".cs");
            }

            if (provider == null)
                throw new NotSupportedException("The specified code language is not supported.");

            ICodeGenerator _generator = provider.CreateGenerator();

            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace();

            foreach (string import in _Imports)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(import));
            }

            codeCompileUnit.Namespaces.Add(codeNamespace);

            foreach (KeyValuePair<string, string> assemblyAttribute in _attributes)
            {
                // create new assembly-level attribute
                CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(assemblyAttribute.Key);

                bool typedValue;

                if ((assemblyAttribute.Key == "CLSCompliant" || assemblyAttribute.Key == "ComVisible")
                    && bool.TryParse(assemblyAttribute.Value, out typedValue))
                {
                    codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(typedValue)));
                }
                else
                {
                    codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyAttribute.Value)));
                }

                // add assembly-level argument to code compile unit
                codeCompileUnit.AssemblyCustomAttributes.Add(codeAttributeDeclaration);
            }
            _generator.GenerateCodeFromCompileUnit(codeCompileUnit, writer, new CodeGeneratorOptions());
        }

        private string ReadAttribute(string key)
        {
            string value;
            _attributes.TryGetValue(key, out value);
            return value;
        }

        private bool ReadBooleanAttribute(string key)
        {
            string value;
            bool result;

            if (!_attributes.TryGetValue(key, out value))
                return false;
            if (!bool.TryParse(value, out result))
                return false;

            return result;
        }

    }
}
