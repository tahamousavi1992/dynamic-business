using DynamicBusiness.BPMS.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class RoslynCompiler
    {
        private static readonly IEnumerable<string> DefaultNamespaces =
             new[]
             {
                "System",
                "System.Linq",
                "System.Collections.Generic",
                "DynamicBusiness.BPMS.Domain",
                "DotNetNuke.Entities.Users",
                "DotNetNuke.Security.Membership",
                "DynamicBusiness.BPMS.EntityMethods",
                "DynamicBusiness.BPMS.CodePanel",
             };
        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        public static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }

        public static void compile(string code, List<string> assemblies, string outputAssembly)
        {
            List<MetadataReference> metadataReferences = new List<MetadataReference>();
            assemblies.ForEach((a) =>
            {
                metadataReferences.Add(MetadataReference.CreateFromFile(a));
            });
            var parsedSyntaxTree = Parse(code, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7));
            var compilation
                = CSharpCompilation.Create(System.IO.Path.GetFileName(outputAssembly), new SyntaxTree[] { parsedSyntaxTree }, metadataReferences, DefaultCompilationOptions);

            var result = compilation.Emit(outputAssembly);
            if (!result.Success)
            {
                System.IO.File.Delete(outputAssembly);
                throw new Exception(string.Join(" , ", result.Diagnostics.Where(c => c.DefaultSeverity.ToString() == "Error").Select(c => c.ToStringObj())));
            }

        }
    }
}
