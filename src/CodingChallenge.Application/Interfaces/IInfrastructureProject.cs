namespace CodingChallenge.Application;

public interface IInfrastructureProject
{
    string Environment { get; set; }
    string Platform { get; set; }
    string System { get; set; }
    string Subsystem { get; set; }

    string Version { get; set; }
}
