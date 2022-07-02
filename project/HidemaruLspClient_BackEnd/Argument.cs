using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using CommandLine;
using HidemaruLspClient.Native;

namespace HidemaruLspClient
{
    internal class Argument
    {
        internal static readonly Argument Default   = new Argument();
        internal static readonly Argument Embedding = new Argument { Mode=RegistryMode.Unknown, Start=true};

        /// <summary>
        /// コマンドライン引数からOptionsを生成する
        /// </summary>
        /// <param name="isConsoleApplication">コンソールアプリケーションかどうか</param>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>--helpの場合はnullを返す</returns>
        internal static Argument Create(bool isConsoleApplication, string [] args) {
            using (var parser = CreateParser(isConsoleApplication))
            {
                Argument result=null;
                parser.ParseArguments<Argument>(args)
                    .WithParsed<Argument>(o =>
                    {
                        result = o;
                    });
                if (isConsoleApplication)
                {
                    return result;
                }
                if (result != null)
                {
                    return result;
                }
                var writer = (StringWriter)parser.Settings.HelpWriter;
                User32.MessageBox(IntPtr.Zero, writer.GetStringBuilder().ToString(), "Help", 0);
                return null;
            }
        }

        static Parser CreateParser(bool isConsoleApplication)
        {
            if (isConsoleApplication)
            {
                return new Parser(config => { config.HelpWriter = Console.Out; });
            }
            return new Parser(config => { config.HelpWriter = new StringWriter(new StringBuilder()); });
        }

        internal enum RegistryMode{
            RegServer,
            UnRegServer,
            RegServerPerUser,
            UnRegServerPerUser,
            Unknown,
        }

        [Option('m', "mode", Required = false, HelpText = "RegServer, UnRegServer, RegServerPerUser, UnRegServerPerUser", Default = RegistryMode.Unknown)]
        public RegistryMode Mode { get; set; } = RegistryMode.Unknown;
        [Option('s',"start", Required = false, HelpText = "Start COM server via manualy.")]
        public bool Start { get; set; } = false;
        //[CommandLine.Option('s',"show", HelpText ="Show window.")]
        //public bool Show { get; set; }
    }

}

