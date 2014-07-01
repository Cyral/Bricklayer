using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Linq;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Bricklayer Common")]
[assembly: AssemblyDescription("Library used by the Bricklayer Client and Server")]
[assembly: AssemblyProduct("Bricklayer")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("https://github.com/Cyral/Bricklayer/blob/master/LICENSE.txt")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3c1e3a16-84a3-4a3d-8e36-32ce5383836c")]

[assembly: AssemblyVersion("0.0.0.1")]
[assembly: AssemblyFileVersion("0.0.0.1")]

#region Custom Attributes
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyVersionName : Attribute
{
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

