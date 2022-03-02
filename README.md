# Easy in memory compilations for Roslyn

[Roslyn](https://github.com/dotnet/roslyn) is a powerful API for C# and Visual 
Basic compilations. Provide it code, a few options and a set of references and 
it will provide an API to inspect the syntax, semantic model, diagnostics and 
even generate DLLs / EXEs. 

Getting references for .NET Core or .NET Standard to use with a `Compilation`
is challenging because these are only shipped as physical files. In order to use
them in a library the developer must do the heavy lifting of packaging them up
as resources in their library and unpacking them at runtime.

The [DataAnnotations.Reference.Assemblies](https://www.nuget.org/packages/DataAnnotations.Reference.Assemblies/) 
library takes care of this heavy lifting and provides the reference assemblies 
for [System.ComponentModel.DataAnnotations](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-6.0) namespace of `netstandard2.0`, `netcoreapp3.1` and `net5.0`. These can be easily 
integrated into the existing Roslyn APIs.

This repository is largely based on [Basic Reference Assemblies](https://github.com/jaredpar/basic-reference-assemblies)

```c#
using Microsoft.CodeAnalysis.CSharp;
using Basic.Reference.Assemblies;
using System.IO;
using System.Reflection;

var code = @"
using System;
public static class Example
{
    public static void Main()
    {
        var tuple = (Part1: ""hello"", Part2: ""world"");
        Console.WriteLine($""{tuple.Part1} {tuple.Part2}"");
    }
}
";

var compilation = CSharpCompilation
    .Create(
        "HelloWorld.dll",
        new[] { CSharpSyntaxTree.ParseText(code) },
        references: DataAnnotationAssemblies.Net50);

using var stream = new MemoryStream();
var emitResults = compilation.Emit(stream);
stream.Position = 0;
var assembly = Assembly.Load(stream.ToArray());
var method = assembly.GetType("Example").GetMethod("Main");
method.Invoke(null, null); // Prints "Hello World"

```

This package also adds extensions methods for easily retargeting `Compilation` 
instances to different Target Frameworks.

```c#
CSharpCompilation compilation = ...;
compilation = compilation.WithDataAnnotationAssemblies(ReferenceAssemblyKind.NetCoreApp31);
```

This repository actually provides a series of packages. The expectation is that
most developers will use [DataAnnotations.Reference.Assemblies](https://www.nuget.org/packages/DataAnnotations.Reference.Assemblies/).
This package has reference assemblies for all of the supported target frameworks
and provides APIs to make it easy to switch between them in `Compilation` 
instances. 

# FAQ

## What if I only need a single target framework?
Developers who only need a single target framework and are extremely size 
conscious can grab the target framework specific package:

- [DataAnnotations.Reference.Assemblies.Net50](https://www.nuget.org/packages/DataAnnotations.Reference.Assemblies.Net50/)
- [DataAnnotations.Reference.Assemblies.NetCoreApp31](https://www.nuget.org/packages/DataAnnotations.Reference.Assemblies.NetCoreApp31/)
- [DataAnnotations.Reference.Assemblies.NetStandard20](https://www.nuget.org/packages/DataAnnotations.Reference.Assemblies.NetStandard20/)

## What is wrong with using typeof(Enumerable).Assembly?
Developers working on .NET Framework will often end up with the following pattern
for creating `Compilation` instances:

```c#
Assembly systemCoreAssembly = typeof(System.Linq.Enumerable).Assembly;
string systemCorePath = systemCoreAssembly.Location;
MetadataReference systemCoreRef = AssemblyMetadata.CreateFromFile(string path).GetReference();
```

This pattern will often work on .NET Framework but will fail with running on 
.NET Core. The reason for this is due to the differences in reference and 
implementation assemblies. Reference assemblies are designed for use at build 
time while implementation assemblies are used at runtime. A particular type for 
a given target framework doesn't necessarily live in the same reference and 
implementation assembly. Also the set of implementation assemblies can be larger
than the reference assemblies for the same target framework.

The reason the above tends to work on .NET Framework is the split between 
reference and implementation assemblies is not as pronounced. In most cases 
there is a 1:1 relationship. On .NET Core though the split is much more 
pronounced and it often requires probing for implementation assemblies to get a 
set which will work for building. This is a fragile process though, developers
are much better off using the reference assemblies as intended by the .NET team.
