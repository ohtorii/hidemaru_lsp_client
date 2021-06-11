using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerProcess
{
    public abstract class Configuration
    {
        public abstract string GetExcutablePath();
        public virtual string GetArguments() { return ""; }
        public virtual string GetRootUri() { return ""; }
        public virtual string GetWorkspaceConfig() { return ""; }
    }
}
