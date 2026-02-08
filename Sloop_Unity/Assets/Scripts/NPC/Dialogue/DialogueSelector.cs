using System;
using System.Collections.Generic;
using System.Linq;
using Sloop.NPC.dialogue;

namespace Sloop.NPC.Dialogue
{
    public enum DialogueType { Bark, Interact }
    public enum WillingnessBand { Hostile, Neutral, Friendly }

    public static class DialogueSelector
    {
        public static string GetLine(
            DialogueDatabaseJson db,
            DialogueType type,
            Sloop.NPC.NPCData npc,
            WillingnessBand band)
        {
            if (db == null || db.entries == null || db.entries.Count == 0)
                return "(no dialogue loaded)";
            
            string typeTag = type == DialogueType.Bark ? "bark" : "interact";
            string alignmentTag = $"alignment:{npc.alignment.ToString().ToLowerInvariant()}";
            string roleTag = $"role:{npc.role.ToString().ToLowerInvariant()}";
            string willingnessTag = $"willingness:{band.ToString().ToLowerInvariant()}";

            // Priority-ordered queries (most specific to least specific)
            var queries = new List<string[]>
            {
                new[] { alignmentTag, roleTag, willingnessTag },
                new[] { alignmentTag, roleTag },                    
                new[] { roleTag, willingnessTag },
                new[] { roleTag },
                new[] { alignmentTag },
                Array.Empty<string>()
            };

            foreach (var requiredTags in queries)
            {
                var matches = db.entries
                    .Where(e => string.Equals(e.type, typeTag, StringComparison.OrdinalIgnoreCase))
                    .Where(e => HasAllTags(e.tags, requiredTags))
                    .Where(e => !string.IsNullOrWhiteSpace(e.text))
                    .ToList();

                if (matches.Count > 0)
                {
                    // Random pick (non-deterministic). Can make deterministic later if desired.
                    int idx = UnityEngine.Random.Range(0, matches.Count);
                    return matches[idx].text;
                }
            }

            return "(no matching dialogue)";
        }

        private static bool HasAllTags(List<string> entryTags, string[] requiredTags)
        {
            if (requiredTags == null || requiredTags.Length == 0) return true;
            if (entryTags == null) return false;

            // Case-insensitive tag compare
            var set = new HashSet<string>(entryTags.Select(t => t.ToLowerInvariant()));
            foreach (var req in requiredTags)
            {
                if (!set.Contains(req.ToLowerInvariant()))
                    return false;
            }
            return true;
        }
    }
}