using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// A task for git commit.
    /// </summary>
    public class GitCommit : GitClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitCommit"/> class.
        /// </summary>
        public GitCommit()
        {
            Command = "commit";
        }

        /// <summary>
        /// Gets or sets the commit message.
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Whether or not to add modified and deleted files to the commit
        /// </summary>
        public bool AddModifiedFiles { get; set; }

        /// <summary>
        /// Generates the arguments.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void GenerateArguments(CommandLineBuilder builder)
        {
            base.GenerateArguments(builder);

            if (AddModifiedFiles)
            {
                builder.AppendSwitch("-a");
            }

            builder.AppendSwitchIfNotNull("-m", Message);
        }
    }
}