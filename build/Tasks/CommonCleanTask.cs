using Cake.Common.IO;
using Cake.Frosting;
using Cake.Common.Tools.DotNet;

namespace CodingChallenge.CakeBuild.Tasks;
[TaskName("Common-Clean-Task")]
public sealed class CommonCleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"{context.Config.StandardFolders.SourceFullPath}/**/bin/{context.Config.DotnetSettings.DotnetConfiguration}");
        context.CleanDirectories($"{context.Config.StandardFolders.SourceFullPath}/**/obj/{context.Config.DotnetSettings.DotnetConfiguration}");
        context.CleanDirectories($"{context.Config.StandardFolders.TestsFullPath}/**/bin/{context.Config.DotnetSettings.DotnetConfiguration}");
        context.CleanDirectories($"{context.Config.StandardFolders.TestsFullPath}/**/obj/{context.Config.DotnetSettings.DotnetConfiguration}");
        context.CleanDirectory(context.Config.StandardFolders.ArtifactsDirFullPath);
        context.CleanDirectory(context.Config.StandardFolders.PublishDir);
        foreach (var solution in context.Config.StandardFolders.Solutions)
        {
            context.DotNetClean(solution.FullPath, new Cake.Common.Tools.DotNet.Clean.DotNetCleanSettings()
            {
                Configuration = context.Config.DotnetSettings.DotnetConfiguration,
                Framework = context.Config.DotnetSettings.DotnetFramework
            });
            context.DotNetBuildServerShutdown();
        }
    }
}