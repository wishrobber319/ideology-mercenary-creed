using RimWorld;
using Verse;

namespace MercenaryCreed
{
    // Gates the WR_MercenaryMorale situational thought: active only for pawns whose ideoligion carries
    // the Mercenary meme, and only while the company actually has morale to show. The mood VALUE comes
    // from Thought_MercenaryMorale.MoodOffset(); this worker just decides on/off.
    public class ThoughtWorker_MercenaryMorale : ThoughtWorker
    {
        private static MemeDef meme;

        private static MemeDef Meme =>
            meme ?? (meme = DefDatabase<MemeDef>.GetNamedSilentFail("WR_Mercenary"));

        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (Meme == null || p?.Ideo == null || !p.Ideo.memes.Contains(Meme))
            {
                return ThoughtState.Inactive;
            }

            MercenaryMoraleTracker tracker = MercenaryMoraleTracker.Get();
            if (tracker == null || tracker.MoodOffset < 1)
            {
                return ThoughtState.Inactive;
            }

            return ThoughtState.ActiveAtStage(0);
        }
    }
}
