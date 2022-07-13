using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFDec.Toolkit.AutoMapper.Attributes
{
    /// <summary>
    /// Signals that a class has one or more AutoMapper mappings configured.
    /// THe class using this attributes must define a public static Method called "CreateMap" with with a single \"AutoMapper.Profile\" parameter.
    /// 
    /// Usage Info:
    /// 
    /// [HasCustomMapping]
    /// public class FooBar 
    /// {
    /// 
    ///     public static void CreateMap(Profile profile)
    ///     {
    ///         profile.CreateMap<..,..>();
    ///     }
    ///     
    /// }
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HasCustomMappingAttribute : Attribute
    {
    }
}
