//Copyright © 2006, Jonathan de Halleux
//http://blog.dotnetwiki.org/default,month,2005-07.aspx

using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
// $Id$

namespace MSBuild.Community.Tasks.Schema
{
    /// <summary>
    /// Different ways to specify the assembly in a UsingTask element.
    /// </summary>
    public enum TaskListAssemblyFormatType
    {
        /// <summary>
        /// Assembly file name (Default): &lt;UsingTask AssemblyFile=&quot;foo.dll&quot; /&gt;
        /// </summary>
        AssemblyFileName,
        /// <summary>
        /// Assembly location: &lt;UsingTask AssemblyName=&quot;foo&quot; /&gt;
        /// </summary>
        AssemblyFileFullPath,
        /// <summary>
        /// Assembly Name: &lt;UsingTask AssemblyFile=&quot;bin\debug\foo.dll&quot; /&gt;
        /// </summary>
        AssemblyName,
        /// <summary>
        /// Assembly fully qualified name: &lt;UsingTask AssemblyName=&quot;foo.dll,version ....&quot; /&gt;
        /// </summary>
        AssemblyFullName
    }

    /// <summary>
    /// A Task that generates a XSD schema of the tasks in an assembly.
    /// </summary>
    /// <example>
    /// <para>Creates schema for MSBuild Community Task project</para>
    /// <code><![CDATA[
    /// <TaskSchema Assemblies="Build\MSBuild.Community.Tasks.dll" 
    ///     OutputPath="Build" 
    ///     CreateTaskList="true" 
    ///     IgnoreMsBuildSchema="true"
    ///     Includes="Microsoft.Build.Commontypes.xsd"/>
    /// ]]></code>
    /// </example>
    public class TaskSchema : MarshalByRefObject, ITask
    {
        #region ITask Members

        private IBuildEngine _BuildEngine;

        /// <summary>
        /// Gets or sets the build engine associated with the task.
        /// </summary>
        /// <value></value>
        /// <returns>The build engine associated with the task.</returns>
        public IBuildEngine BuildEngine
        {
            get { return _BuildEngine; }
            set { _BuildEngine = value; }
        }

        private ITaskHost _HostObject;

        /// <summary>
        /// Gets or sets any host object that is associated with the task.
        /// </summary>
        /// <value></value>
        /// <returns>The host object associated with the task.</returns>
        public ITaskHost HostObject
        {
            get { return _HostObject; }
            set { _HostObject = value; }
        }

        #endregion

        #region Property - Log

        private TaskLoggingHelper log;

        /// <summary>
        /// Gets or sets the logger for the task.
        /// </summary>
        /// <value>The logger.</value>
        public TaskLoggingHelper Log
        {
            get
            {
                if (log == null)
                {
                    log = new TaskLoggingHelper(this);
                }
                return log;
            }
            set { log = value; }
        }

        #endregion

        private ITaskItem[] assemblies;
        private ITaskItem[] schemas;
        private ITaskItem[] taskLists;
        private string outputPath;
        private bool createTaskList = true;
        private bool ignoreDocumentation = false;
        private string taskListAssemblyFormat = TaskListAssemblyFormatType.AssemblyFileName.ToString();
        private bool ignoreMsBuildSchema = false;
        private ITaskItem[] includes;

        /// <summary>
        /// Gets or sets the list of <see cref="Assembly"/> path to analyse.
        /// </summary>
        [Required]
        public ITaskItem[] Assemblies
        {
            get { return this.assemblies; }
            set { this.assemblies = value; }
        }

        /// <summary>
        /// Gets or sets the output path for the generated files.
        /// </summary>
        public string OutputPath
        {
            get { return this.outputPath; }
            set { this.outputPath = value; }
        }

        /// <summary>
        /// Gets the list of path to the generated XSD schema.
        /// </summary>
        [Output]
        public ITaskItem[] Schemas
        {
            get { return this.schemas; }
            private set { this.schemas = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the task list (using UsingTask)
        /// has to be genereted.
        /// </summary>
        public bool CreateTaskList
        {
            get { return this.createTaskList; }
            set { this.createTaskList = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating how the assembly is specified in the
        /// UsingTask element.
        /// </summary>
        /// <enum cref="TaskListAssemblyFormatType" />
        public string TaskListAssemblyFormat
        {
            get { return this.taskListAssemblyFormat; }
            set { this.taskListAssemblyFormat = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating wheter documentation should be ignored
        /// even if available (Default is false).
        /// </summary>
        public bool IgnoreDocumentation
        {
            get { return this.ignoreDocumentation; }
            set { this.ignoreDocumentation = value; }
        }

        /// <summary>
        /// Gets the path to the task list if it was generated.
        /// </summary>
        [Output]
        public ITaskItem[] TaskLists
        {
            get { return this.taskLists; }
            private set { this.taskLists = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the 
        /// MsBuild schema inclusing should be ignored
        /// </summary>
        public bool IgnoreMsBuildSchema
        {
            get { return this.ignoreMsBuildSchema; }
            set { this.ignoreMsBuildSchema = value; }
        }

        /// <summary>
        /// Gets or sets a list of included schemas
        /// </summary>
        public ITaskItem[] Includes
        {
            get { return this.includes; }
            set { this.includes = value; }
        }

        private string GetTaskListName(Assembly taskAssembly)
        {
            string schemaFileName =
                String.Format(taskAssembly.GetName().Name + ".Targets");
            if (string.IsNullOrEmpty(this.OutputPath))
            {
                schemaFileName =
                    Path.Combine(
                        Path.GetDirectoryName(taskAssembly.Location),
                        schemaFileName);
            }
            else
            {
                schemaFileName = Path.Combine(this.OutputPath, schemaFileName);
            }
            return schemaFileName;
        }

        private string GetSchemaFileName(Assembly taskAssembly)
        {
            string schemaFileName =
                String.Format(taskAssembly.GetName().Name + ".xsd");
            if (string.IsNullOrEmpty(this.OutputPath))
            {
                schemaFileName =
                    Path.Combine(
                        Path.GetDirectoryName(taskAssembly.Location),
                        schemaFileName);
            }
            else
                schemaFileName = Path.Combine(this.OutputPath, schemaFileName);
            return schemaFileName;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public bool Execute()
        {
            if (!String.IsNullOrEmpty(this.OutputPath) && !Directory.Exists(this.OutputPath))
            {
                this.Log.LogMessage("Create output path {0}", this.OutputPath);
                Directory.CreateDirectory(this.OutputPath);
            }

            this.schemas = new ITaskItem[this.Assemblies.Length];
            if (this.CreateTaskList)
                this.taskLists = new ITaskItem[this.Assemblies.Length];

            for (int i = 0; i < this.Assemblies.Length; ++i)
            {
                ITaskItem taskAssemblyFile = this.Assemblies[i];
                if (!AnalyseAssembly(taskAssemblyFile, i))
                    return false;
            }

            return true;
        }


        private bool AnalyseAssembly(ITaskItem taskAssemblyFile, int i)
        {
            string assemblyFullName = taskAssemblyFile.GetMetadata("FullPath");
            this.Log.LogMessage("Analysing {0}", assemblyFullName);
            bool result = false;

            // Need to set the ApplicationBase of the new AppDomain to be the MSBuildCommunityTasks directory
            // so that when a TaskSchemaSandboxLoader is instantiated in the sandbox domain, its dependencies
            // (that is, MSBuild.Community.Tasks.dll) are found.
            AppDomainSetup sandboxOptions = new AppDomainSetup();
            sandboxOptions.ApplicationBase = GetCommunityTasksInstallationPath();

            AppDomain sandboxDomain = AppDomain.CreateDomain("TaskSchemaSandbox", null, sandboxOptions);
            try
            {
                // Instantiate a new TaskSchema object in the new AppDomain
                TaskSchema sandboxTask = (TaskSchema) sandboxDomain.CreateInstanceAndUnwrap(
                                                          typeof (TaskSchema).Assembly.GetName().FullName,
                                                          typeof (TaskSchema).FullName);
                // Since only the default constructor was called on the object, we need to copy
                // all of these properties over to the remote object
                this.PrepareClone(sandboxTask);
                // And now we initiate the processing in the other AppDomain
                result = sandboxTask.AnalyzeAssemblyInSeparateAppDomain(
                    this.Log,
                    taskAssemblyFile,
                    i);
            }
            finally
            {
                AppDomain.Unload(sandboxDomain);
            }

            return result;
        }

        private string GetCommunityTasksInstallationPath()
        {
            return Path.GetDirectoryName(GetType().Assembly.Location);
        }

        private bool AnalyzeAssemblyInSeparateAppDomain(TaskLoggingHelper logger, ITaskItem assemblyItem, int i)
        {
            Assembly taskAssembly = ReflectionHelper.LoadAssembly(logger, assemblyItem);
            if (taskAssembly == null)
                return false;

            logger.LogMessage(MessageImportance.High, "Loaded DLL: {0}", taskAssembly.Location);

            try
            {
                TaskSchemaAnalyser analyzer = new TaskSchemaAnalyser(this, taskAssembly);
                analyzer.CreateSchema();
                this.schemas[i] = analyzer.WriteSchema(GetSchemaFileName(taskAssembly));

                if (this.CreateTaskList)
                {
                    analyzer.CreateUsingDocument();
                    this.taskLists[i] = analyzer.WriteUsingDocument(GetTaskListName(taskAssembly));
                }
            }
            catch (Exception ex)
            {
                logger.LogErrorFromException(ex);
                return false;
            }

            return true;
        }

        private void PrepareClone(TaskSchema remote)
        {
            remote.Assemblies = this.Assemblies;
            remote.BuildEngine = this.BuildEngine;
            remote.CreateTaskList = this.CreateTaskList;
            remote.HostObject = this.HostObject;
            remote.IgnoreDocumentation = this.IgnoreDocumentation;
            remote.IgnoreMsBuildSchema = this.IgnoreMsBuildSchema;
            remote.Includes = this.Includes;
            remote.Log = this.Log;
            remote.OutputPath = this.OutputPath;
            remote.Schemas = this.Schemas;
            remote.TaskListAssemblyFormat = this.TaskListAssemblyFormat;
            remote.TaskLists = this.TaskLists;
        }
    }
}