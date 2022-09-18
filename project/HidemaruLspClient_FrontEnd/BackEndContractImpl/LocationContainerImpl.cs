using HidemaruLspClient_FrontEnd.Hidemaru;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HidemaruLspClient_FrontEnd.BackEndContractImpl
{
    //todo: インターフェースと実装に分離する
	[ComVisible(true)]
	[Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9592")]
	public sealed class LocationContainerImpl
    {
       internal class WithContent
       {
            internal WithContent(HidemaruLspClient_BackEndContract.ILocation location, string contentText = "")
            {
                this.location = location;
                this.text = contentText;
            }
            HidemaruLspClient_BackEndContract.ILocation location;

            public string uri => location.uri;
            public HidemaruLspClient_BackEndContract.IRange range => location.range;
            public string text { get; set; }
        }

        internal LocationContainerImpl(List<LocationContainerImpl.WithContent> locations)
        {
            locations_ = locations;
        }
        public LocationImpl Item(long index)
        {
            if (locations_ == null)
            {
                return null;
            }
            return new LocationImpl(locations_[(int)index]);
        }

        public long Length
        {
            get
            {
                if (locations_ == null)
                {
                    return 0;
                }
                return locations_.Count;
            }
        }

        readonly List<LocationContainerImpl.WithContent> locations_;
    }

	//todo: インターフェースと実装に分離する
	[ComVisible(true)]
	[Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9594")]
	public sealed class PositionImpl
    {
        internal PositionImpl(HidemaruLspClient_BackEndContract.IPosition position)
        {
            if (position == null)
            {
                hidemaruCharacter_ = -1;
                hidemaruLine_ = -1;
                return;
            }
            Api.ZeroBaseToHidemaru(out hidemaruLine_,
                                        out hidemaruCharacter_,
                                        position.line,
                                        position.character);
        }
        public long character => hidemaruCharacter_;
        public long line => hidemaruLine_;

        readonly long hidemaruCharacter_;
        readonly long hidemaruLine_;
    }

    //todo: インターフェースと実装に分離する
    [ComVisible(true)]
	[Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9595")]
	public sealed class RangeImpl
    {
        internal RangeImpl(HidemaruLspClient_BackEndContract.IRange range)
        {
            range_ = range;
        }
        public PositionImpl start
        {
            get
            {
                if (range_ == null)
                {
                    return null;
                }
                return new PositionImpl(range_.start);
            }
        }
        public PositionImpl end
        {
            get
            {
                if (range_ == null)
                {
                    return null;
                }
                return new PositionImpl(range_.end);
            }
        }
        readonly HidemaruLspClient_BackEndContract.IRange range_;
    }

	//todo: インターフェースと実装に分離する
	[ComVisible(true)]
	[Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9596")]
	public sealed class LocationImpl
    {
        internal LocationImpl(LocationContainerImpl.WithContent location)
        {
            location_ = location;
        }
        public string AbsFilename
        {
            get
            {
                if (location_ == null)
                {
                    return "";
                }
                var uri = new Uri(location_.uri);
                return uri.LocalPath;
            }
        }
        public RangeImpl range
        {
            get
            {
                if (location_ == null)
                {
                    return null;
                }
                return new RangeImpl(location_.range);
            }
        }
        /// <summary>
        /// The content of the referenced line
        ///(Memo)LSPには存在しない、秀丸エディタ独自の追加メソッド
        /// </summary>
        public string Text
        {
            get
            {
                if (location_ == null)
                {
                    return "";
                }
                return location_.text;
            }
        }

        readonly LocationContainerImpl.WithContent location_;
    }
}
