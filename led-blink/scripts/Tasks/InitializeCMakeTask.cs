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
    internal class InitializeCMakeTask : ITask
    {
        public InitializeCMakeTask(string tempFolder, ProjectOptions projectOptions, InitializeCmdLineOptions cmdLineOptions)
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
                if (OperatingSystem.IsWindows())
                {
                    var cmakeFolderName = "cmake-3.21.3-windows-x86_64";
                    logger.LogInformation($"Downloading {cmakeFolderName} binary.");
                    var makeUrl = $"https://github.com/Kitware/CMake/releases/download/v3.21.3/{cmakeFolderName}.zip";
                    using var httpClient = new HttpClient();
                    using var stream = await httpClient.GetStreamAsync(makeUrl);
                    using var zipStream = new ZipArchive(stream);
                    logger.LogInformation($"Extracting {cmakeFolderName} binary.");
                    zipStream.ExtractToDirectory(tempFolder, true);
                    var srcPath = Path.Combine(tempFolder, cmakeFolderName);
                    var dstPath = projectOptions.Tools.CMake.GetFullPath(projectOptions.ToolsPath);
                    logger.LogInformation($"Coping {cmakeFolderName} binaries to {dstPath}.");
                    Directory.Move(srcPath, dstPath);
                    result = true;
                }else if (OperatingSystem.IsLinux())
                {
                    var cmakeFolderName = "cmake-3.21.3-linux-x86_64";
                    logger.LogInformation($"Downloading {cmakeFolderName} binary.");
                    var makeUrl = $"https://github.com/Kitware/CMake/releases/download/v3.21.3/{cmakeFolderName}.tar.gz";
                    using var httpClient = new HttpClient();
                    using var stream = await httpClient.GetStreamAsync(makeUrl);
                    using var tarStream = new GZipStream(stream, CompressionMode.Decompress);
                    using var tar = TarArchive.CreateInputTarArchive(tarStream, Encoding.UTF8);
                    tar.ExtractContents(tempFolder);
                    var makeFolder = Path.Combine(tempFolder, cmakeFolderName);
                    var srcPath = Path.Combine(tempFolder, cmakeFolderName);
                    var dstPath = projectOptions.Tools.CMake.GetFullPath(projectOptions.ToolsPath);
                    var cmd = "mv";
                    var arugments = $"-v \"{srcPath}\"  \"{dstPath}\" ";

                    logger.LogInformation($"Coping {cmakeFolderName} binaries to {dstPath}.");
                    using var task = new CmdTask(cmd, arugments, srcPath, true, true);
                    result = await task.ExecuteAsync(logger);
                    if (!result)
                        return result;
                    
                    // Directory.Move(srcPath, dstPath);
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

        private string tempFolder;
        private ProjectOptions projectOptions;
        private InitializeCmdLineOptions cmdLineOptions;
    }
}
