using System;
using static Amazon.CDK.AWS.APIGateway.CfnRestApi;

namespace CodingChallenge.Cdk;
public class LamdaFunctionCdkSettings
    {
        public string FunctionNameSuffix { get; set; }
        public int Memory { get; set; }
        public double ReservedConcurrentExecutions { get; set; }
        public int Timeout { get; set; }

        public Type HandlerClassType { get; set; }
        public string HandlerFunctionName { get; set; }

        public string RoleArn { get; set; }

        public S3LocationProperty S3Location { get; set; }
        public string Runtime { get; set; }
        public string ImageUri { get; set; }
        public string[] Architectures { get; set; }
        public string Authorizer { get; set; }
    }
