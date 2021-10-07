using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Check if executable exists on different OS
        /// On Windows it will append filename.exe
        /// On OSx it will append filename.app
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileNameWithoutExtensions"></param>
        /// <returns></returns>
        internal static bool ExecutableExists(this string fileNameWithoutExtension, string pathToExecutable)
        {
            return File.Exists(fileNameWithoutExtension.GetExecutableFullPath(pathToExecutable));
        }

        internal static string GetExecutableFullPath(this string fileNameWithoutExtension, string pathToExecutable)
        {
            if (OperatingSystem.IsWindows())
                return Path.GetFullPath(Path.Combine(pathToExecutable, $"{fileNameWithoutExtension}.exe"));
            else if (OperatingSystem.IsLinux())
                return Path.GetFullPath(Path.Combine(pathToExecutable, fileNameWithoutExtension));
            else if (OperatingSystem.IsMacOS())
                return Path.GetFullPath(Path.Combine(pathToExecutable, $"{fileNameWithoutExtension}.app"));

            return fileNameWithoutExtension;
        }

        internal static string GetExecutableNameWithExtension(this string executableNameWithoutExtension)
        {
            if (OperatingSystem.IsWindows())
                return $"{executableNameWithoutExtension}.exe";
            else if (OperatingSystem.IsLinux())
                return executableNameWithoutExtension;
            else if (OperatingSystem.IsMacOS())
                return $"{executableNameWithoutExtension}.app";

            return executableNameWithoutExtension;
        }
    }
}
