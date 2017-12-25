using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Git
{
    /// <summary>
    /// A task for git commit.
    /// </summary>
    public class GitTag : GitClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitTag"/> class.
        /// </summary>
        public GitTag()
        {
            Command = "tag";
        }

        /// <summary>
        /// Gets or sets the tag label.
        /// </summary>
        [Required]
        public string Label { get; set; }

        /// <summary>
        /// Commit hash to add the tag to. If empty, tag HEAD.
        /// </summary>
        public string CommitHash { get; set; }

        /// <summary>
        /// For annotated tags, gets or sets the tag message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Whether or not to add modified files to the commit
        /// </summary>
        public bool Annotated { get; set; }

        /// <summary>
        /// Generates the arguments.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void GenerateArguments(CommandLineBuilder builder)
        {
            base.GenerateArguments(builder);

            if (Annotated)
            {
                builder.AppendSwitch("-a");
                builder.AppendSwitch(Label);

                if (!string.IsNullOrWhiteSpace(Message))
                {
                    builder.AppendSwitchIfNotNull("-m", Message);
                }
            }
            else
            {
                builder.AppendSwitch(Label);
            }

            if (!string.IsNullOrWhiteSpace(CommitHash))
            {
                builder.AppendTextUnquoted("\"" + CommitHash + "\"");
            }
        }
    }
}