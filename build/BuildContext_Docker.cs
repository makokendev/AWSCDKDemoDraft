using Cake.Core;
using Cake.Frosting;
using System;
using System.Linq;
using Cake.Common;
using Cake.Core.IO;
using Cake.Common.Diagnostics;
using CodingChallenge.Cdk;

namespace CodingChallenge.CakeBuild;

public partial class BuildContext : FrostingContext
{


    public void UseDefaultDockerContext()
    {
        //todo - get the values from environment variables!
        var arguments = new ProcessArgumentBuilder()
                                       .Append("context")
                                       .Append("use")
                                       .Append("default");

        var exitCodeWithArgument =
                 _context.StartProcess(
                     "docker",
                     new ProcessSettings
                     {
                         Arguments = arguments,
                         RedirectStandardOutput = true
                     },
                     out var redirectedStandardOutput
                 );
        var outputString = string.Join(System.Environment.NewLine, redirectedStandardOutput);
        this.Information($"command -- dotnet {arguments.Render()}");
        this.Information($"exit code -- {exitCodeWithArgument}");
        this.Information($"output -- {outputString}");
    }
    public void DockerEcrLogin(string awsAccountNumber,string awsRegion)
    {
        var logPrefix = "DOCKER ECR PUSH";
        if (string.IsNullOrWhiteSpace(awsRegion))
        {
            throw new ArgumentNullException($"{logPrefix} aws region can not be null");
        }
        if (string.IsNullOrWhiteSpace(awsAccountNumber))
        {
            throw new ArgumentNullException($"{logPrefix} aws account number can not be null");
        }
        var getLoginPasswordArguments = new ProcessArgumentBuilder()
                                       .Append("ecr")
                                       .Append("get-login-password")
                                       .Append("--region")
                                       .Append($"{awsRegion}");

        var getLoginPasswordArgumentsExitCode =
                 _context.StartProcess(
                     "aws",
                     new ProcessSettings
                     {
                         Arguments = getLoginPasswordArguments,
                         RedirectStandardOutput = true
                     },
                     out var redirectedStandardOutput
                 );
        var outputString = string.Join(System.Environment.NewLine, redirectedStandardOutput);

        // var tempFileFullPath = System.IO.Path.Combine(this.Config.StandardFolders.RootFullPath, "my_password.txt");
        // this.FileWriteText($"{tempFileFullPath}", outputString);
        // this.Information($"{logPrefix} -- aws {arguments.Render()}");
        // this.Information($"{logPrefix} - output file -- {tempFileFullPath}");
        var dockerLoginArguments = new ProcessArgumentBuilder()
                                      .Append($"login")
                                      .Append($"{awsAccountNumber}.dkr.ecr.{awsRegion}.amazonaws.com")
                                      .Append($"--username")
                                      .Append($"AWS")
                                      .Append($"--password")
                                      .Append($"{outputString}");

        var dockerLoginArgumentsExitCode =
                 _context.StartProcess(
                     "docker",
                     new ProcessSettings
                     {
                         Arguments = dockerLoginArguments,
                         RedirectStandardOutput = true
                     },
                     out var redirectedStandardOutput2
                 );
        var outputString2 = string.Join(System.Environment.NewLine, redirectedStandardOutput);

        this.Information($"{logPrefix} -- docker {dockerLoginArguments.Render()}");
        this.Information($"{logPrefix} -- exit code - {getLoginPasswordArgumentsExitCode}");
        this.Information($"{logPrefix} -- output result - {outputString}");
    }


}
