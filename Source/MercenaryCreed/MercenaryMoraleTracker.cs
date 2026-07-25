using IsekaiLeveling.Quests;
using UnityEngine;
using Verse;

namespace MercenaryCreed
{
    // Holds the mercenary company's "morale" - a single colony-wide value that rides from -MaxMorale
    // (restless, no work) up to +MaxMorale (content, steady contracts). Completing an Isekai guild bounty
    // pushes it UP by the bounty's rank; the rest of the time it drifts DOWN (restlessness). So a company
    // that keeps taking contracts stays in the buff range, and one that lets the work dry up slides through
    // zero into the debuff range. Persisted; auto-instantiated by RimWorld for every game.
    public class MercenaryMoraleTracker : GameComponent
    {
        // Bound on the swing in each direction: +MaxMorale content, -MaxMorale restless. A single SSS
        // bounty (+20) is a full-range swing.
        public const float MaxMorale = 20f;

        // How much morale drifts DOWN per in-game day with no fresh contracts. The same rate governs both
        // the buff fading and the debuff building - one continuous downward drift, with bounties the only
        // thing pushing back up.
        public const float DriftPerDay = 4f;

        private const int TicksPerDay = 60000;

        private float morale;

        public MercenaryMoraleTracker(Game game)
        {
        }

        public static MercenaryMoraleTracker Get() => Current.Game?.GetComponent<MercenaryMoraleTracker>();

        // Current mood offset for the situational thought (rounded, clamped). Positive = content (buff),
        // negative = restless (debuff), zero = neutral (no thought).
        public int MoodOffset => Mathf.Clamp(Mathf.RoundToInt(morale), -(int)MaxMorale, (int)MaxMorale);

        // Raise morale by the rank-scaled amount for a completed bounty (climbs out of the debuff first).
        public void AddForRank(QuestRank rank)
        {
            morale = Mathf.Clamp(morale + RankBonus(rank), -MaxMorale, MaxMorale);
        }

        // Rank -> morale gained. F none, then a gentle climb that reaches the full cap at SSS.
        private static float RankBonus(QuestRank rank)
        {
            switch (rank)
            {
                case QuestRank.F:   return 0f;
                case QuestRank.E:   return 1f;
                case QuestRank.D:   return 2f;
                case QuestRank.C:   return 5f;
                case QuestRank.B:   return 8f;
                case QuestRank.A:   return 11f;
                case QuestRank.S:   return 14f;
                case QuestRank.SS:  return 17f;
                case QuestRank.SSS: return 20f;
                default:            return 0f;
            }
        }

        public override void GameComponentTick()
        {
            // Constant downward drift: with no fresh contracts the company grows restless, sliding from the
            // buff range down through zero into the debuff range, floored at -MaxMorale.
            if (morale > -MaxMorale)
            {
                morale -= DriftPerDay / TicksPerDay;
                if (morale < -MaxMorale)
                {
                    morale = -MaxMorale;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref morale, "mercenaryMorale", 0f);
        }
    }
}
