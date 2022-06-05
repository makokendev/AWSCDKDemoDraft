using Cake.Core;
using Cake.Frosting;
using CodingChallenge.CakeBuild.Models;

namespace CodingChallenge.CakeBuild;

public partial class BuildContext : FrostingContext
{
    public ICakeContext _context { get; set; }
    public Settings Config { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        _context = context;
        Config = new Settings(context);
    }

    public string GetProjectFilePathUsingBuiltinArguments(string projectName)
    {
        return $"{this.Config.StandardFolders.SourceFullPath}/{projectName}/{projectName}.csproj";
    }
}
