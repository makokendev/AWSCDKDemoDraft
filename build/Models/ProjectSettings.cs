using System.Collections.Generic;

namespace CodingChallenge.CakeBuild.Models;
public class ProjectSettings
{
    public string ProjectName { get; set; }
    public bool PublishProject { get; set; }

    public bool RunCdkDeploy { get; set; }
    public List<DockerImageSetting> DockerImageSettings {get;set;}

    public class DockerImageSetting
    {
        public bool RunDockerBuild { get; set; }
        public bool RunDockerPush { get; set; }
        public string DockerRepoNameSuffix {get;set;}
        public string DockerFileName {get;set;}
        public string AwsAccountNumber {get;set;}
        public string AwsRegion {get;set;}
    }
}