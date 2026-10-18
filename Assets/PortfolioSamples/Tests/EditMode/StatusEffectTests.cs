using NUnit.Framework;
using PortfolioSamples.StatusEffects;

namespace PortfolioSamples.Tests
{
    public class StatusEffectTests
    {
        private sealed class FakeMonster : IEffectTarget
        {
            public int Hp = 10;
            public bool IsDead => Hp <= 0;
            public bool CanMove { get; set; } = true;
            public bool DealsContactDamage { get; set; } = true;
            public float BaseSpeed => 4f;
            public float SpeedModifier { get; set; }
            public int DamageTaken;
            public void TakeDamage(int amount) { Hp -= amount; DamageTaken += amount; }
        }

        [Test]
        public void Freeze_WhileActive_BlocksMovementAndContactDamage()
        {
            var m = new FakeMonster();
            var runner = new StatusEffectRunner(m);
            runner.Add(new FreezeEffect(2f));
            runner.Tick(1f);

            Assert.That(m.CanMove, Is.False);
            Assert.That(m.DealsContactDamage, Is.False);
        }

        [Test]
        public void Freeze_Expired_ResumesMovement()
        {
            var m = new FakeMonster();
            var freeze = new FreezeEffect(2f);
            var runner = new StatusEffectRunner(m);
            runner.Add(freeze);

            runner.Tick(2.1f);

            Assert.That(freeze.EndReason, Is.EqualTo(EffectEndReason.Expired));
            Assert.That(m.CanMove, Is.True);
            Assert.That(m.DealsContactDamage, Is.True);
            Assert.That(runner.ActiveCount, Is.EqualTo(0));
        }

        [Test]
        public void Freeze_TargetDied_DoesNotResumeMovement()
        {
            var m = new FakeMonster();
            var freeze = new FreezeEffect(5f);
            var runner = new StatusEffectRunner(m);
            runner.Add(freeze);

            m.TakeDamage(999);
            runner.Tick(0.1f);

            Assert.That(freeze.EndReason, Is.EqualTo(EffectEndReason.TargetDied));
            Assert.That(m.CanMove, Is.False);
        }

        [Test]
        public void Slow_End_RestoresExactlyTheAppliedReduction()
        {
            var m = new FakeMonster { SpeedModifier = 1f };   // 다른 효과가 이미 +1 해 둔 상태
            var runner = new StatusEffectRunner(m);
            runner.Add(new SlowEffect(1f, 0.5f));

            Assert.That(m.SpeedModifier, Is.EqualTo(1f - 2f).Within(1e-4));

            runner.Tick(1.5f);

            Assert.That(m.SpeedModifier, Is.EqualTo(1f).Within(1e-4));
        }

        [Test]
        public void FireDot_DealsDamageAtEachInterval()
        {
            var m = new FakeMonster { Hp = 100 };
            var runner = new StatusEffectRunner(m);
            runner.Add(new FireDotEffect(3f, 1f, 2));

            for (int i = 0; i < 8; i++) runner.Tick(0.5f);   // 4초 진행

            Assert.That(m.DamageTaken, Is.EqualTo(3 * 2));
            Assert.That(runner.ActiveCount, Is.EqualTo(0));
        }

        [Test]
        public void FireDot_LargeFrame_ProcessesMissedTicks()
        {
            var m = new FakeMonster { Hp = 100 };
            var runner = new StatusEffectRunner(m);
            runner.Add(new FireDotEffect(5f, 1f, 1));

            runner.Tick(2.5f);

            Assert.That(m.DamageTaken, Is.EqualTo(2));
        }

        [Test]
        public void FireDot_StopsWhenTargetDies()
        {
            var m = new FakeMonster { Hp = 3 };
            var fire = new FireDotEffect(10f, 1f, 2);
            var runner = new StatusEffectRunner(m);
            runner.Add(fire);

            runner.Tick(5f);

            Assert.That(fire.EndReason, Is.EqualTo(EffectEndReason.TargetDied));
            Assert.That(m.DamageTaken, Is.EqualTo(4));   // 2틱 만에 사망, 그 뒤로는 피해 없음
        }

        [Test]
        public void Stun_BlocksBehaviourUntilExpired()
        {
            var stun = new StunState();
            stun.Begin(1f);

            Assert.That(stun.Tick(0.5f), Is.True);
            Assert.That(stun.Tick(0.6f), Is.True);    // 이번 프레임에 끝나지만 이번 프레임까지는 차단
            Assert.That(stun.Tick(0.1f), Is.False);   // 다음 프레임부터 기존 행동 재개
        }

        [Test]
        public void Stun_ReapplyKeepsLongerRemainingTime()
        {
            var stun = new StunState();
            stun.Begin(2f);
            stun.Begin(0.5f);

            stun.Tick(1f);

            Assert.That(stun.IsStunned, Is.True);
        }
    }
}
