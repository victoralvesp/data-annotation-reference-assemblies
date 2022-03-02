using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace DataAnnotations.Reference.Assemblies.UnitTests
{
    public class BaseSanityUT
    {
        internal static Basic.Reference.Assemblies.ReferenceAssemblyKind ConvertToBasicReferenceKind(ReferenceAssemblyKind kind) => kind switch
        {
            ReferenceAssemblyKind.NetCoreApp31 => Basic.Reference.Assemblies.ReferenceAssemblyKind.NetCoreApp31,
            ReferenceAssemblyKind.Net50 => Basic.Reference.Assemblies.ReferenceAssemblyKind.Net50,
            ReferenceAssemblyKind.Net60 => Basic.Reference.Assemblies.ReferenceAssemblyKind.Net60,
            ReferenceAssemblyKind.NetStandard20 => Basic.Reference.Assemblies.ReferenceAssemblyKind.NetStandard20,
            ReferenceAssemblyKind.NetStandard13 => Basic.Reference.Assemblies.ReferenceAssemblyKind.NetStandard13,
            ReferenceAssemblyKind.Net461 => Basic.Reference.Assemblies.ReferenceAssemblyKind.Net461,
            ReferenceAssemblyKind.Net472 => Basic.Reference.Assemblies.ReferenceAssemblyKind.Net472,
            _ => throw new NotImplementedException(),
        };
    }

    public class SanityUnitTests : BaseSanityUT
    {
        [Fact]
        public void AllCanCompile()
        {
            var source = @"
using System;

public class C
{
    public Exception Exception;

    static void Main() 
    {
        Console.WriteLine(""Hello World"");
    }
}";

            foreach (var kind in Enum.GetValues<ReferenceAssemblyKind>())
            {
                var compilation = CSharpCompilation.Create(
                    "Example",
                    new[] { CSharpSyntaxTree.ParseText(source) },
                    references: ReferenceAssemblies.Get(kind));
                compilation = compilation.WithReferenceAssemblies(ConvertToBasicReferenceKind(kind));
                // NetStandard1.3 comes with several no warn options we need here
                if (kind == ReferenceAssemblyKind.NetStandard13)
                {
                    compilation = NoWarn(compilation, "CS1702");
                    compilation = NoWarn(compilation, "CS1701");
                }

                Assert.Empty(compilation.GetDiagnostics());
                using var stream = new MemoryStream();
                var emitResult = compilation.Emit(stream);
                Assert.True(emitResult.Success);
                Assert.Empty(emitResult.Diagnostics);
            }
        }

        [Fact]
        public void AllCanCompile2()
        {
            var source = @"
using System;

public class C
{
    public Exception Exception;

    static void Main() 
    {
        Console.WriteLine(""Hello World"");
    }
}";

            foreach (var kind in Enum.GetValues<ReferenceAssemblyKind>())
            {
                var compilation = CSharpCompilation.Create(
                    "Example",
                    new[] { CSharpSyntaxTree.ParseText(source) },
                    references: Array.Empty<MetadataReference>());
                compilation = compilation.WithDataAnnotationAssemblies(kind)
                                    .WithReferenceAssemblies(ConvertToBasicReferenceKind(kind));

                // NetStandard1.3 comes with several no warn options we need here
                if (kind == ReferenceAssemblyKind.NetStandard13)
                {
                    compilation = NoWarn(compilation, "CS1702");
                    compilation = NoWarn(compilation, "CS1701");
                }

                Assert.Empty(compilation.GetDiagnostics());
                using var stream = new MemoryStream();
                var emitResult = compilation.Emit(stream);
                Assert.True(emitResult.Success);
                Assert.Empty(emitResult.Diagnostics);
            }
        }

        private static CSharpCompilation NoWarn(CSharpCompilation compilation, string id)
        {
            var diagnosticMap = compilation.Options.SpecificDiagnosticOptions;
            diagnosticMap = diagnosticMap.SetItem(id, ReportDiagnostic.Suppress);
            var options = compilation.Options.WithSpecificDiagnosticOptions(diagnosticMap);
            return compilation.WithOptions(options);
        }
    }
}
