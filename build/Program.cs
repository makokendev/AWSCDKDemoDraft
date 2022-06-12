using Cake.Frosting;
using CodingChallenge.CakeBuild;
using CodingChallenge.CakeBuild.Models;
using CodingChallenge.CakeBuild.Tasks;
using CodingChallenge.Infrastructure;

public static class Extensions
{
    public static CakeHost InstallBaseTools(this CakeHost cakehost)
    {
        return cakehost.
            InstallTool(new System.Uri("dotnet:?package=GitVersion.Tool&version=5.8.1"))
            .InstallTool(new System.Uri("nuget:?package=nuget.commandline&version=5.3.0"));
    }
    public static AWSAppProject ToAwsApplication(this MetaData metadata)
    {
        return new AWSAppProject()
        {
            Version = metadata.Version,
            System = metadata.System,
            Subsystem = metadata.Subsystem,
            Platform = metadata.Platform,
            Environment = metadata.Environment,
        };
    }
}
public static class Program
{
    public static void SetEnvironmentVariables()
    {

    }
    public static int Main(string[] args)
    {
        SetEnvironmentVariables();
        return new CakeHost()
            .UseContext<BuildContext>()
            //.AddAssembly(typeof(BuildContext).Assembly)
            .InstallBaseTools()
            .Run(args);
    }
}

[TaskName("Dotnet-Build-Task")]
[IsDependentOn(typeof(CommonCleanTask))]
[IsDependentOn(typeof(SetupTask))]
[IsDependentOn(typeof(GitVersionTask))]
[IsDependentOn(typeof(DotnetCoreCleanTask))]
[IsDependentOn(typeof(DotnetCoreRestoreTask))]
[IsDependentOn(typeof(DotnetCoreBuildTask))]
[IsDependentOn(typeof(DotnetCoreUnitTestTask))]
[IsDependentOn(typeof(DotnetCorePublishTask))]

public class DotnetBuildTask : FrostingTask
{
}
[TaskName("Default")]
[IsDependentOn(typeof(DotnetBuildTask))]
[IsDependentOn(typeof(RunConsoleAppCommandsTask))]
public class DefaultTask : FrostingTask
{
}
[TaskName("doc")]
[IsDependentOn(typeof(UploadDocumentationToStorage))]

public class DocTask : FrostingTask
{
}
[TaskName("InfraDeploy")]
[IsDependentOn(typeof(DotnetBuildTask))]
[IsDependentOn(typeof(CdkInfraStackDeployTask))]
public class InfraStackDeployTask : FrostingTask
{
}
[TaskName("DatabaseDeploy")]
[IsDependentOn(typeof(DotnetBuildTask))]
[IsDependentOn(typeof(CdkDatabaseStackDeployTask))]
public class DatabaseStackDeployTask : FrostingTask
{
}
[TaskName("Deploy")]
[IsDependentOn(typeof(DotnetBuildTask))]
[IsDependentOn(typeof(DockerBuildTask))]
[IsDependentOn(typeof(DockerEcrPushTask))]
[IsDependentOn(typeof(CdkMainStackDeployTask))]
public class MainStackDeployTask : FrostingTask
{
}
[TaskName("Build")]
[IsDependentOn(typeof(DotnetBuildTask))]
[IsDependentOn(typeof(DockerBuildTask))]
[IsDependentOn(typeof(DockerEcrPushTask))]
public class BuildTask : FrostingTask
{
}
