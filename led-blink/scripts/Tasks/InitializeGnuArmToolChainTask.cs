using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;

namespace Scripts.Tasks
{
    internal class InitializeGnuArmToolChainTask : ITask
    {
        public InitializeGnuArmToolChainTask(string tempFolder, ProjectOptions projectOptions, InitializeCmdLineOptions cmdLineOptions)
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
                    var gnuArmFolderName = "gcc-arm-none-eabi-10.3-2021.07";
                    logger.LogInformation($"Downloading {gnuArmFolderName} binary. Be patient it is huge file.");
                    var gnuArmUrl = $"https://developer.arm.com/-/media/Files/downloads/gnu-rm/10.3-2021.07/{gnuArmFolderName}-win32.zip";
                    using var httpClient = new HttpClient();
                    using var stream = await httpClient.GetStreamAsync(gnuArmUrl);
                    using var zipStream = new ZipArchive(stream);
                    logger.LogInformation($"Extracting {gnuArmFolderName} binary.");
                    zipStream.ExtractToDirectory(tempFolder, true);
                    var srcPath = Path.Combine(tempFolder, gnuArmFolderName);
                    var dstPath = projectOptions.Tools.GCC.GetFullPath(projectOptions.ToolsPath);
                    logger.LogInformation($"Coping {gnuArmFolderName} binaries to {dstPath}.");
                    Directory.Move(srcPath, dstPath);
                    result = true;
                }
                else if (OperatingSystem.IsLinux())
                {
                    var gnuArmFolderName = "gcc-arm-none-eabi-10.3-2021.07";
                    logger.LogInformation($"Downloading {gnuArmFolderName} binary. Be patient it is huge file.");
                    var gnuArmUrl = $"https://developer.arm.com/-/media/Files/downloads/gnu-rm/10.3-2021.07/{gnuArmFolderName}-x86_64-linux.tar.bz2";
                    using var httpClient = new HttpClient();
                    using var stream = await httpClient.GetStreamAsync(gnuArmUrl);
                    using var tarStream = new MemoryStream();
                    BZip2.Decompress(stream, tarStream, false);
                    //using var tarStream = new GZipStream(stream, CompressionMode.Decompress);
                    tarStream.Seek(0, SeekOrigin.Begin);
                    using var tar = TarArchive.CreateInputTarArchive(tarStream, Encoding.UTF8);
                    logger.LogInformation($"Extracting {gnuArmFolderName} binary.");
                    tar.ExtractContents(tempFolder);
                    var srcPath = Path.Combine(tempFolder, gnuArmFolderName);
                    var dstPath = projectOptions.Tools.GCC.GetFullPath(projectOptions.ToolsPath);
                    var cmd = "mv";
                    var arugments = $"-v \"{srcPath}\"  \"{dstPath}\" ";

                    logger.LogInformation($"Coping {gnuArmFolderName} binaries to {dstPath}.");
                    using var task = new CmdTask(cmd, arugments, srcPath, true, true);
                    result = await task.ExecuteAsync(logger);
                    if (!result)
                        return result;
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
