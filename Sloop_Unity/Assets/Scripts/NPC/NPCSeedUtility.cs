namespace Sloop.NPC
{
    public static class NPCSeedUtility
    {
        // Form (worldSeed, islandID, npcIndex)
        // Used to derive a unique npcSeed
        public static int Combine(int a, int b, int c)
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash ^ a) * 16777619;
                hash = (hash ^ b) * 16777619;
                hash = (hash ^ c) * 16777619;
                return hash;
            }
        }
    }
}
