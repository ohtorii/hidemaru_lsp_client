using System;
using System.Collections.Generic;

namespace HidemaruLspClient
{
    class LspKey : IEquatable<LspKey>
    {
        public LspKey(string serverName, string rootUri)
        {
            ServerName = serverName;
            RootUri    = rootUri.ToLower();
            Hash       = HashCode.Combine(ServerName, RootUri);
        }

        readonly string ServerName;
        readonly string RootUri;
        readonly int Hash;

        public override bool Equals(object obj)
        {
            return Equals(obj as LspKey);
        }

        public bool Equals(LspKey other)
        {
            return other != null &&
                    ServerName == other.ServerName &&
                    RootUri == other.RootUri;
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public static bool operator ==(LspKey left, LspKey right)
        {
            return EqualityComparer<LspKey>.Default.Equals(left, right);
        }

        public static bool operator !=(LspKey left, LspKey right)
        {
            return !(left == right);
        }
    }
}
