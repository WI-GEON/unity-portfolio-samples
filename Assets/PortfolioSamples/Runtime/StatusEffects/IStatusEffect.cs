namespace PortfolioSamples.StatusEffects
{
    public enum EffectEndReason
    {
        None,
        Expired,     // 지속 시간이 끝남 → 원래 상태로 되돌림
        TargetDied   // 대상이 죽음 → 행동을 재개시키지 않음
    }

    public interface IStatusEffect
    {
        bool IsActive { get; }
        EffectEndReason EndReason { get; }
        void Apply(IEffectTarget target);
        void Tick(IEffectTarget target, float deltaTime);
    }
}
