using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("iQuarc.AppBoot")]
[assembly: AssemblyDescription("Basic functionality for hiding the DI Container and for defining a modular application.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("iQuarc")]
[assembly: AssemblyProduct("iQuarc.AppBoot")]
[assembly: AssemblyCopyright("Copyright © iQuarc 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5df84816-eaa2-460c-8068-96a2b1a066ee")]

// This assembly code is intended to be used from other languages than C#
[assembly: CLSCompliant(true)]

// Make the Unit Testing assembly friendly
[assembly: InternalsVisibleTo("iQuarc.AppBoot.UnitTests")]


// Versioning:
//		Adopting SemVer both for NuGet packages versions and also for Assembly Version  (http://semver.org/ | https://docs.nuget.org/create/versioning). 
//		We prefer nuspec to take the package version from assembly version.
[assembly: AssemblyVersion(Version.Assembly)]
[assembly: AssemblyFileVersion(Version.Assembly)]
[assembly: AssemblyInformationalVersion(Version.NugetReleasePackage)]

static class Version
{
	/// <summary>
	///     Breaking changes.
	/// </summary>
	private const string Major = "1";

	/// <summary>
	///     New features, but backwards compatible.
	/// </summary>
	private const string Minor = "1";

	/// <summary>
	///     Backwards compatible bug fixes only.
	/// </summary>
	private const string Patch = "0";

	/// <summary>
	///     Build number. Prefix with 0 for NuGet version ranges
	/// </summary>
	private const string Build = "000";

	/// <summary>
	///     NuGet Pre-Release package versions
	/// </summary>
	private const string Prerelease = "test";

	/// <summary>
	/// Used to set the assembly version
	/// </summary>
	public const string Assembly = Major + "." + Minor + "." + Patch + "." + Build;

	/// <summary>
	/// Used to set the version of a release NuGet Package
	/// </summary>
	public const string NugetReleasePackage = Major + "." + Minor + "." + Patch;

	/// <summary>
	/// Used to set the version of a pre-release NuGet Package
	/// </summary>
	public const string NugetPrereleasePackage = Major + "." + Minor + "." + Patch + "-" + Prerelease+Build;
}