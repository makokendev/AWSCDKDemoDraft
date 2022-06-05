namespace CodingChallenge.Cdk;

public class AwsAppProject : ICloudProject
{
    public string Platform { get; set; }
    public string System { get; set; }
    public string Subsystem { get; set; }
    public string Version { get; set; }
    public string Environment { get; set; }
}