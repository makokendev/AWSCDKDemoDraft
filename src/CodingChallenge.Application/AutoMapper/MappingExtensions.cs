using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodingChallenge.Application.AutoMapper;
public static class MappingExtensions
{

    public static List<TDestination> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration)
        => queryable.ProjectTo<TDestination>(configuration).ToList();

    public static void ApplyMappingsFromAssembly<T>(this Assembly assembly, T profile)
    where T : Profile
    {
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod("Mapping")
                ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

            methodInfo?.Invoke(instance, new object[] { profile });

        }
    }
}

