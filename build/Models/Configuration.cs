using System.Collections.Generic;
using System.ComponentModel;
using AutoMapper;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.IO;
using Cake.Yaml;
using CodingChallenge.Cdk;
using CodingChallenge.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CodingChallenge.CakeBuild.Models;
public class Settings
{
    public Settings(ICakeContext cakeContext)
    {
        InitVariables();
        SetMetadataProperties(cakeContext);
        SetAwsAppProject(cakeContext);
        StandardFolders = new StandardFolderSettings(cakeContext);
        DotnetSettings = new DotnetSettings(cakeContext);
    }

    private void InitVariables()
    {
        ProjectSettingsList = new List<ProjectSettings>();
    }

    public StandardFolderSettings StandardFolders { get; private set; }
    public DotnetSettings DotnetSettings { get; private set; }
    public List<ProjectSettings> ProjectSettingsList { get; private set; }

    public MetaData metadata { get; private set; }

    public AWSAppProject AwsApplication;

    private void SetMetadataProperties(ICakeContext cakeContext)
    {
        metadata = cakeContext.DeserializeYamlFromFile<MetaData>(new FilePath("../metadata.yaml"));
    }
    private void PrintProperties(ICakeContext cakeContext,object obj)
    {
        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
        {
            string name = descriptor.Name;
            object value = descriptor.GetValue(obj);
            cakeContext.Information($"{obj.GetType().Name} - Property Name {name} - Value: {value}");
        }
    }
    private void SetAwsAppProject(ICakeContext cakeContext)
    {
        AwsApplication = new AWSAppProject();
        var configuration = new ConfigurationBuilder()
           .AddEnvironmentVariables().Build();
        configuration.GetSection(Constants.APPLICATION_ENVIRONMENT_VAR_PREFIX).Bind(AwsApplication);

        var autoMapperConfig = new MapperConfiguration(
            cfg => cfg.CreateMap<MetaData, AWSAppProject>().
                ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => !string.IsNullOrWhiteSpace((string)sourceMember)))
            );
        var autoMapper = autoMapperConfig.CreateMapper();
        autoMapper.Map(metadata, AwsApplication);
    }
}

