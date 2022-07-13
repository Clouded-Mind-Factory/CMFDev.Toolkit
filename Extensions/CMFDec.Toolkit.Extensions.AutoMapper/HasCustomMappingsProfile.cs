using AutoMapper;
using CMFDec.Toolkit.AutoMapper.Attributes;
using System.Reflection;

namespace CMFDec.Toolkit.AutoMapper
{
    public class HasCustomMappingsProfile : Profile
    {
        public HasCustomMappingsProfile()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _CreateCustomMappings(assemblies);

        }
        private void _CreateCustomMappings(params Assembly[] assemblies)
        {
            var customMappingTypes = assemblies.SelectMany(x => x.GetTypes().Where(t => t.GetCustomAttribute<HasCustomMappingAttribute>() is not null)).ToList();
            foreach (var type in customMappingTypes)
            {
                var createMapMethod = type.GetMethod("CreateMap", BindingFlags.Public | BindingFlags.Static);
                if (createMapMethod is null
                    || createMapMethod.GetParameters() is not ParameterInfo[] pi
                    || pi.Length != 1
                    || pi[0].ParameterType != typeof(Profile))
                {
                    throw new InvalidOperationException($"Tried to create maps for type: {type.Name} - Types with the {nameof(HasCustomMappingAttribute)} must declare a public static \"CreateMap\" Method, with a single \"AutoMapper.Profile\" parameter");
                }
                createMapMethod.Invoke(null, new[] { this });
            }
        }
    }
}
