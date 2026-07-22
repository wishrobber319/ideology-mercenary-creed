using HarmonyLib;
using IsekaiLeveling.Quests;
using Verse;

namespace MercenaryCreed
{
    [StaticConstructorOnStartup]
    public static class MercenaryCreedMod
    {
        static MercenaryCreedMod()
        {
            new Harmony("wishRobber.mercenarycreed").PatchAll();
        }
    }

    // When an Isekai guild bounty is completed, raise the mercenary company's morale by an amount scaled
    // to the bounty's rank. QuestPart_IsekaiLocalHunt.CompleteLocalHunt is the base mod's single, guarded
    // completion point (it fires exactly once, the moment the bounty target is killed and the quest ends
    // in success), so this counts each bounty once. Morale only shows for a Mercenary-creed colony; on
    // any other ideoligion the value simply goes unused.
    [HarmonyPatch(typeof(QuestPart_IsekaiLocalHunt), "CompleteLocalHunt")]
    public static class Patch_CompleteLocalHunt_Morale
    {
        public static void Postfix(QuestPart_IsekaiLocalHunt __instance)
        {
            MercenaryMoraleTracker.Get()?.AddForRank(__instance.rank);
        }
    }
}
