
namespace Sloop.NPC
{
    public static class NPCSlotRules
    {
        public const int NPCS_PER_ISLAND = 5;

        public static NPCRole RoleForSlot(int npcIndex)
        {
            return npcIndex switch
            {
                0 => NPCRole.Merchant,
                1 => NPCRole.Deckhand,
                2 => NPCRole.Deckhand,
                3 => NPCRole.Deckhand,
                4 => NPCRole.Civilian,
                _ => NPCRole.Civilian
            };
        }
    }
}