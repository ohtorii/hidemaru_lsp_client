using System.Collections.Generic;
using System.IO;

namespace HidemaruLspClient_FrontEnd.Utils
{
    /// <summary>
    /// 複数のテキストファイルから行を集める
    /// </summary>
    internal class TextLines
    {
        internal class FileItem
        {
            public FileItem(in string AbsFilename)
            {
                this.AbsFilename = AbsFilename;
                this.LineItems = new List<LineItem>();
            }
            public string AbsFilename { get; set; }
            public List<LineItem> LineItems { get; set; }
        }
        internal class LineItem
        {
            public LineItem(long Line, object UserData)
            {
                this.Line = Line;
                this.UserData = UserData;
            }
            /// <summary>
            /// ファイルの行番号
            /// </summary>
            public long Line { get; }
            /// <summary>
            /// ユーザーデータ
            /// </summary>
            public object UserData { get; }
            /// <summary>
            /// Lineに対応する一行テキスト
            /// </summary>
            public string Text { get; set; }
        }
        internal class Option
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="AbsFilename"></param>
            /// <param name="line"></param>
            /// <param name="UserData">lineに紐付いたユーザーデータ</param>
            public void Add(in string AbsFilename, in long line, in object UserData)
            {
                string normalizedPath;
                string key;
                MakeKeyAndPath(out key, out normalizedPath, AbsFilename);

                List<LineItem> dst;
                {
                    FileItem value;
                    if (keyValuePairs.TryGetValue(key, out value))
                    {
                        dst = value.LineItems;
                    }
                    else
                    {
                        var item = new FileItem(normalizedPath);
                        keyValuePairs.Add(key, item);
                        dst = item.LineItems;
                    }
                }
                dst.Add(new LineItem(line, UserData));
            }
            static void MakeKeyAndPath(out string Key, out string NormalizedPath, in string AbsFilename)
            {
                NormalizedPath = System.IO.Path.GetFullPath(AbsFilename);
                Key = NormalizedPath.ToLower();
            }
            public void Clear()
            {
                keyValuePairs.Clear();
            }

            public Dictionary<string, FileItem> keyValuePairs = new Dictionary<string, FileItem>();
        }
        /// <summary>
        /// 複数ファイルから指定された行を取り出す
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        /// Todo: 秀丸エディタで編集中のファイルを検索対象に加える。
        public static SortedDictionary<string, List<LineItem>> Gather(in Option option)
        {
            var result = new SortedDictionary<string, List<LineItem>>();
            foreach (KeyValuePair<string, FileItem> KeyValue in option.keyValuePairs)
            {
                result[KeyValue.Key] = GatherTextLines(KeyValue.Value);
            }
            return result;
        }

        static List<LineItem> GatherTextLines(in FileItem fileItem)
        {
            var result = new List<LineItem>();
            var allLines = File.ReadAllLines(fileItem.AbsFilename);
            foreach (var item in fileItem.LineItems)
            {
                var newItem = new LineItem(item.Line, item.UserData);
                newItem.Text = allLines[item.Line].Trim();
                result.Add(newItem);
            }
            return result;
        }
    }
}
