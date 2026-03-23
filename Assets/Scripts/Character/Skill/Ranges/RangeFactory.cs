public static class RangeFactory
{
    public static Range GetRange(RangeType type)
    {
        switch (type)
        {
            case RangeType.Grid:
                return new GridRange();
                break;
            case RangeType.Sector:
                return new SectorRange();
                break;
        }

        return null;
    }
}