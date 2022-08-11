//using NLog;
using HidemaruLspClient_BackEndContract;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HidemaruLspClient_FrontEnd
{
    class Configuration
    {
        static ILspClientLogger logger_ = null;

        public class Option
        {
            public string ServerName { get; set; }
            public string ExcutablePath { get; set; }
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
            new Method { name="GetServerName",      action=(Option dst,object src)=>dst.ServerName      =src.ToString()  },
            new Method { name="GetExcutablePath",   action=(Option dst,object src)=>dst.ExcutablePath   =src.ToString()  },
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

        public static void Initialize(ILspClientLogger logger)
        {
            logger_ = logger;
        }
        /// <summary>
        /// 構成ファイルを評価する
        /// </summary>
        /// <param name="serverConfigFilename"></param>
        public static Option Eval(string serverConfigFilename, string currentSourceCodeDirectory)
        {
            LanguageServerProcess.Environment.Initialize(currentSourceCodeDirectory);

            {
                var result = new Option();
                {
                    var codeDom = CodeDomProvider.CreateProvider("CSharp", new Dictionary<string, string>() { { "TargetFrameworkVersion", "v4.8" } });
                    var compileParameters = new CompilerParameters
                    {
                        CompilerOptions = MakeCompilerOptions(),
                        GenerateExecutable = false,
                        TreatWarningsAsErrors = false,
                        IncludeDebugInformation = false,
                        /*Memo:メモリに生成すると GetType()==null となる*/
                        GenerateInMemory = false,

                    };
                    logger_?.Debug(string.Format("CompilerOptions={0}", compileParameters.CompilerOptions));
                    compileParameters.ReferencedAssemblies.AddRange(ReferencedAssemblies);
                    if (Convert.ToBoolean(logger_?.IsDebugEnabled))
                    {
                        logger_?.Debug(string.Format("compileParameters.ReferencedAssemblies[]={0}", ReferencedAssemblies.ToList()));
                    }

                    logger_?.Info(string.Format("filename={0}", serverConfigFilename));

                    var code = File.ReadAllText(serverConfigFilename);
                    var cr = codeDom.CompileAssemblyFromSource(compileParameters, code);
                    if (0 < cr.Errors.Count)
                    {
                        foreach (var err in cr.Errors)
                        {
                            logger_?.Error(err.ToString());
                        }
                        return null;
                    }
                    var a = cr.CompiledAssembly;
                    var instance = a.CreateInstance("LanguageServerProcess.ServerConfiguration");
                    var t = instance.GetType();
                    foreach (var method in Methods)
                    {
                        var mi = t.GetMethod(method.name);
                        var s = mi.Invoke(instance, null);
                        method.action(result, s);
                        logger_?.Info(string.Format("{0}={1}", method.name, s));
                    }
                }
                return result;
            }
        }
        static string MakeCompilerOptions()
        {
            var sb = new StringBuilder();
            sb.Append("/target:library");
            //sb.Append(" /platform:x64");
            sb.AppendFormat(" /reference:{0}", DllPath("LanguageServerProcess.dll"));
            return sb.ToString();
        }
        static string DllPath(string dllName)
        {
            var self_full_path = Assembly.GetExecutingAssembly().Location;
            var self_dir = Path.GetDirectoryName(self_full_path);
            return Path.GetFullPath(Path.Combine(self_dir, dllName));
        }
    }
}
