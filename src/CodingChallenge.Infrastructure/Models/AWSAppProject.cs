using System;
using CodingChallenge.Application;

namespace CodingChallenge.Infrastructure;

public class AWSAppProject : IInfrastructureProject
{
    public string Platform { get; set; }
    public string System { get; set; }
    public string Subsystem { get; set; }
    public string Version { get; set; }
    public string Environment { get; set; }
    public string AwsRegion { get; set; }
    public string CertificateArn { get; set; }
    public string DomainName { get; set; }
}