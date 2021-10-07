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
    internal class CMakeTask : ITask
    {
        public CMakeTask(ProjectOptions projectOptions, CMakeCmdLineOptions cmdLineOptions)
        {
            this.projectOptions = projectOptions;
            this.cmdLineOptions = cmdLineOptions;
        }
        public void Dispose() { }

        public async Task<bool> ExecuteAsync(ILogger logger)
        {
            var result = false;
            try
            {
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
                {
                    logger.LogInformation($"Executing CMake task.");
                    var cmakeExecutablePath = projectOptions.Tools.CMake.GetFullPathToExecutable(projectOptions.ToolsPath);
                    var cmakeExecutableFileName = projectOptions.Tools.CMake.ExecutableName.GetExecutableFullPath(cmakeExecutablePath);
                    if (!File.Exists(cmakeExecutableFileName))
                    {
                        logger.LogError($"CMake not found on: {cmakeExecutableFileName}.");
                        logger.LogInformation("Please initialize project first. Execute initialize task");
                        return result;
                    }

                    var makeExecutablePath = projectOptions.Tools.Make.GetFullPathToExecutable(projectOptions.ToolsPath);
                    var makeExecutableFileName = projectOptions.Tools.Make.ExecutableName.GetExecutableFullPath(makeExecutablePath);
                    if (!File.Exists(cmakeExecutableFileName))
                    {
                        logger.LogError($"Make not found on: {cmakeExecutableFileName}.");
                        logger.LogInformation("Please initialize project first. Execute initialize task");
                        return result;
                    }

                    var srcDir = projectOptions.BaseProjectPath;
                    var buildDir = Path.Combine(srcDir, "build");
                    var toolchainFile = Path.Combine(srcDir, "cmake", "toolchain.cmake");
                    var arguments = $"-S \"{srcDir}\" -B \"{buildDir}\" -G \"Unix Makefiles\" -D CMAKE_TOOLCHAIN_FILE=\"{toolchainFile}\" -D CMAKE_MAKE_PROGRAM:PATH=\"{makeExecutableFileName}\" -D CMAKE_BUILD_TYPE={cmdLineOptions.Target} -Wdev --log-level={cmdLineOptions.LogLevel} ";

                    if (!Directory.Exists(buildDir))
                        Directory.CreateDirectory(buildDir);

                    var cmdTask = new CmdTask(cmakeExecutableFileName, arguments, buildDir);
                    if (!await cmdTask.ExecuteAsync(logger))
                        return result;

                    arguments = $"--build \"{buildDir}\" -j {cmdLineOptions.Parallel} -v";
                    cmdTask = new CmdTask(cmakeExecutableFileName, arguments, buildDir);
                    if (!await cmdTask.ExecuteAsync(logger))
                        return result;

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

            return result;
        }


        private ProjectOptions projectOptions;
        private CMakeCmdLineOptions cmdLineOptions;
    }

    [Verb("cmake", HelpText = "CMake build tool")]
    public class CMakeCmdLineOptions
    {
        //[Option(shortName: 'S', HelpText = "Path to source")]
        //internal string SourceDir { get; set; } = string.Empty;

        //[Option(shortName: 'B', HelpText = "Path to build")]
        //internal string BuildDir { get; set; } = string.Empty;

        //[Option(shortName: 'G', HelpText = "Generator")]
        //internal string Generator { get; set; } = string.Empty;

        //[Option("toolchain", HelpText = "Specify the cross compiling toolchain file.")]
        //internal string ToolChain { get; set; } = string.Empty;

        [Option("log-level", Default = "STATUS", HelpText = "Set Log Level: [ERROR|WARNING|NOTICE|STATUS|VERBOSE|DEBUG|TRACE]")]
        public string LogLevel { get; set; } = string.Empty;

        [Option("parallel", HelpText = "Maximum number of concurrent processes to use when building.")]
        public uint Parallel { get; set; } = Convert.ToUInt32(Environment.ProcessorCount);

        [Option("target", HelpText = "Build target instead of the default target")]
        public string Target { get; set; } = string.Empty;

        [Option('D', HelpText = "Create or update CMake cache entry")]
        public IEnumerable<string> DList { get; set; } = new List<string>();

        [Option(HelpText = "CMake arguments with values")]
        public string Arguments { get; set; } = string.Empty;
    }
}
