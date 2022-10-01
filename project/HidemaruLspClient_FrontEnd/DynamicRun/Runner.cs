using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd.DynamicRun
{
    internal class Runner
    {
        public static void Execute(byte[] compiledAssembly,string typeName,Action<object>onInstanceCreated)
        {
            var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly,typeName,onInstanceCreated);

            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] compiledAssembly, string typeName,Action<object> onInstanceCreated)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                var assembly = assemblyLoadContext.LoadFromStream(asm);
                var classType = assembly.GetType(typeName);
                //var invokeResult = (string)classType.GetMethod("GetServerName").Invoke(null, null);

                //var instance = Activator.CreateInstance(classType);


                var instance = Activator.CreateInstance(classType);
                onInstanceCreated(instance);

                var t = instance.GetType();
                var mi = t.GetMethod("GetServerName");
                var s = mi.Invoke(instance, null);
                Console.WriteLine($"GetServerName()={s.ToString()}");

                /*
                var entry = assembly.EntryPoint;

                _ = entry != null && entry.GetParameters().Length > 0
                    ? entry.Invoke(null, new object[] {args})
                    : entry.Invoke(null, null);
                */
                assemblyLoadContext.Unload();

                return new WeakReference(assemblyLoadContext);
            }
        }
    }
}
