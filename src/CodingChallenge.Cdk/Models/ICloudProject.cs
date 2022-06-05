namespace CodingChallenge.Cdk;

public interface ICloudProject
{
    string Environment { get; set; }
    string Platform { get; set; }
    string System { get; set; }
    string Subsystem { get; set; }
}
