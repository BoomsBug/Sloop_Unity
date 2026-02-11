using UnityEngine;

namespace Sloop.World
{
    public static class IslandVisitContext
    {
        public static int WorldSeed { get; private set; }
        public static int IslandID { get; private set; }
        public static string Morality { get; private set; }  // "R", "N", "H"
        public static bool HasTreasure { get; private set; }

        public static void Set(int worldSeed, int islandID, string morality, bool hasTreasure)
        {
            WorldSeed = worldSeed;
            IslandID = islandID;
            Morality = morality;
            HasTreasure = hasTreasure;
        }
    }
}
