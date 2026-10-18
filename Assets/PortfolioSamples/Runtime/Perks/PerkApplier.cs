namespace PortfolioSamples.Perks
{
    /// <summary>
    /// 「결정」 후 게임에 들어가 플레이어가 생성될 때 한 번만 호출한다.
    /// 적용 쪽은 Perk의 구체 종류를 모르고 인터페이스로만 분기하므로,
    /// 새 Perk는 클래스 하나만 추가하면 된다.
    /// </summary>
    public static class PerkApplier
    {
        public static void ApplyOnSpawn(Player player, PerkLoadout loadout)
        {
            player.ResetPerkState();

            foreach (var perk in loadout.Equipped)
            {
                if (perk is IStatPerk stat) stat.ApplyStats(player.Stats);
                if (perk is IOnHitPerk onHit) player.AddOnHit(onHit);
                if (perk is IAttackPerk attack) attack.ModifyAttack(player.Attack);
            }

            ApplyTendency(player.Stats, loadout.Preview());
        }

        private static void ApplyTendency(PlayerStats stats, TendencyResult tendency)
        {
            stats.Luck += tendency.LuckBonus;
            stats.FireRateMultiplier *= 1f - tendency.FireRatePenalty;
            stats.BonusDamageRate += tendency.BonusDamageRate;
            stats.MaxHpMultiplier *= 1f - tendency.MaxHpPenalty;
        }
    }
}
