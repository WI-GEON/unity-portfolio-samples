using System.Collections.Generic;

namespace PortfolioSamples.StatusEffects
{
    /// <summary>몬스터 한 마리가 가진 상태 이상들을 매 프레임 진행하고, 끝난 효과를 정리한다.</summary>
    public sealed class StatusEffectRunner
    {
        private readonly IEffectTarget _target;
        private readonly List<IStatusEffect> _effects = new List<IStatusEffect>();

        public StatusEffectRunner(IEffectTarget target)
        {
            _target = target;
        }

        public int ActiveCount => _effects.Count;

        public void Add(IStatusEffect effect)
        {
            if (_target.IsDead) return;
            effect.Apply(_target);
            _effects.Add(effect);
        }

        public void Tick(float deltaTime)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].Tick(_target, deltaTime);
                if (!_effects[i].IsActive) _effects.RemoveAt(i);
            }
        }
    }
}
