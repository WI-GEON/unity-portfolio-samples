namespace PortfolioSamples.Perks
{
    /// <summary>판금갑옷: 최대 체력 +5 (포트폴리오 사례 B)</summary>
    public sealed class PlateArmorPerk : PerkBase, IStatPerk
    {
        public PlateArmorPerk() : base("plate_armor", PerkFaction.Knight) { }

        public void ApplyStats(PlayerStats stats) => stats.MaxHp += 5;
    }

    /// <summary>예시: 탄 수를 늘리는 공격 방식 변경 Perk</summary>
    public sealed class SpreadShotPerk : PerkBase, IAttackPerk
    {
        public SpreadShotPerk() : base("spread_shot", PerkFaction.Monster) { }

        public void ModifyAttack(AttackSpec attack)
        {
            attack.ProjectileCount += 2;
            attack.SpreadAngle += 15f;
        }
    }

    /// <summary>예시: 적중 횟수를 세는 적중 시 효과 Perk (상태 이상 부여 등으로 확장 가능)</summary>
    public sealed class HitCounterPerk : PerkBase, IOnHitPerk
    {
        public HitCounterPerk() : base("hit_counter", PerkFaction.Monster) { }

        public int Count { get; private set; }

        public void OnHit(HitContext hit) => Count++;
    }

    /// <summary>예시: 스탯과 적중 효과를 함께 가진 Perk — 인터페이스를 여러 개 구현해도 된다.</summary>
    public sealed class KnightOathPerk : PerkBase, IStatPerk, IOnHitPerk
    {
        public KnightOathPerk() : base("knight_oath", PerkFaction.Knight) { }

        public int HitCount { get; private set; }

        public void ApplyStats(PlayerStats stats) => stats.Luck += 1;

        public void OnHit(HitContext hit) => HitCount++;
    }
}
