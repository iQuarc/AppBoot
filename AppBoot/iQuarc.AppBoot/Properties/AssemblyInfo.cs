﻿using System;
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


// Version attributes are in a separate source file: AssemblyVersion.cs