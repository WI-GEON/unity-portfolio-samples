namespace PortfolioSamples.Perks
{
    /// <summary>스탯을 바꾸는 Perk (예: 최대 체력 +5)</summary>
    public interface IStatPerk
    {
        void ApplyStats(PlayerStats stats);
    }

    /// <summary>공격이 적중했을 때 추가 효과를 내는 Perk</summary>
    public interface IOnHitPerk
    {
        void OnHit(HitContext hit);
    }

    /// <summary>공격 방식 자체를 바꾸는 Perk (탄 수, 탄 퍼짐 등)</summary>
    public interface IAttackPerk
    {
        void ModifyAttack(AttackSpec attack);
    }

    /// <summary>성향 계산에 쓰는 계열. 몬스터 계열에는 수인 계열이 포함된다.</summary>
    public enum PerkFaction
    {
        Neutral,
        Knight,
        Monster
    }
}
