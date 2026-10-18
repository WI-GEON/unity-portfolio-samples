using System;

namespace PortfolioSamples.StatusEffects
{
    /// <summary>
    /// 화염 지속 피해: deltaTime을 누적해 일정 간격마다 피해를 준다.
    /// 프레임 간격이 커도 놓친 틱을 while로 모두 처리한다.
    /// </summary>
    public sealed class FireDotEffect : IStatusEffect
    {
        private readonly float _duration;
        private readonly float _interval;
        private readonly int _tickDamage;
        private float _remain;
        private float _elapsed;

        public FireDotEffect(float duration, float interval, int tickDamage)
        {
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration));
            if (interval <= 0f) throw new ArgumentOutOfRangeException(nameof(interval));
            if (tickDamage <= 0) throw new ArgumentOutOfRangeException(nameof(tickDamage));
            _duration = duration;
            _interval = interval;
            _tickDamage = tickDamage;
        }

        public bool IsActive { get; private set; }
        public EffectEndReason EndReason { get; private set; }

        public void Apply(IEffectTarget target)
        {
            _remain = _duration;
            _elapsed = 0f;
            IsActive = true;
            EndReason = EffectEndReason.None;
        }

        public void Tick(IEffectTarget target, float deltaTime)
        {
            if (!IsActive) return;

            if (target.IsDead)
            {
                End(EffectEndReason.TargetDied);
                return;
            }

            float step = Math.Min(deltaTime, _remain);
            _remain -= deltaTime;
            _elapsed += step;

            while (_elapsed >= _interval)
            {
                _elapsed -= _interval;
                target.TakeDamage(_tickDamage);

                if (target.IsDead)
                {
                    End(EffectEndReason.TargetDied);
                    return;
                }
            }

            if (_remain <= 0f) End(EffectEndReason.Expired);
        }

        private void End(EffectEndReason reason)
        {
            IsActive = false;
            EndReason = reason;
        }
    }
}
