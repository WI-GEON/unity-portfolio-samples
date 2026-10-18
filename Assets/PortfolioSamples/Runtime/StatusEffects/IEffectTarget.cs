namespace PortfolioSamples.StatusEffects
{
    /// <summary>
    /// 상태 이상이 영향을 주는 몬스터 쪽 계약.
    /// MonoBehaviour와 분리해 두어 EditMode 테스트에서 가짜 대상으로 검증할 수 있다.
    /// </summary>
    public interface IEffectTarget
    {
        bool IsDead { get; }
        bool CanMove { get; set; }
        bool DealsContactDamage { get; set; }
        float BaseSpeed { get; }
        float SpeedModifier { get; set; }
        void TakeDamage(int amount);
    }
}
