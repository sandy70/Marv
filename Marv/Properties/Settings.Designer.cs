﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18213
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marv.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1980")]
        public int StartYear {
            get {
                return ((int)(this["StartYear"]));
            }
            set {
                this["StartYear"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2010")]
        public int EndYear {
            get {
                return ((int)(this["EndYear"]));
            }
            set {
                this["EndYear"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public double FitToPad {
            get {
                return ((double)(this["FitToPad"]));
            }
            set {
                this["FitToPad"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Data\\Koc\\Tally.xlsx")]
        public string TallyFileName {
            get {
                return ((string)(this["TallyFileName"]));
            }
            set {
                this["TallyFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("D:\\Data\\BP\\BpData.xlsx")]
        public string ProfileFileName {
            get {
                return ((string)(this["ProfileFileName"]));
            }
            set {
                this["ProfileFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsSensorButtonVisible {
            get {
                return ((bool)(this["IsSensorButtonVisible"]));
            }
            set {
                this["IsSensorButtonVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\")]
        public string CacheDirectory {
            get {
                return ((string)(this["CacheDirectory"]));
            }
            set {
                this["CacheDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsMapVisible {
            get {
                return ((bool)(this["IsMapVisible"]));
            }
            set {
                this["IsMapVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsYearSliderVisible {
            get {
                return ((bool)(this["IsYearSliderVisible"]));
            }
            set {
                this["IsYearSliderVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsMenuVisible {
            get {
                return ((bool)(this["IsMenuVisible"]));
            }
            set {
                this["IsMenuVisible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\r\n          <ArrayOfString xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n            <string>Resources\\Net" +
            "works\\Blowout - with groups.net</string>\r\n          </ArrayOfString>\r\n        ")]
        public global::LibPipeline.SelectableStringCollection NetworkFileNames {
            get {
                return ((global::LibPipeline.SelectableStringCollection)(this["NetworkFileNames"]));
            }
            set {
                this["NetworkFileNames"] = value;
            }
        }
    }
}
