using System;
using System.Collections.Generic;

namespace PortfolioSamples.Perks
{
    public struct TendencyResult
    {
        public int Score;               // 기사 수 − 몬스터 · 수인 수
        public int Stage;               // min(|score|, 3)
        public PerkFaction Direction;   // Neutral이면 효과 없음

        public int LuckBonus;           // 기사: +1 / +2 / +3
        public float FireRatePenalty;   // 기사: 5% / 10% / 15%
        public float BonusDamageRate;   // 몬스터: 10% / 20% / 30%
        public float MaxHpPenalty;      // 몬스터: 10% / 20% / 30%
    }

    /// <summary>
    /// 성향은 비용 합이 아니라, 장착한 계열별 개수 차이로 계산한다.
    /// 단계마다 이득과 대가가 함께 커진다.
    /// </summary>
    public static class TendencyCalculator
    {
        public const int MaxStage = 3;

        public static TendencyResult Calculate(IEnumerable<PerkBase> equipped)
        {
            int knights = 0, monsters = 0;
            foreach (var perk in equipped)
            {
                if (perk.Faction == PerkFaction.Knight) knights++;
                else if (perk.Faction == PerkFaction.Monster) monsters++;
            }

            var result = new TendencyResult { Score = knights - monsters };
            result.Stage = Math.Min(Math.Abs(result.Score), MaxStage);

            if (result.Score > 0)
            {
                result.Direction = PerkFaction.Knight;
                result.LuckBonus = result.Stage;
                result.FireRatePenalty = 0.05f * result.Stage;
            }
            else if (result.Score < 0)
            {
                result.Direction = PerkFaction.Monster;
                result.BonusDamageRate = 0.10f * result.Stage;
                result.MaxHpPenalty = 0.10f * result.Stage;
            }
            else
            {
                result.Direction = PerkFaction.Neutral;
            }

            return result;
        }
    }
}
