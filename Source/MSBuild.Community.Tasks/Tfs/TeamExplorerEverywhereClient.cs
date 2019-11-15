namespace MSBuild.Community.Tasks.Tfs
{
    /// <summary>
    /// A task for Team Foundation Server version control.
    /// </summary>
    public class TeamExplorerEverywhereClient : TfsClient
    {
        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <returns>
        /// The name of the executable file to run.
        /// </returns>
        protected override string ToolName
        {
            get { return "tf.cmd"; }
        }
    }
}
