using System;

namespace PortfolioSamples.StatusEffects
{
    /// <summary>
    /// 둔화: 적용한 감소분을 기억했다가 끝날 때 정확히 그만큼만 되돌린다.
    /// 다른 효과가 SpeedModifier를 함께 바꿔도 서로의 값을 덮어쓰지 않는다.
    /// </summary>
    public sealed class SlowEffect : IStatusEffect
    {
        private readonly float _duration;
        private readonly float _ratio;
        private float _remain;
        private float _reduction;

        public SlowEffect(float duration, float ratio)
        {
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration));
            if (ratio <= 0f || ratio > 1f) throw new ArgumentOutOfRangeException(nameof(ratio));
            _duration = duration;
            _ratio = ratio;
        }

        public bool IsActive { get; private set; }
        public EffectEndReason EndReason { get; private set; }

        public void Apply(IEffectTarget target)
        {
            _reduction = target.BaseSpeed * _ratio;
            target.SpeedModifier -= _reduction;
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
            target.SpeedModifier += _reduction;
            _reduction = 0f;
            IsActive = false;
            EndReason = reason;
        }
    }
}
