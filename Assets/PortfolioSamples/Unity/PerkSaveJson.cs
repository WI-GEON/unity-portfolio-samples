using System.IO;
using PortfolioSamples.Perks;
using UnityEngine;

namespace PortfolioSamples.Unity
{
    /// <summary>해금 여부와 장착 ID 목록만 JsonUtility로 저장한다.</summary>
    public static class PerkSaveJson
    {
        private static string FilePath => Path.Combine(Application.persistentDataPath, "perk_save.json");

        public static void Save(PerkLoadout loadout)
        {
            var json = JsonUtility.ToJson(PerkSaveData.From(loadout), prettyPrint: true);
            File.WriteAllText(FilePath, json);
        }

        public static PerkSaveData Load()
        {
            if (!File.Exists(FilePath)) return new PerkSaveData();
            return JsonUtility.FromJson<PerkSaveData>(File.ReadAllText(FilePath)) ?? new PerkSaveData();
        }
    }
}
