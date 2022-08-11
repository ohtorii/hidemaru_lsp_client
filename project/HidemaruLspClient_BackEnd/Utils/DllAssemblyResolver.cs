using System;
using System.IO;
using System.Reflection;

namespace HidemaruLspClient.Utils
{
    internal class DllAssemblyResolver
    {
        public DllAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        ~DllAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        // アセンブリdllの読み込みに失敗した時、このメソッドが実行される。
        // ようするに「dllがみつからなかったので、この場所のこのファイルを探してください」といった形で返すメソッドである。
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var requestingAssembly = args.RequestingAssembly;
                var requestedAssembly = new AssemblyName(args.Name);
                //System.Diagnostics.Trace.WriteLine($"CurrentDomain_AssemblyResolve:{args.Name}"); // デバッグモニター表示用

                // このdll自体を置いているフォルダに読み込み対象のアセンブリがあるかもしれない。
                string self_full_path = Assembly.GetExecutingAssembly().Location;
                string self_dir = Path.GetDirectoryName(self_full_path);

                // このフルパスを整形することで、違うフォルダ、あるいはサブフォルダに配置してあるdllをアセンブリとして読み込ませることが出来る。
                var targetfullpath = self_dir + $@"\{requestedAssembly.Name}.dll";

                if (File.Exists(targetfullpath))
                {
                    return Assembly.LoadFile(targetfullpath);
                }

                // そのようなフルパスが指定されている場合(フルパスを指定した書き方)
                targetfullpath = requestedAssembly.Name;
                if (File.Exists(targetfullpath))
                {
                    return Assembly.LoadFile(targetfullpath);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}
