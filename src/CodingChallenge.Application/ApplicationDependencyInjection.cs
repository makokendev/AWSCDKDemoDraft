using System;
using System.Reflection;
using CodingChallenge.Application.Behaviours;
using CodingChallenge.Application.NFT.Commands.Mint;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CodingChallenge.Application;
public static class ApplicationDependencyInjection
{

    public static IServiceCollection AddApplicationBaseDependencies(this IServiceCollection services)
    {
        // var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        // foreach (var assembly in allAssemblies)
        // {
        //     Console.WriteLine($"assembly name is {assembly.FullName}");
        // }
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        //services.AddMediatR(Assembly.GetAssembly(typeof(MintCommand)));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(MintCommand)));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        return services;
    }
}

