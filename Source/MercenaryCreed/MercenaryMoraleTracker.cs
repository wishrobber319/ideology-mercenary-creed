using IsekaiLeveling.Quests;
using UnityEngine;
using Verse;

namespace MercenaryCreed
{
    // Holds the mercenary company's "morale" - a single colony-wide value that rises when an Isekai
    // guild bounty is completed (by the bounty's rank) and slowly decays over time. Capped so morale can
    // never exceed MaxMorale no matter how many bounties are stacked. Persisted; auto-instantiated by
    // RimWorld for every game.
    public class MercenaryMoraleTracker : GameComponent
    {
        // Total combined mood cap. A single SSS bounty (+20) maxes it; lower ranks stack toward it.
        public const float MaxMorale = 20f;

        // How much morale bleeds off per in-game day when no new contracts come in.
        public const float DecayPerDay = 4f;

        private const int TicksPerDay = 60000;

        private float morale;

        public MercenaryMoraleTracker(Game game)
        {
        }

        public static MercenaryMoraleTracker Get() => Current.Game?.GetComponent<MercenaryMoraleTracker>();

        // Current mood offset shown by the situational thought (rounded, clamped).
        public int MoodOffset => Mathf.Clamp(Mathf.RoundToInt(morale), 0, (int)MaxMorale);

        // Raise morale by the rank-scaled amount for a completed bounty, up to the cap.
        public void AddForRank(QuestRank rank)
        {
            morale = Mathf.Clamp(morale + RankBonus(rank), 0f, MaxMorale);
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
            if (morale > 0f)
            {
                morale -= DecayPerDay / TicksPerDay;
                if (morale < 0f)
                {
                    morale = 0f;
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
