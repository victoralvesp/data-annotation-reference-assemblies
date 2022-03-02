﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAnnotations.Reference.Assemblies
{
    public static class ReferenceAssemblies
    {
        public static IEnumerable<PortableExecutableReference> NetCoreApp31 => DataAnnotations.Reference.Assemblies.NetCoreApp31.All;
        public static IEnumerable<PortableExecutableReference> Net50 => DataAnnotations.Reference.Assemblies.Net50.All;
        public static IEnumerable<PortableExecutableReference> Net60 => DataAnnotations.Reference.Assemblies.Net60.All;
        public static IEnumerable<PortableExecutableReference> NetStandard13 => DataAnnotations.Reference.Assemblies.NetStandard13.All;
        public static IEnumerable<PortableExecutableReference> NetStandard20 => DataAnnotations.Reference.Assemblies.NetStandard20.All;
        public static IEnumerable<PortableExecutableReference> Net461 => DataAnnotations.Reference.Assemblies.Net461.All;
        public static IEnumerable<PortableExecutableReference> Net472 => DataAnnotations.Reference.Assemblies.Net472.All;

        public static IEnumerable<PortableExecutableReference> Get(ReferenceAssemblyKind kind) => kind switch
        {
            ReferenceAssemblyKind.NetCoreApp31 => NetCoreApp31,
            ReferenceAssemblyKind.Net50 => Net50,
            ReferenceAssemblyKind.Net60 => Net60,
            ReferenceAssemblyKind.NetStandard13 => NetStandard13,
            ReferenceAssemblyKind.NetStandard20 => NetStandard20,
            ReferenceAssemblyKind.Net461 => Net461,
            ReferenceAssemblyKind.Net472 => Net472,
            _ => throw new Exception($"Invalid kind: {kind}")
        };
    }

    public enum ReferenceAssemblyKind
    {
        NetCoreApp31,
        Net50,
        Net60,
        NetStandard20,
        NetStandard13,
        Net461,
        Net472,
    }

    public static class Extensions
    {
        public static T WithDataAnnotationAssemblies<T>(this T compilation, ReferenceAssemblyKind kind)
            where T : Compilation
        {
            var references = ReferenceAssemblies.Get(kind);
            return (T)(compilation.WithReferences(references));
        }
    }
}
