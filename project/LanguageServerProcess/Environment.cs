using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerProcess
{
    public class Environment
    {
        /// <summary>
        /// 秀丸エディタのプロセスIDを取得する
        /// </summary>
        /// <returns></returns>
        public static int GetHostProcessId()
        {
            return System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        public static string GetCurrentWorkingFolder()
        {
            return "";
        }
        /// <summary>
        /// VisualStudioのソリューションファイル(.sln)名を探す
        /// </summary>
        /// <returns></returns>
        public static string FindVisualStudioSolutionFileName()
        {
            return "";
        }
        /// <summary>
        /// リポジトリのフォルダを探す
        /// </summary>
        /// <returns></returns>
        public static string FindRepositoryFolderName()
        {
            return "";
        }
    }
}
