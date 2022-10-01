using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HidemaruLspClient_BackEndContract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;


namespace HidemaruLspClient_FrontEnd.DynamicRun
{
    internal class Compiler
    {
        public static byte[] CompillingString(string sourceCode, ILspClientLogger logger=null)
        {
            using (var peStream = new MemoryStream())
            {
                var result = GenerateCode(sourceCode,logger).Emit(peStream);
                if (!result.Success)
                {
                    logger?.Error("Compilation done with error.");
                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
                    foreach (var diagnostic in failures)
                    {
                        logger?.Error(String.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage()));
                    }
                    return null;
                }
                //Console.WriteLine("Compilation done without any error.");
                peStream.Seek(0, SeekOrigin.Begin);
                return peStream.ToArray();
            }
        }
        public static byte[] CompilingFile(string filepath, ILspClientLogger logger = null)
        {
            logger?.Info(String.Format($"Starting compilation of: '{filepath}'"));
            var sourceCode = File.ReadAllText(filepath);
            return CompillingString(sourceCode,logger);
        }

        private static CSharpCompilation GenerateCode(string sourceCode, ILspClientLogger logger)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };

            /*
            var items = Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList();
            if (items != null)
            {
                logger?.Debug(String.Format($"Assembly names. num={items.Count}"));
                foreach (var item in items)
                {
                    logger?.Debug(item.FullName);
                    references.Add(MetadataReference.CreateFromFile(Assembly.Load(item).Location));
                }
            }*/

            /*
            string[] ReferencedAssemblies = new[]
            {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll",
                "System.Data.dll",
                "System.Data.DataSetExtensions.dll",
                "System.Xml.dll",
                "System.Xml.Linq.dll",
            };
            foreach(var item in ReferencedAssemblies)
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load(item).Location));
            }*/
            return CSharpCompilation.Create("Hello.dll",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
