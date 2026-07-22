using RimWorld;

namespace MercenaryCreed
{
    // A situational thought whose mood offset is the company's live morale value (0..MaxMorale) rather
    // than a fixed per-stage number, so it slides as morale accrues from bounties and decays over time.
    public class Thought_MercenaryMorale : Thought_Situational
    {
        public override float MoodOffset()
        {
            MercenaryMoraleTracker tracker = MercenaryMoraleTracker.Get();
            return tracker != null ? tracker.MoodOffset : 0f;
        }
    }
}
