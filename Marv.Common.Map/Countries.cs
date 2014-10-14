using System.Collections.Generic;

namespace Marv.Map
{
    public static class Countries
    {
        public static readonly Dictionary<string, LocationRect> BoundsForKey = new Dictionary<string, LocationRect>
        {
            { "China", China },
            { "UAE", UAE },
            { "USA", USA }
        };

        public static readonly LocationRect China = new LocationRect { North = 54, East = 135, South = 20, West = 73 };
        public static readonly LocationRect UAE = new LocationRect { North = 30, East = 60, West = 50, South = 20 };
        public static readonly LocationRect USA = new LocationRect { South = 25, West = -124, North = 50, East = -66 };
    }
}