﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KITT.Web.App.Resources.Pages {
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
    public class Index {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Index() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KITT.Web.App.Resources.Pages.Index", typeof(Index).Assembly);
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
        ///   Looks up a localized string similar to lives delivered.
        /// </summary>
        public static string LivesDeliveredLabel {
            get {
                return ResourceManager.GetString("LivesDeliveredLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to lives scheduled.
        /// </summary>
        public static string LivesScheduledLabel {
            get {
                return ResourceManager.GetString("LivesScheduledLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to proposals accepted.
        /// </summary>
        public static string ProposalsAcceptedLabel {
            get {
                return ResourceManager.GetString("ProposalsAcceptedLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to proposals.
        /// </summary>
        public static string ProposalsLabel {
            get {
                return ResourceManager.GetString("ProposalsLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View all the proposals.
        /// </summary>
        public static string ViewProposalsButtonText {
            get {
                return ResourceManager.GetString("ViewProposalsButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to View all my streamings.
        /// </summary>
        public static string ViewStreamingsButtonText {
            get {
                return ResourceManager.GetString("ViewStreamingsButtonText", resourceCulture);
            }
        }
    }
}