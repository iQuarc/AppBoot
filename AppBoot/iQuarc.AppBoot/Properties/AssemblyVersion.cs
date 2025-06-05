//		We Adopt SemVer both for NuGet packages versions and also for Assembly Version  (http://semver.org/ | https://docs.nuget.org/create/versioning). 
//		We prefer nuspec to take the package version from assembly version.
using System.Reflection;

[assembly: AssemblyVersion(Version.Assembly)]
[assembly: AssemblyFileVersion(Version.Assembly)]
[assembly: AssemblyInformationalVersion(Version.NugetReleasePackage)]

static class Version
{
	/// <summary>
	///     Breaking changes.
	/// </summary>
	private const string Major = "2";

	/// <summary>
	///     New features, but backwards compatible.
	/// </summary>
	private const string Minor = "1";

	/// <summary>
	///     Backwards compatible bug fixes only.
	/// </summary>
	private const string Patch = "1";

	/// <summary>
	///     Build number. Prefix with 0 for NuGet version ranges
	/// </summary>
	private const string Build = "000";

	/// <summary>
	///     NuGet Pre-Release package versions
	/// </summary>
	private const string Prerelease = "test";

	/// <summary>
	///     Used to set the assembly version
	/// </summary>
	public const string Assembly = Major + "." + Minor + "." + Patch + "." + Build;

	/// <summary>
	///     Used to set the version of a release NuGet Package
	/// </summary>
	public const string NugetReleasePackage = Major + "." + Minor + "." + Patch;

	/// <summary>
	///     Used to set the version of a pre-release NuGet Package
	/// </summary>
	public const string NugetPrereleasePackage = Major + "." + Minor + "." + Patch + "-" + Prerelease + Build;
}