namespace ClientExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Runner(new CSharpClient());
            //Runner(new PythonClient());
            //Runner(new LuaClient());
            //Runner(new CppClient());
            //Runner(new VimScriptClient());
        }

        private static void Runner(ExampleBase example)
        {
            example.Start();
        }
    }
}