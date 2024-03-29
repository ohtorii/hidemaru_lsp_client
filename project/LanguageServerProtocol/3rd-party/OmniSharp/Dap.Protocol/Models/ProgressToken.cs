﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace LSP.Model //OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [JsonConverter(typeof(ProgressTokenConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    class ProgressToken : IEquatable<long>, IEquatable<string>
    {
        private long? _long;
        private string _string;

        public ProgressToken(Guid value)
        {
            _string = value.ToString();
            _long = null;
        }

        public ProgressToken(long value)
        {
            _long = value;
            _string = null;
        }

        public ProgressToken(string value)
        {
            _long = null;
            _string = value;
        }

        public bool IsLong => _long.HasValue;

        public long Long
        {
            get => _long ?? 0;
            set
            {
                _string = null;
                _long = value;
            }
        }

        public bool IsString => _string != null;

        public string String
        {
            get => _string ?? string.Empty;
            set
            {
                _string = value;
                _long = null;
            }
        }

        public static implicit operator ProgressToken(long value) => new ProgressToken(value);

        public static implicit operator ProgressToken(string value) => new ProgressToken(value);
        public static implicit operator ProgressToken(Guid value) => new ProgressToken(value);

        public bool Equals(long other) => IsLong && Long == other;
        public bool Equals(string other) => IsString && String == other;

        private string DebuggerDisplay => IsString ? String : IsLong ? Long.ToString() : "";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
