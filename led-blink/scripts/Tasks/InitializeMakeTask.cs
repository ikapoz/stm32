using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.SharpZipLib.Tar;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;

namespace Scripts.Tasks
{
    internal class InitializeMakeTask : ITask
    {
        public InitializeMakeTask(string tempFolder, ProjectOptions projectOptions, InitializeCmdLineOptions cmdLineOptions)
        {
            this.tempFolder = tempFolder;
            this.projectOptions = projectOptions;
            this.cmdLineOptions = cmdLineOptions;
        }

        public void Dispose() { }

        public async Task<bool> ExecuteAsync(ILogger logger)
        {
            var result = false;
            try
            {
                var makeFolderName = "make-4.3";
                logger.LogInformation($"Downloading {makeFolderName} source code.");
                var makeUrl = $"http://ftp.gnu.org/gnu/make/{makeFolderName}.tar.gz";
                var makeZipPath = Path.Combine(tempFolder, "make.tar");
                using var httpClient = new HttpClient();
                using var stream = await httpClient.GetStreamAsync(makeUrl);
                //using var fileStream = File.OpenWrite(makeZipPath);
                using var tarStream = new GZipStream(stream, CompressionMode.Decompress);
                using var tar = TarArchive.CreateInputTarArchive(tarStream, Encoding.UTF8);
                tar.ExtractContents(tempFolder);
                var makeFolder = Path.Combine(tempFolder, makeFolderName);

                var canExecute = false;
                string fileName = string.Empty;
                string arguments = string.Empty;
                string buildPath = string.Empty;
                if (OperatingSystem.IsWindows())
                {
                    var buildBatFileResult = await GetBuildBatFullPath(makeFolder, logger);
                    if (!buildBatFileResult.Success)
                        return false;


                    canExecute = true;
                    fileName = buildBatFileResult.Data ?? "";
                    buildPath = Path.Combine(makeFolder, "WinRel");

                    logger.LogInformation($"Building {makeFolderName} source code.");
                    using var task = new CmdTask(fileName, arguments, makeFolder, true, true);
                    result = await task.ExecuteAsync(logger);
                    if (!result)
                        return result;

                    var destPath = projectOptions.Tools.Make.GetFullPath(projectOptions.ToolsPath);
                    logger.LogInformation($"Coping {makeFolderName} binaries to {destPath}.");
                    Directory.Move(buildPath, destPath);

                    var gnumakeFileName = "gnumake";
                    var toolsPath = projectOptions.ToolsPath;
                    var makePath = projectOptions.Tools.Make.GetFullPath(toolsPath);
                    var gnumakePath = gnumakeFileName.GetExecutableFullPath(makePath);
                    var makeFileName = "make";
                    var makeFilePath = makeFileName.GetExecutableFullPath(makePath);
                    File.Copy(gnumakePath, makeFilePath);

                }
                else if (OperatingSystem.IsLinux())
                {
                    logger.LogInformation($"Configure {makeFolderName} source code.");
                    fileName = "configure";
                    buildPath = Path.Combine(makeFolder);
                    result = await ExecuteCmd("chmod", $"+x {fileName}", makeFolder, logger);
                    if (!result)
                        return result;

                    result = await ExecuteCmd(fileName, arguments, makeFolder, logger);
                    if (!result)
                        return result;

                    logger.LogInformation($"Build {makeFolderName} source code.");
                    fileName = "build.sh"; 
                    buildPath = Path.Combine(makeFolder);
                    result = await ExecuteCmd("chmod", $"+x {fileName}", makeFolder, logger);
                    if (!result)
                        return result;

                    result = await ExecuteCmd(fileName, arguments, makeFolder, logger);
                    if (!result)
                        return result;

                    var dstPath = projectOptions.Tools.Make.GetFullPath(projectOptions.ToolsPath);
                    Directory.CreateDirectory(dstPath);
                    var srcMakeFilePath = Path.Combine(makeFolder, "make");
                    var dstMakeFilePath = Path.Combine(dstPath, "make");
                    File.Copy(srcMakeFilePath, dstMakeFilePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToJson());
            }

            return result;
        }

        private async Task<bool> ExecuteCmd(string fileName, string arguments, string workingDir, ILogger logger)
        {
            var task = new CmdTask(fileName, arguments, workingDir, true, true);
            return await task.ExecuteAsync(logger);
        }

        private async Task<Result<string>> GetBuildBatFullPath(string makeSrcFolderPath, ILogger logger)
        {
            var result = new Result<string>();
            // check for vs is installed
            var vsInstallerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft Visual Studio\Installer\");
            var vsWhere = Path.Combine(vsInstallerPath, "vswhere.exe");
            if (!File.Exists(vsWhere))
            {
                logger.LogError("Could not found any Visual Studio installed");
                return result;
            }

            // check if vc is installed
            var task = new CmdTask(vsWhere, "-latest", vsInstallerPath, false);
            var taskResult = await task.ExecuteAsync(logger);
            if (!taskResult)
                return result;

            var installationPathPrefix = "installationPath:";
            var found = task.Output.First(line => line.StartsWith(installationPathPrefix));
            var vsInstallPath = found.Substring(installationPathPrefix.Length).Trim();
            var vcvars64batPath = Path.Combine(vsInstallPath, @"VC\Auxiliary\Build\vcvars64.bat");
            if (!File.Exists(vcvars64batPath))
            {
                logger.LogError($"Could not find VC compiler in {vsInstallPath}");
                logger.LogError($"Please add \"Desktop development with C++\" and try again.");
                return result;
            }

            logger.LogInformation($"Found Visual Studio: {vsInstallPath}");

            // creating build batch file
            var buildBatFullPath = Path.Combine(makeSrcFolderPath, "build.bat");
            using var outFile = new StreamWriter(buildBatFullPath);
            outFile.WriteLine($"call \"{vcvars64batPath}\" ");
            outFile.WriteLine($"call \"{makeSrcFolderPath}\\build_w32.bat\"");
            outFile.Flush();
            outFile.Close();

            return new Result<string>(buildBatFullPath);
        }

        private string tempFolder;
        private ProjectOptions projectOptions;
        private InitializeCmdLineOptions cmdLineOptions;
    }
}
