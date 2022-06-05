using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Core;
using Cake.Core.IO;

namespace CodingChallenge.CakeBuild.Models;
public class StandardFolderSettings
{
    public ConvertableDirectoryPath PublishDir { get; private set; }
    public Cake.Core.IO.FilePathCollection Solutions { get; private set; }
    public string RootFullPath { get; private set; }
    public string SourceFullPath { get; private set; }
    public string TestsFullPath { get; private set; }
    public string PublishDirFullPath { get; private set; }
    public string ArtifactsDirFullPath { get; private set; }
    

    public StandardFolderSettings(ICakeContext cakeContext)
    {
        PublishDir = cakeContext.Directory("../publish/");
        Solutions = cakeContext.GetFiles("../**/*.sln");
        RootFullPath = System.IO.Path.GetFullPath(cakeContext.Directory("../"));
        SourceFullPath = System.IO.Path.GetFullPath(cakeContext.Directory("../src"));
        TestsFullPath = System.IO.Path.GetFullPath(cakeContext.Directory("../tests"));
        PublishDirFullPath = System.IO.Path.GetFullPath(cakeContext.Directory("../publish"));
        ArtifactsDirFullPath = System.IO.Path.GetFullPath(cakeContext.Directory("../artifacts"));
    }
}

