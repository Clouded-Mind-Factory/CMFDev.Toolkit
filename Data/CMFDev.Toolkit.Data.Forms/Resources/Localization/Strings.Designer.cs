﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMFDev.Toolkit.Data.Forms.Resources.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CMFDev.Toolkit.Data.Forms.Resources.Localization.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid directory path.
        /// </summary>
        internal static string ValidationResult_Directory {
            get {
                return ResourceManager.GetString("ValidationResult_Directory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid email address.
        /// </summary>
        internal static string ValidationResult_Email {
            get {
                return ResourceManager.GetString("ValidationResult_Email", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid file name.
        /// </summary>
        internal static string ValidationResult_FileName {
            get {
                return ResourceManager.GetString("ValidationResult_FileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Does not match the pattern.
        /// </summary>
        internal static string ValidationResult_Match {
            get {
                return ResourceManager.GetString("ValidationResult_Match", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Is required.
        /// </summary>
        internal static string ValidationResult_Required {
            get {
                return ResourceManager.GetString("ValidationResult_Required", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid url.
        /// </summary>
        internal static string ValidationResult_Url {
            get {
                return ResourceManager.GetString("ValidationResult_Url", resourceCulture);
            }
        }
    }
}