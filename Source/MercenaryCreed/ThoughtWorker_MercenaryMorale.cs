using RimWorld;
using Verse;

namespace MercenaryCreed
{
    // Gates the WR_MercenaryMorale situational thought: active only for pawns whose ideoligion carries
    // the Mercenary meme. Picks the stage by the sign of the company's morale - stage 0 (content, buff)
    // when positive, stage 1 (restless, debuff) when negative, inactive at neutral. The mood VALUE comes
    // from Thought_MercenaryMorale.MoodOffset().
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
            if (tracker == null)
            {
                return ThoughtState.Inactive;
            }

            int mood = tracker.MoodOffset;
            if (mood >= 1)
            {
                return ThoughtState.ActiveAtStage(0); // content - buff
            }
            if (mood <= -1)
            {
                return ThoughtState.ActiveAtStage(1); // restless - debuff
            }
            return ThoughtState.Inactive;             // neutral
        }
    }
}
