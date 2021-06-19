using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServerProcess
{
    /// <summary>
    /// 環境の情報
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// クラスの初期化
        /// </summary>
        /// <param name="currentWorkingFolder"></param>
        static public void Initialize(string currentWorkingFolder)
        {
            currentWorkingDirectory_ = currentWorkingFolder;
        }
        /// <summary>
        /// 秀丸エディタのプロセスIDを取得する
        /// </summary>
        /// <returns></returns>
        public static int GetHostProcessId()
        {
            return System.Diagnostics.Process.GetCurrentProcess().Id;
        }
        /// <summary>
        /// 現在の作業ディレクトリを取得する
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentWorkingDirectoryName()
        {
            return currentWorkingDirectory_;
        }
        /// <summary>
        /// VisualStudioのソリューションファイル(.sln)名を探す
        /// </summary>
        /// <returns>ソリューションファイル(.sln)への絶対パス、または、空文字</returns>
        public static string FindVisualStudioSolutionFileName()
        {
            string solutionFileName="";
            Func<string,bool> findSolutionFileName = (dir)=>
            {
                foreach(var file in Directory.EnumerateFiles(dir, "*.sln"))
                {
                    solutionFileName = file;
                    return false;
                }
                return true;
            };
            EnumerationParentDirectory(currentWorkingDirectory_,findSolutionFileName);
            return solutionFileName;
        }
        /// <summary>
        /// リポジトリのディレクトリを探す
        /// </summary>
        /// <returns>リポジトリの絶対パス、または、空文字</returns>
        public static string FindRepositoryDirectoryName()
        {
            string repositoryName = "";
            Func<string, bool> findSolutionFileName = (currentDir) =>
            {
                foreach (var dir in Directory.EnumerateDirectories(currentDir))
                {
                    var firstItem = rootMarker.FirstOrDefault((maker) => dir.EndsWith(maker));
                    if (firstItem != null)
                    {
                        repositoryName = currentDir;
                        return false;
                    }
                }
                return true;
            };
            EnumerationParentDirectory(currentWorkingDirectory_, findSolutionFileName);
            return repositoryName;
        }



        /*******************************************************************/
        /* Private */
        /*******************************************************************/
        static string currentWorkingDirectory_ = null;
        static readonly string[] rootMarker = new string[] {
                   @"\.git",
                   @"\.svn",
                   @"\.hg",
                   @"\.bzr",
                   @"\_darcs"
            };

        static void EnumerationParentDirectory(string currentDirectory, Func<string,bool>func)
        {
            EnumerationParentDirectoryMain(currentDirectory,func);
        }
        static bool EnumerationParentDirectoryMain(string currentDirectory, Func<string, bool> func)
        {
            if (!func(currentDirectory))
            {
                return false;
            }

            DirectoryInfo parent;
            try
            {
                parent = Directory.GetParent(currentDirectory);
            }
            catch (Exception)
            {
                return false;
            }
            if (parent == null)
            {
                return false;
            }
            return EnumerationParentDirectoryMain(parent.FullName, func);
        }
    }
}
