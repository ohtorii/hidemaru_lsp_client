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
        public bool Eval(string filename)
        {
            Initialize();

            return true;

            {//環境変数を
                /*
                    %VISUAL_STUDIO_SOLUTION_FILENAME%
                    %HOST_PROCESS_ID%
                    %ROOT_URI%
                 */
            }
            var codeDom = CodeDomProvider.CreateProvider("CSharp" /*, new Dictionary<string, string>() { { "TargetFrameworkVersion", "v4.8" } }*/);
            var compileParameters = new CompilerParameters();
            compileParameters.CompilerOptions = "/target:library";
            compileParameters.GenerateInMemory = true;
            compileParameters.ReferencedAssemblies.AddRange(ReferencedAssemblies);
            var code = File.ReadAllText(filename);
            var cr = codeDom.CompileAssemblyFromSource(compileParameters, code);
            if (cr.Errors.Count > 0)
            {
                errorLog_ = cr.Errors[0].ToString();
                //Console.WriteLine("ERROR: " + cr.Errors[0] + "\nError evaluating cs code");
                return false;
            }
            System.Reflection.Assembly a = cr.CompiledAssembly;
            var instance = a.CreateInstance("HidemaruLsp.ServerConfiguration");
            var t = instance.GetType();
            var mi = t.GetMethod("GetExcutable");
            var s = mi.Invoke(instance, null);
            
        }

        bool success_;
        string errorLog_;
        Option Options_;

        void Initialize()
        {
            success_ = false;
            Options_ = null;
            errorLog_ = "";
        }
        public bool Success { get { return success_; } }        
        public Option Options { get { return Options_; } }
    }
}
