using System;

namespace PortfolioSamples.StatusEffects
{
    /// <summary>
    /// 빙결: 움직임 전체 제한, 근접해도 플레이어에게 피해를 주지 않음.
    /// 시간이 끝나면 다시 움직이고, 대상이 죽으면 재개하지 않는다.
    /// </summary>
    public sealed class FreezeEffect : IStatusEffect
    {
        private readonly float _duration;
        private float _remain;

        public FreezeEffect(float duration)
        {
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration));
            _duration = duration;
        }

        public bool IsActive { get; private set; }
        public EffectEndReason EndReason { get; private set; }

        public void Apply(IEffectTarget target)
        {
            target.CanMove = false;
            target.DealsContactDamage = false;
            _remain = _duration;
            IsActive = true;
            EndReason = EffectEndReason.None;
        }

        public void Tick(IEffectTarget target, float deltaTime)
        {
            if (!IsActive) return;

            if (target.IsDead)
            {
                End(target, EffectEndReason.TargetDied);
                return;
            }

            _remain -= deltaTime;
            if (_remain <= 0f) End(target, EffectEndReason.Expired);
        }

        private void End(IEffectTarget target, EffectEndReason reason)
        {
            IsActive = false;
            EndReason = reason;

            // 해제와 행동 재개를 같은 결과로 처리하지 않는다.
            if (reason == EffectEndReason.Expired)
            {
                target.CanMove = true;
                target.DealsContactDamage = true;
            }
        }
    }
}
