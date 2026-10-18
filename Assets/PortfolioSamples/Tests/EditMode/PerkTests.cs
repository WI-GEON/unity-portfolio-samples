using System.Collections.Generic;
using NUnit.Framework;
using PortfolioSamples.Perks;

namespace PortfolioSamples.Tests
{
    public class PerkTests
    {
        private sealed class KnightPerk : PerkBase
        {
            public KnightPerk(string id) : base(id, PerkFaction.Knight) { }
        }

        private sealed class MonsterPerk : PerkBase
        {
            public MonsterPerk(string id) : base(id, PerkFaction.Monster) { }
        }

        private static PerkLoadout LoadoutWith(params PerkBase[] perks)
        {
            var loadout = new PerkLoadout();
            foreach (var p in perks)
            {
                loadout.Unlock(p.Id);
                loadout.Equip(p);
            }
            return loadout;
        }

        [Test]
        public void Equip_MoreThanSixSlots_IsRejected()
        {
            var loadout = new PerkLoadout();
            for (int i = 0; i < 7; i++) loadout.Unlock("k" + i);
            for (int i = 0; i < 6; i++) Assert.That(loadout.Equip(new KnightPerk("k" + i)), Is.EqualTo(EquipResult.Equipped));

            Assert.That(loadout.Equip(new KnightPerk("k6")), Is.EqualTo(EquipResult.SlotsFull));
        }

        [Test]
        public void Equip_LockedPerk_IsRejected()
        {
            var loadout = new PerkLoadout();
            Assert.That(loadout.Equip(new PlateArmorPerk()), Is.EqualTo(EquipResult.NotUnlocked));
        }

        [TestCase(3, 3, 0, 0)]
        [TestCase(4, 2, 2, 2)]
        [TestCase(5, 1, 4, 3)]   // score 4 → 상한 3단계
        [TestCase(1, 4, -3, 3)]
        public void Tendency_IsCountDifference_CappedAtThree(int knights, int monsters, int score, int stage)
        {
            var perks = new List<PerkBase>();
            for (int i = 0; i < knights; i++) perks.Add(new KnightPerk("k" + i));
            for (int i = 0; i < monsters; i++) perks.Add(new MonsterPerk("m" + i));

            var t = TendencyCalculator.Calculate(perks);

            Assert.That(t.Score, Is.EqualTo(score));
            Assert.That(t.Stage, Is.EqualTo(stage));
        }

        [Test]
        public void Tendency_KnightStageOne_LuckUpFireRateDown()
        {
            var t = TendencyCalculator.Calculate(new PerkBase[] { new KnightPerk("k") });

            Assert.That(t.LuckBonus, Is.EqualTo(1));
            Assert.That(t.FireRatePenalty, Is.EqualTo(0.05f).Within(1e-4));
        }

        [Test]
        public void Preview_DoesNotChangePlayerStats()
        {
            var player = new Player(new PlayerStats { MaxHp = 20 });
            var loadout = LoadoutWith(new PlateArmorPerk());

            loadout.Preview();

            Assert.That(player.Stats.MaxHp, Is.EqualTo(20));
        }

        [Test]
        public void ApplyOnSpawn_PlateArmor_MaxHpTwentyToTwentyFive()
        {
            var player = new Player(new PlayerStats { MaxHp = 20 });
            var loadout = LoadoutWith(new PlateArmorPerk(), new MonsterPerk("m"));   // 기사 1 : 몬스터 1 → 성향 효과 없음

            PerkApplier.ApplyOnSpawn(player, loadout);

            Assert.That(player.Stats.EffectiveMaxHp, Is.EqualTo(25));
        }

        [Test]
        public void ApplyOnSpawn_DispatchesByInterface()
        {
            var player = new Player(new PlayerStats { MaxHp = 20 });
            var counter = new HitCounterPerk();
            var loadout = LoadoutWith(new SpreadShotPerk(), counter);

            PerkApplier.ApplyOnSpawn(player, loadout);
            player.NotifyHit(new HitContext(null, 5));

            Assert.That(player.Attack.ProjectileCount, Is.EqualTo(3));
            Assert.That(counter.Count, Is.EqualTo(1));
        }

        [Test]
        public void ApplyOnSpawn_PerkWithTwoInterfaces_AppliesBoth()
        {
            var player = new Player(new PlayerStats());
            var oath = new KnightOathPerk();
            var loadout = LoadoutWith(oath, new MonsterPerk("m"));

            PerkApplier.ApplyOnSpawn(player, loadout);
            player.NotifyHit(new HitContext(null, 1));

            Assert.That(player.Stats.Luck, Is.EqualTo(1));
            Assert.That(oath.HitCount, Is.EqualTo(1));
        }

        [Test]
        public void ResetPerkState_OnRespawn_ClearsEverything()
        {
            var player = new Player(new PlayerStats { MaxHp = 20 });
            PerkApplier.ApplyOnSpawn(player, LoadoutWith(new PlateArmorPerk(), new HitCounterPerk()));

            player.ResetPerkState();

            Assert.That(player.Stats.MaxHp, Is.EqualTo(20));
            Assert.That(player.OnHitPerks.Count, Is.EqualTo(0));
        }

        [Test]
        public void SaveData_KeepsOnlyUnlockedAndEquippedIds()
        {
            var plate = new PlateArmorPerk();
            var spread = new SpreadShotPerk();
            var loadout = LoadoutWith(plate);
            loadout.Unlock(spread.Id);

            var data = PerkSaveData.From(loadout);
            var catalog = new Dictionary<string, PerkBase> { { plate.Id, plate }, { spread.Id, spread } };
            var restored = data.Restore(catalog);

            Assert.That(data.unlocked, Is.EquivalentTo(new[] { "plate_armor", "spread_shot" }));
            Assert.That(data.equipped, Is.EqualTo(new[] { "plate_armor" }));
            Assert.That(restored.Equipped.Count, Is.EqualTo(1));
            Assert.That(restored.IsUnlocked("spread_shot"), Is.True);
        }
    }
}
