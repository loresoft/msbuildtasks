using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// A task for git to get the current commit hash.
    /// </summary>
    public class GitCommits : GitClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitCommits"/> class.
        /// </summary>
        public GitCommits()
        {
            Command = "rev-list";
            Revision = "HEAD";                    
        }

        /// <summary>
        /// Gets or sets the revision to get the version from. Default is HEAD.
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Gets or sets the commits.
        /// </summary>
        [Output]
        public string Commits { get; set; }

        /// <summary>
        /// Generates the arguments.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void GenerateArguments(CommandLineBuilder builder)
        {
            builder.AppendSwitch("--count");

            base.GenerateArguments(builder);

            builder.AppendSwitch(Revision);
        }

        /// <summary>
        /// Parses a single line of text to identify any errors or warnings in canonical format.
        /// </summary>
        /// <param name="singleLine">A single line of text for the method to parse.</param>
        /// <param name="messageImportance">A value of <see cref="T:Microsoft.Build.Framework.MessageImportance"/> that indicates the importance level with which to log the message.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            bool isError = messageImportance == StandardErrorLoggingImportance;

            if (isError)
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            else
                Commits = singleLine.Trim();
        }
    }
}