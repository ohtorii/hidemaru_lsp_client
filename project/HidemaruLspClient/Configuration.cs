using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class Configuration
    {
        public class Option
        {
            public string Excutable { get; set; }
            public string Arguments { get; set; }
            public string RootUri { get; set; }
            public string WorkspaceConfig { get; set; }
        }

        class Method
        {
            public string name { get; set; }
            public Action<Option, object> action { get; set; }
        }
        static readonly Method[] Methods = new Method[]{
            new Method { name="GetExcutable",       action=(Option dst,object src)=>dst.Excutable       =src.ToString()  },
            new Method { name="GetArguments",       action=(Option dst,object src)=>dst.Arguments       =src.ToString()  },
            new Method { name="GetRootUri",         action=(Option dst,object src)=>dst.RootUri         =src.ToString()  },
            new Method { name="GetWorkspaceConfig", action=(Option dst,object src)=>dst.WorkspaceConfig =src.ToString()  },
        }; 
        static readonly string[] ReferencedAssemblies = new[]
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
            "System.Data.dll",
            "System.Data.DataSetExtensions.dll",
            "System.Net.Http.dll",
            "System.Xml.dll",
            "System.Xml.Linq.dll",
        };

        /// <summary>
        /// 構成ファイルを評価する
        /// </summary>
        /// <param name="filename"></param>
        public static Option Eval(string filename)
        {
            var result = new Option();
            {
                var codeDom = CodeDomProvider.CreateProvider("CSharp" /*, new Dictionary<string, string>() { { "TargetFrameworkVersion", "v4.8" } }*/);
                var compileParameters = new CompilerParameters
                {
                    CompilerOptions = "/target:library",
                    GenerateInMemory = true,
                };
                compileParameters.ReferencedAssemblies.AddRange(ReferencedAssemblies);

                var code = File.ReadAllText(filename);
                var cr = codeDom.CompileAssemblyFromSource(compileParameters, code);
                if (cr.Errors.Count > 0)
                {
                    //Todo: ログを用意する
                    Console.WriteLine("ERROR: " + cr.Errors[0] + "\nError evaluating cs code");
                    return null;
                }
                var a = cr.CompiledAssembly;
                var instance = a.CreateInstance("HidemaruLsp.ServerConfiguration");
                var t = instance.GetType();
                foreach (var method in Methods)
                {
                    var mi = t.GetMethod(method.name);
                    var s = mi.Invoke(instance, null);
                    method.action(result, s);
                }
            }
            return result;
        }        
        
        
    }
}
