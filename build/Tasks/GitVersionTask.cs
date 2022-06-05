using System;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.GitVersion;
using Cake.Frosting;
using Cake.Json;

namespace CodingChallenge.CakeBuild.Tasks;

[TaskName("GitVersionTask")]
public sealed class GitVersionTask : FrostingTask<BuildContext>
{
    public GitVersion GitVersionResult { get; set; }
    public override void Run(BuildContext context)
    {
        try
        {
            context.Information("Calculating Git Version - manually set branch to development");
            try
            {
                var settings = new GitVersionSettings
                {
                    NoFetch = true,
                };


                GitVersionResult = context.GitVersion(settings);
            }
            catch (Exception ex)
            {
                context.Information($"Something went wrong while calculating git version. Exception message: {ex.Message}");
            }
            context.Information("Current branch is " + GitVersionResult.BranchName);
            context.Information("MajorMinorPatch: {0}", GitVersionResult.MajorMinorPatch);
            context.Information("FullSemVer: {0}", GitVersionResult.FullSemVer);
            context.Information("context.InformationalVersion: {0}", GitVersionResult.InformationalVersion);
            context.Information("LegacySemVer: {0}", GitVersionResult.LegacySemVer);
            context.Information("Nuget v1 version: {0}", GitVersionResult.NuGetVersion);
            context.Information("Nuget v2 version: {0}", GitVersionResult.NuGetVersionV2);

            context.Config.AwsApplication.Version = GitVersionResult.NuGetVersionV2;
        }
        catch (Exception ex)
        {
            context.Information("error is: " + ex.Message);
            var serializedJson = context.SerializeJson(ex);
            context.Information($"serialized error {serializedJson}");
            throw;
        }
    }
}
