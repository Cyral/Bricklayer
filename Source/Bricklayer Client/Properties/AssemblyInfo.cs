using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Bricklayer Client")]
[assembly: AssemblyProduct("Bricklayer")]
[assembly: AssemblyDescription("An Open Source, Fully Moddable and Customizable 2D Building Game inspired by Everybody Edits")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("Copyright © Cyral 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("b6f80ac6-e328-404f-ba87-73a1706ef8ec")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("0.0.2.1")]
[assembly: AssemblyVersionName("Alpha","a")]

#region Custom Attributes
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyVersionName : Attribute {
    public const string Prefix = "v";
    public readonly string Name, ShortName;
    public AssemblyVersionName(string name, string shortName) { Name = name; ShortName = shortName; }
    public static string GetVersion()
    {
        Version version = Assembly.GetEntryAssembly().GetName().Version;
        AssemblyVersionName name = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyVersionName), false).Cast<AssemblyVersionName>().ToList<AssemblyVersionName>()[0];
        return AssemblyVersionName.Prefix + version + name.ShortName;
    }
}
#endregion
