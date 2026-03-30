public static class RangeFactory
{
    public static Range GetRange(RangeType type)
    {
        switch (type)
        {
            case RangeType.Grid:
                return new GridRange();
            case RangeType.Sector:
                return new SectorRange();
            case RangeType.Circle:
                return new CircleRange();
            case RangeType.TileDistance:
                return new TileDistanceRange();
        }

        return null;
    }
}