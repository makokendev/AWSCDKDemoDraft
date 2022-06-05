using Cake.Common;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
namespace CodingChallenge.CakeBuild.Models;
public class DotnetSettings
{
    public string DotnetConfiguration { get; private set; }
    public DotNetVerbosity? DotnetCoreVerbosity { get; private set; }
    public string DotnetFramework { get; private set; }

    public DotnetSettings(ICakeContext cakeContext)
    {
        this.DotnetCoreVerbosity = cakeContext.Argument<DotNetCoreVerbosity>("dotNetCoreVerbosity", DotNetCoreVerbosity.Minimal);
        this.DotnetFramework = cakeContext.Argument<string>("dotnetFramework", "net6.0");
        this.DotnetConfiguration = cakeContext.Argument<string>("dotnetConfiguration", "Release");
    }
}

