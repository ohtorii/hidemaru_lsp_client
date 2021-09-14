using System;

namespace HidemaruLspClient_FrontEnd
{
    public sealed class LocationContainerImpl
    {
        public LocationContainerImpl(HidemaruLspClient_BackEndContract.ILocationContainer locations)
        {
            locations_ = locations;
        }
        public LocationImpl Item(long index)
        {
            if (locations_ == null)
            {
                return null;
            }
            return new LocationImpl(locations_.Item(index));
        }

        public long Length
        {
            get
            {
                if (locations_ == null)
                {
                    return 0;
                }
                return locations_.Length;
            }
        }

        readonly HidemaruLspClient_BackEndContract.ILocationContainer locations_;
    }

    public sealed class PositionImpl
    {
        public PositionImpl(HidemaruLspClient_BackEndContract.IPosition position)
        {
            if (position == null)
            {
                hidemaruCharacter_ = -1;
                hidemaruLine_ = -1;
                return;
            }
            Hidemaru.ZeroBaseToHidemaru(out hidemaruLine_,
                                        out hidemaruCharacter_,
                                        position.line,
                                        position.character);
        }
        public long character => hidemaruCharacter_;
        public long line => hidemaruLine_;

        readonly long hidemaruCharacter_;
        readonly long hidemaruLine_;
    }
    public sealed class RangeImpl
    {
        public RangeImpl(HidemaruLspClient_BackEndContract.IRange range)
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
    public sealed class LocationImpl
    {
        public LocationImpl(HidemaruLspClient_BackEndContract.ILocation location)
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
        readonly HidemaruLspClient_BackEndContract.ILocation location_;
    }
}
