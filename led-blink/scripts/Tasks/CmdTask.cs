using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;

namespace Scripts.Tasks
{
    internal class CmdTask : ITask
    {
        public CmdTask(string fileName, string arguments, string workingDir, bool logOutput = true, bool useShellExecute = false)
        {
            this.fileName = fileName;
            this.arguments = arguments;
            this.workingDir = workingDir;
            this.logOutput = logOutput;
            this.useShellExecute = useShellExecute;
        }

        public void Dispose() { }

        public async Task<bool> ExecuteAsync(ILogger logger)
        {
            var result = false;
            try
            {
                using var process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        RedirectStandardOutput = !useShellExecute,
                        RedirectStandardError = !useShellExecute,
                        UseShellExecute = useShellExecute,
                        CreateNoWindow = !useShellExecute,
                        WorkingDirectory = workingDir
                    }
                };
                process.Start();
                string line = string.Empty;
                if (!useShellExecute)
                {
                    while (!process.HasExited | !process.StandardOutput.EndOfStream)
                    {
                        line = await process.StandardOutput.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            Output.Add(line);
                            if (logOutput)
                                logger.LogInformation(line);
                        }
                    }

                    while (!process.StandardError.EndOfStream)
                    {
                        line = await process.StandardError.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            Error.Add(line);
                            if (logOutput)
                                if (line.Contains("Error", StringComparison.OrdinalIgnoreCase))
                                    logger.LogError(line);
                                else
                                    logger.LogWarning(line);
                        }
                    }
                }

                await process.WaitForExitAsync();
                result = process.ExitCode == 0 ? true : false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToJson());
            }

            return result;
        }


        public List<string> Output = new List<string>();
        public List<string> Error = new List<string>();

        private string fileName;
        private string arguments;
        private string workingDir;
        private bool logOutput;
        private bool useShellExecute;
    }
}
