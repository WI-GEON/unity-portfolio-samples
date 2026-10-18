using System;
using System.Collections.Generic;

namespace PortfolioSamples.Perks
{
    public sealed class PlayerStats
    {
        public int MaxHp;
        public int Luck;
        public float FireRateMultiplier = 1f;
        public float BonusDamageRate;      // 0.1 = +10%
        public float MaxHpMultiplier = 1f;

        public PlayerStats Clone()
        {
            return (PlayerStats)MemberwiseClone();
        }

        public int EffectiveMaxHp => (int)Math.Round(MaxHp * MaxHpMultiplier);
    }

    public sealed class AttackSpec
    {
        public int ProjectileCount = 1;
        public float SpreadAngle;
    }

    public sealed class HitContext
    {
        public HitContext(object target, int damage)
        {
            Target = target;
            Damage = damage;
        }

        public object Target { get; }
        public int Damage { get; }
    }

    /// <summary>Perk가 붙는 플레이어. 생성 시 기본값을 기억해 두고, 사망 · 재생성 때 되돌린다.</summary>
    public sealed class Player
    {
        private readonly PlayerStats _baseStats;
        private readonly List<IOnHitPerk> _onHitPerks = new List<IOnHitPerk>();

        public Player(PlayerStats baseStats)
        {
            _baseStats = baseStats.Clone();
            Stats = baseStats.Clone();
        }

        public PlayerStats Stats { get; private set; }
        public AttackSpec Attack { get; private set; } = new AttackSpec();
        public IReadOnlyList<IOnHitPerk> OnHitPerks => _onHitPerks;

        public void AddOnHit(IOnHitPerk perk) => _onHitPerks.Add(perk);

        public void NotifyHit(HitContext hit)
        {
            foreach (var perk in _onHitPerks) perk.OnHit(hit);
        }

        /// <summary>사망 · 재생성 시 Perk 관련 상태를 모두 초기화한다.</summary>
        public void ResetPerkState()
        {
            Stats = _baseStats.Clone();
            Attack = new AttackSpec();
            _onHitPerks.Clear();
        }
    }
}
