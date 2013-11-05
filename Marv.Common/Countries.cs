﻿namespace Marv.Common
{
    public static class Countries
    {
        public static LocationRect China = new LocationRect { North = 54, East = 135, South = 20, West = 73 };
        public static LocationRect UAE = new LocationRect { North = 30, East = 60, West = 50, South = 20 };
        public static LocationRect USA = new LocationRect { South = 25, West = -124, North = 50, East = -66 };

        public static Dict<string, LocationRect> BoundsForKey = new Dict<string, LocationRect>
        {
            { "China", Countries.China },
            { "UAE", Countries.UAE },
            { "USA", Countries.USA }
        };
    }
}