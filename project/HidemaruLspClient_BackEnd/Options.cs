﻿using System;
using System.IO;
using System.Text;
using CommandLine;


namespace HidemaruLspClient
{
    internal class Options
    {
        internal static Options Create(bool isConsoleApplication, string [] args) {
            Options result=null;
            {
                Parser parser = null;
                StringBuilder helpTextBuilder=null;
                StringWriter helpTextWriter = null;
                if (isConsoleApplication)
                {
                    parser = Parser.Default;
                }
                else
                {
                    helpTextBuilder = new StringBuilder();
                    helpTextWriter  = new StringWriter(helpTextBuilder);
                    parser          = new Parser(config => config.HelpWriter = helpTextWriter);
                }
                parser.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        result = o;
                    });
                if (isConsoleApplication)
                {
                    //pass
                }
                else
                {
                    if (result == null)
                    {
                        Win32Native.MessageBox(IntPtr.Zero, helpTextBuilder.ToString(), "Help", 0);
                    }
                }
            }
            return result;
        }


        internal enum RegistryMode{
            RegServer,
            UnRegServer,
            RegServerPerUser,
            UnRegServerPerUser,
            Unknown,
        }

        [Option('m', "mode", HelpText = "RegServer, UnRegServer, RegServerPerUser, UnRegServerPerUser", Default = RegistryMode.Unknown)]
        public RegistryMode Mode { get; set; }

        //[CommandLine.Option('s',"show", HelpText ="Show window.")]
        //public bool Show { get; set; }
    }

}

