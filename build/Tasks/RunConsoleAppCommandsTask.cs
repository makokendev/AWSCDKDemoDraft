using System;
using System.IO;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Frosting;
namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("Run-ConsoleApp-Commands-Task")]
public sealed class RunConsoleAppCommandsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        RunReadFileCommand(context);
    }

    private void RunReadFileCommand(BuildContext context)
    {
        var transactionFileFullPath = Path.Combine(context.Config.StandardFolders.RootFullPath, "transactions.json");
        var inlineJson = "'{\"Type\":\"Mint\",\"TokenId\":\"0xD000000000000000000000000000000000000000\",\"Address\":\"0x1000000000000000000000000000000000000000\"}'";
        RunConsoleApplicationCommand(context, $"--read-file {transactionFileFullPath}");
        RunConsoleApplicationCommand(context, "--nft 0xA000000000000000000000000000000000000000");
        RunConsoleApplicationCommand(context, "--nft 0xB000000000000000000000000000000000000000");
        RunConsoleApplicationCommand(context, "--nft 0xC000000000000000000000000000000000000000");
        RunConsoleApplicationCommand(context, "--nft 0xD000000000000000000000000000000000000000");
        // TODO - bugfix - double quites from the json is not relayed to process via process.start command.
        // Token will not be created but it is a good test to see how the application behaves.
        // this command works when called directly from command line. see utils/samplecommands.sh
        RunConsoleApplicationCommand(context, "--read-inline " + inlineJson);
        RunConsoleApplicationCommand(context, "--nft 0xD000000000000000000000000000000000000000");
        RunConsoleApplicationCommand(context, "--wallet 0x3000000000000000000000000000000000000000");
        RunConsoleApplicationCommand(context, "--reset");
        RunConsoleApplicationCommand(context, "--wallet 0x3000000000000000000000000000000000000000");
    }

    private void RunConsoleApplicationCommand(BuildContext context, string consoleAppArguments)
    {
        var projectPublishFolderName = "CodingChallenge.Console";
        var consoleAppDllFullPath = Path.Combine(context.Config.StandardFolders.PublishDirFullPath, projectPublishFolderName, $"{projectPublishFolderName}.dll");
        var commandLineArguments = $"{consoleAppDllFullPath} {consoleAppArguments}";
        var set = new Cake.Core.IO.ProcessSettings();
        set.Arguments = Cake.Core.IO.ProcessArgumentBuilder.FromString(commandLineArguments);
        set.RedirectStandardOutput = true;
        context.Information($"> Program {consoleAppArguments}");
        var processResult = context.StartProcess(
              "dotnet", set, out var redirectedStandardOutput
          );

        var outputString = string.Join(Environment.NewLine, redirectedStandardOutput);
        context.Information($"{outputString}");
    }
}
