using System;
using System.Collections.Generic;

namespace HidemaruLspClient.ComContract
{
    class LanguageServerIdentifier : IEquatable<LanguageServerIdentifier>
    {
        public LanguageServerIdentifier(string serverName, string rootUri)
        {
            ServerName = serverName;
            RootUri = rootUri.ToLower();
            Hash = HashCode.Combine(ServerName, RootUri);
        }

        readonly string ServerName;
        readonly string RootUri;
        readonly int Hash;

        public override bool Equals(object obj)
        {
            return Equals(obj as LanguageServerIdentifier);
        }

        public bool Equals(LanguageServerIdentifier other)
        {
            return other != null &&
                    ServerName == other.ServerName &&
                    RootUri == other.RootUri;
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public static bool operator ==(LanguageServerIdentifier left, LanguageServerIdentifier right)
        {
            return EqualityComparer<LanguageServerIdentifier>.Default.Equals(left, right);
        }

        public static bool operator !=(LanguageServerIdentifier left, LanguageServerIdentifier right)
        {
            return !(left == right);
        }
    }
}
