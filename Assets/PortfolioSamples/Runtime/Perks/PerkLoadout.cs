using System.Collections.Generic;
using System.Linq;

namespace PortfolioSamples.Perks
{
    public enum EquipResult
    {
        Equipped,
        AlreadyEquipped,
        NotUnlocked,
        SlotsFull
    }

    /// <summary>최대 6개 슬롯. 장착 · 해제는 미리보기만 바꾸고, 능력치는 바꾸지 않는다.</summary>
    public sealed class PerkLoadout
    {
        public const int MaxSlots = 6;

        private readonly HashSet<string> _unlocked = new HashSet<string>();
        private readonly List<PerkBase> _equipped = new List<PerkBase>();

        public IReadOnlyList<PerkBase> Equipped => _equipped;
        public IEnumerable<string> UnlockedIds => _unlocked;

        public void Unlock(string perkId) => _unlocked.Add(perkId);
        public bool IsUnlocked(string perkId) => _unlocked.Contains(perkId);

        public EquipResult Equip(PerkBase perk)
        {
            if (!_unlocked.Contains(perk.Id)) return EquipResult.NotUnlocked;
            if (_equipped.Any(p => p.Id == perk.Id)) return EquipResult.AlreadyEquipped;
            if (_equipped.Count >= MaxSlots) return EquipResult.SlotsFull;

            _equipped.Add(perk);
            return EquipResult.Equipped;
        }

        public bool Unequip(string perkId) => _equipped.RemoveAll(p => p.Id == perkId) > 0;

        /// <summary>장착 즉시 UI가 보여줄 성향 미리보기</summary>
        public TendencyResult Preview() => TendencyCalculator.Calculate(_equipped);
    }
}
