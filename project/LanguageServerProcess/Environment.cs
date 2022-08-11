using System;
using System.IO;
using System.Linq;

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
        /// RootUriをアドホックに見付ける。
        /// 
        ///（数値が小さいほど優先順位が高い）
        /// 1. Repository
        /// 2. 編集中ファイルのフォルダ
        /// </summary>
        /// <returns>URI形式のパス名</returns>
        public static string FindRootUriAsAdhoc()
        {
            var rootUri = new Uri(FindRootDirectoryAsAdhoc());
            return rootUri.AbsoluteUri;
        }
        /// <summary>
        /// RootDirectoryをアドホックに見付ける。
        /// </summary>
        /// <returns>絶対パス形式</returns>
        public static string FindRootDirectoryAsAdhoc()
        {
            var repo = FindRepositoryDirectoryName();
            if (repo != "")
            {
                return repo;
            }
            return GetCurrentWorkingDirectoryName();
        }
        /// <summary>
        /// VisualStudioのソリューションファイル(.sln)名を探す
        /// </summary>
        /// <returns>ソリューションファイル(.sln)への絶対パス、または、空文字</returns>
        public static string FindVisualStudioSolutionFileName()
        {
            string solutionFileName = "";
            Func<string, bool> findSolutionFileName = (dir) =>
            {
                foreach (var file in Directory.EnumerateFiles(dir, "*.sln"))
                {
                    solutionFileName = file;
                    return false;
                }
                return true;
            };
            EnumerationParentDirectory(currentWorkingDirectory_, findSolutionFileName);
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

        static void EnumerationParentDirectory(string currentDirectory, Func<string, bool> func)
        {
            EnumerationParentDirectoryMain(currentDirectory, func);
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
