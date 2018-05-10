﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SqlBinder.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Exceptions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Exceptions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SqlBinder.Properties.Exceptions", typeof(Exceptions).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Empty sql was returned..
        /// </summary>
        public static string EmptySqlReturned {
            get {
                return ResourceManager.GetString("EmptySqlReturned", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Illegal combination of value type and operator..
        /// </summary>
        public static string IllegalComboOfValueAndOperator {
            get {
                return ResourceManager.GetString("IllegalComboOfValueAndOperator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sql formatting for value of type &apos;{0}&apos; and operator &apos;{1}&apos; is not possible. {2}.
        /// </summary>
        public static string InvalidCondition {
            get {
                return ResourceManager.GetString("InvalidCondition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Following conditions had no matching parameters in the script: {0}..
        /// </summary>
        public static string NoMatchingParams {
            get {
                return ResourceManager.GetString("NoMatchingParams", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An exception has occurred in the SqlBinder template parser engine..
        /// </summary>
        public static string ParserFailure {
            get {
                return ResourceManager.GetString("ParserFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of sql parameter placeholders and number of values don&apos;t match..
        /// </summary>
        public static string PlaceholdersAndActualParamsDontMatch {
            get {
                return ResourceManager.GetString("PlaceholdersAndActualParamsDontMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your SqlBinder script is not valid: {0}.
        /// </summary>
        public static string ScriptNotValid {
            get {
                return ResourceManager.GetString("ScriptNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When both &apos;from&apos; and &apos;to&apos; values are specified, the output SQL will always be inclusive. To specify an exclusive range you would have to alter your script to contain two parameter placeholders rather than one..
        /// </summary>
        public static string SqlBetweenCanOnlyBeInclusive {
            get {
                return ResourceManager.GetString("SqlBetweenCanOnlyBeInclusive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is an unclosed scope (a {...} or [...] or a &apos;...&apos; string literal) in your script..
        /// </summary>
        public static string UnclosedScope {
            get {
                return ResourceManager.GetString("UnclosedScope", resourceCulture);
            }
        }
    }
}
