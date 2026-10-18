using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioSamples.Perks
{
    /// <summary>
    /// JSON으로 저장하는 것은 두 가지뿐: Perk별 해금 여부, 현재 장착한 Perk ID 목록.
    /// 효과 수치는 Perk 데이터에 있으므로 저장하지 않는다.
    /// (Unity에서는 JsonUtility로 직렬화 — Unity/PerkSaveJson.cs)
    /// </summary>
    [Serializable]
    public sealed class PerkSaveData
    {
        public List<string> unlocked = new List<string>();
        public List<string> equipped = new List<string>();

        public static PerkSaveData From(PerkLoadout loadout)
        {
            return new PerkSaveData
            {
                unlocked = loadout.UnlockedIds.OrderBy(id => id, StringComparer.Ordinal).ToList(),
                equipped = loadout.Equipped.Select(p => p.Id).ToList()
            };
        }

        /// <summary>저장 데이터를 장착 구성으로 복원한다. 알 수 없는 ID는 건너뛴다.</summary>
        public PerkLoadout Restore(IReadOnlyDictionary<string, PerkBase> catalog)
        {
            var loadout = new PerkLoadout();
            foreach (var id in unlocked) loadout.Unlock(id);
            foreach (var id in equipped)
            {
                if (catalog.TryGetValue(id, out var perk)) loadout.Equip(perk);
            }
            return loadout;
        }
    }
}
