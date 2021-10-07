using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using ICSharpCode.SharpZipLib.Tar;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;

namespace Scripts.Tasks
{
    internal class InitializeTask : ITask
    {
        public InitializeTask(ProjectOptions projectOptions, InitializeCmdLineOptions cmdLineOptions)
        {
            this.projectOptions = projectOptions;
            this.cmdLineOptions = cmdLineOptions;
        }


        public void Dispose() { }

        public async Task<bool> ExecuteAsync(ILogger logger)
        {
            var result = false;
            var tempFolder = Path.Combine(Path.GetTempPath(), $"initialize-{DateTime.Now.Ticks}");
            Directory.CreateDirectory(tempFolder);

            try
            {
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                var toolsPath = projectOptions.ToolsPath;
                var makeFileName = projectOptions.Tools.Make.ExecutableName;
                var makePath = projectOptions.Tools.Make.GetFullPath(toolsPath);
                if (!makeFileName.ExecutableExists(makePath))
                { // download and build Make
                    logger.LogInformation($"{makeFileName} not found...we will download and build the utility");
                    var makeTask = new InitializeMakeTask(tempFolder, projectOptions, cmdLineOptions);
                    if (!await makeTask.ExecuteAsync(logger))
                        return result;
                } else
                    logger.LogInformation($"{makeFileName} found.");

                var cmakeBinPath = projectOptions.Tools.CMake.GetFullPathToExecutable(toolsPath);
                var cmakeFileName = projectOptions.Tools.CMake.ExecutableName;
                if (!cmakeFileName.ExecutableExists(cmakeBinPath))
                {// download and build CMake
                    logger.LogInformation($"{cmakeFileName} not found...we will download the utility");
                    var cmakeTask = new InitializeCMakeTask(tempFolder, projectOptions, cmdLineOptions);
                    if (!await cmakeTask.ExecuteAsync(logger))
                        return result;

                }
                else
                    logger.LogInformation($"{cmakeFileName} found.");

                var gccBinPath = projectOptions.Tools.GCC.GetFullPathToExecutable(toolsPath);
                var gccFileName = projectOptions.Tools.GCC.ExecutableName; // "arm-none-eabi-c++";
                if (!gccFileName.ExecutableExists(gccBinPath))
                { // download GNU ARM toolchain
                    logger.LogInformation($"{gccFileName} not found...we will download GNU ARM toolchain");
                    var gccTask = new InitializeGnuArmToolChainTask(tempFolder, projectOptions, cmdLineOptions);
                    if (!await gccTask.ExecuteAsync(logger))
                        return result;
                }
                else
                    logger.LogInformation($"{gccFileName} found.");


                result = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToJson());
                return false;
            }

            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);

            return result;
        }

        private ProjectOptions projectOptions;
        private InitializeCmdLineOptions cmdLineOptions;
    }

    [Verb("initialize", HelpText = "Automatic tools download and setup")]
    public class InitializeCmdLineOptions { }
}
