using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

using CommandLine;

using Microsoft.Extensions.Logging;

using Scripts.Extensions;
using Scripts.Tasks;
// See https://aka.ms/new-console-template for more information
// setup
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(configure =>
    {
        configure.IncludeScopes = false;
        configure.SingleLine = true;
    });
});
var logger = loggerFactory.CreateLogger(">");

// main
try
{
    logger.LogInformation("Initializing");
    var projectOptionsResult = await GetProjectOptions(logger);
    if (!projectOptionsResult.Success)
        return;

    if (projectOptionsResult.Data == null)
        return;

    var projectOptions = projectOptionsResult.Data;
    Type[] verbsOptions =
        {
            typeof(CleanBuildCmdLineOptions),
            typeof(InitializeCmdLineOptions),
            typeof(GDBServerCmdLineOptions),
            typeof(CMakeCmdLineOptions),
        };

    var result = await Parser.Default.ParseArguments(args, verbsOptions)
        .WithNotParsed(errors =>
        {
            foreach (UnknownOptionError error in errors)
                logger.LogError($"Unknown option: {error.Token}");
        })
        .WithParsedAsync(async (options) =>
        {
            switch (options)
            {
                case InitializeCmdLineOptions cmdLineOptions:
                    {
                        var task = new InitializeTask(projectOptions, cmdLineOptions);
                        await task.ExecuteAsync(logger);
                        break;

                    }
                case CleanBuildCmdLineOptions cmdLineOptions:
                    {
                        var task = new CleanBuildTask(projectOptions, cmdLineOptions);
                        await task.ExecuteAsync(logger);
                        break;
                    }
                case GDBServerCmdLineOptions cmdLineOptions:
                    {
                        var task = new GDBServerTask(projectOptions, cmdLineOptions);
                        var result = await task.ExecuteAsync(logger);
                        break;
                    }
                case CMakeCmdLineOptions cmdLineOptions:
                    {
                        var task = new CMakeTask(projectOptions, cmdLineOptions);
                        var result = await task.ExecuteAsync(logger);
                        break;
                    }
                default:
                    break;
            }
        });
}
catch (Exception ex)
{
    logger.LogError($"Exception:{ex.ToJson()}");
}

async Task<Result<ProjectOptions>> GetProjectOptions(ILogger logger)
{
    var result = new Result<ProjectOptions>();
    try
    {
        var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        var optionsPath = Path.Combine(execPath, "options.json");
        if (!File.Exists(optionsPath))
        {
            logger.LogError($"Please provide options.json on path: {execPath}");
            return result;
        }

        // using var reader = new StreamReader(optionsPath);
        using var reader = File.OpenRead(optionsPath);
        var baseProjOptions = await JsonSerializer.DeserializeAsync<ProjectOptions>(reader);
        var baseProjectPath = Path.GetFullPath(Path.Combine(execPath, @"..\..\..\.."));
        if (baseProjOptions != null)
        {
            var toolsPath = Path.GetFullPath(Path.Combine(baseProjectPath, baseProjOptions.ToolsPath));
            var projectOptions = new ProjectOptions(baseProjectPath, toolsPath, new Tools());
            return new Result<ProjectOptions>(projectOptions);
        }
        else
            logger.LogError("Problem on deserializing options.json");
    }
    catch (System.Exception ex)
    {
        logger.LogError($"Exception:{ex.ToJson()}");
    }

    return result;
}

// helper class/records
internal class ToolInfo
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Tool name</param>
    /// <param name="ExecutableName"></param>
    /// <param name="FolderName">base folder name</param>
    /// <param name="RelativePathToExecutableNameFromFolderName">subfolder where excutable is</param>
    public ToolInfo(string name, string executableName, string folderName, string relativePathToExecutable)
    {
        this.name = name; ;
        this.executableName = executableName;
        this.folderName = folderName;
        this.relativePathToExecutableName = relativePathToExecutable;
    }

    public string Name => name;
    public string ExecutableName => executableName;
    public string FolderName => folderName;
    public string RelativePathToExecutable => relativePathToExecutableName;

    public string GetFullPath(string basePath) => Path.GetFullPath(Path.Combine(basePath, folderName));

    public string GetFullPathToExecutable(string basePath) => Path.GetFullPath(Path.Combine(basePath, folderName, relativePathToExecutableName));

    private string name;
    private string executableName;
    private string folderName;
    private string relativePathToExecutableName;
}

/// <summary>
/// Tools/Toolchains used in the project
/// </summary>
internal class Tools
{
    public Tools() { }

    public ToolInfo Make => make;
    public ToolInfo CMake => cmake;
    public ToolInfo GCC => gcc;
    public ToolInfo Stm32Tools => stm32Tools;
    public ToolInfo GDB => gdbServer;

    private ToolInfo make = new ToolInfo("make", "gnumake", "make", ".");
    private ToolInfo cmake = new ToolInfo("cmake", "cmake", "cmake", "bin");
    private ToolInfo gcc = new ToolInfo("gcc", "", "gcc", @"bin");
    private ToolInfo stm32Tools = new ToolInfo("stm32tools", "", "stm32-tools", "bin");
    private ToolInfo gdbServer = new ToolInfo("gdb server", "ST-LINK_gdbserver", "gdb", ".");
}

record ProjectOptions(string BaseProjectPath, string ToolsPath, Tools Tools);