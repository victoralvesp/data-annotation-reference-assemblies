// This is a generated file, please edit Generate.ps1 to change the contents

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DataAnnotations.Reference.Assemblies
{
    public static partial class Net60
    {
        public static class Resources
        {
            private static byte[]? _SystemComponentModelAnnotations;
            public static byte[] SystemComponentModelAnnotations => ResourceLoader.GetOrCreateResource(ref _SystemComponentModelAnnotations, "net60.System.ComponentModel.Annotations");
        }
    }
    public static partial class Net60
    {
        public readonly struct ReferenceInfo
        {
            public string FileName { get; }
            public byte[] ImageBytes { get; }
            public ReferenceInfo(string fileName, byte[] imageBytes)
            {
                FileName = fileName;
                ImageBytes = imageBytes;
            }
        }

        public static class References
        {
            public static ReferenceInfo SystemComponentModelAnnotations => new ReferenceInfo("System.ComponentModel.Annotations.dll", Resources.SystemComponentModelAnnotations);
            public static IEnumerable<ReferenceInfo> All { get; }= new []
            {
                SystemComponentModelAnnotations,
            };
        }
    }
    public static partial class Net60
    {
        public static PortableExecutableReference SystemComponentModelAnnotations { get; } = AssemblyMetadata.CreateFromImage(Resources.SystemComponentModelAnnotations).GetReference(filePath: "System.ComponentModel.Annotations.dll", display: "System.ComponentModel.Annotations (net60)");
        public static IEnumerable<PortableExecutableReference> All { get; }= new PortableExecutableReference[]
        {
            SystemComponentModelAnnotations,
        };
    }
}
