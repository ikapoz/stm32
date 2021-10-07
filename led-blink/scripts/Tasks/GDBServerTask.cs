using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;

namespace Scripts.Tasks
{
    internal class GDBServerTask : ITask
    {
        public GDBServerTask(ProjectOptions projectOptions, GDBServerCmdLineOptions cmdLineOptions)
        {
            this.projectOptions = projectOptions;
            this.cmdLineOptions = cmdLineOptions;
        }

        public void Dispose() { }

        public Task<bool> ExecuteAsync(ILogger logger)
        {
            var result = false;
            try
            {
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
                {
                    logger.LogInformation($"Starting GDB server.");

                    var executablePath = projectOptions.Tools.GDB.GetFullPathToExecutable(projectOptions.ToolsPath);
                    var executableWithExt =  projectOptions.Tools.GDB.ExecutableName.GetExecutableNameWithExtension();
                    var executableFileName = projectOptions.Tools.GDB.ExecutableName.GetExecutableFullPath(executablePath);
                    if (!File.Exists(executableFileName))
                    {
                        logger.LogError($"GDB server not found on: {executableFileName}.");
                        logger.LogInformation("Please read the ReadMe.md to see how to download and setup STM32 GDB server");
                        return Task.FromResult(result);
                    }

                    var stm32ToolsPath = projectOptions.Tools.Stm32Tools.GetFullPathToExecutable(projectOptions.ToolsPath);
                    var arguments = $"/K {executableWithExt} {cmdLineOptions.Arguments} -cp \"{stm32ToolsPath}\" ";
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = arguments,
                            RedirectStandardOutput = false,
                            RedirectStandardError = false,
                            UseShellExecute = true,
                            CreateNoWindow = false,
                            WorkingDirectory = executablePath
                        }
                    };
                    process.Start();
                    result = true;
                }
                else
                {
                    logger.LogError("OS not supported");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToJson());
            }

            return Task.FromResult(result);
        }


        private ProjectOptions projectOptions;
        private GDBServerCmdLineOptions cmdLineOptions;
    }


    [Verb("gdbserver", HelpText = "Starts GDB server")]
    public class GDBServerCmdLineOptions
    {
        [Option('a', "arguments", HelpText = "Arguments that will be passed to GDB server")]
        public string Arguments { get; set; } = "-l 31 -s -d";
    }
}
