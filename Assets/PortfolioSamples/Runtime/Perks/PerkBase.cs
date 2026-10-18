using System;

namespace PortfolioSamples.Perks
{
    /// <summary>
    /// Perk 데이터의 공통 부분. Unity에서는 ScriptableObject로 에셋화해 쓸 수 있고,
    /// 여기서는 테스트를 위해 순수 C# 클래스로 둔다.
    /// 효과는 필요한 인터페이스만 골라 구현한다.
    /// </summary>
    public abstract class PerkBase
    {
        protected PerkBase(string id, PerkFaction faction)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Perk id is required.", nameof(id));
            Id = id;
            Faction = faction;
        }

        public string Id { get; }
        public PerkFaction Faction { get; }
    }
}
