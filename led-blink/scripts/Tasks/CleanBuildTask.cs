using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;

namespace Scripts.Tasks
{
    internal class CleanBuildTask : ITask
    {
        internal CleanBuildTask(ProjectOptions projectOptions, CleanBuildCmdLineOptions cmdLineOptions)
        {
            this.projectOptions = projectOptions;
            this.cmdLineOptions = cmdLineOptions;
        }

        public void Dispose() { }

        public Task<bool> ExecuteAsync(ILogger logger)
        {
            return Task.Run(() =>
             {
                 try
                 {
                     logger.LogInformation("Clean build executing");
                     var path = Path.Combine(projectOptions.BaseProjectPath, "build");
                     if (Directory.Exists(path))
                     {
                         logger.LogInformation("Deleting build folder");
                         Directory.Delete(path, true);
                     }
                     else
                         logger.LogInformation("Nothing to delete");

                     logger.LogInformation("Clean build finished");
                 }
                 catch (Exception ex)
                 {
                     logger.LogError(ex.ToJson());
                     return false;
                 }

                 return true;
             });
        }


        private ProjectOptions projectOptions;
        private CleanBuildCmdLineOptions cmdLineOptions;
    }


    [Verb("clean", HelpText = "Deletes build folder")]
    internal class CleanBuildCmdLineOptions { }

    internal record CleanBuildOptions(string BaseProjectPath);
}
