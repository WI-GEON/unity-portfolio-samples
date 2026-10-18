namespace PortfolioSamples.StatusEffects
{
    /// <summary>
    /// 스턴: 몬스터의 Update에서 Boolean 상태로 기존 행동을 막는다.
    /// <code>
    /// void Update()
    /// {
    ///     if (_stun.Tick(Time.deltaTime)) return; // 스턴 중: 기존 행동 차단
    ///     RunBehaviour();                          // 끝나면 기존 행동 재개
    /// }
    /// </code>
    /// </summary>
    public sealed class StunState
    {
        private float _remain;

        public bool IsStunned { get; private set; }

        public void Begin(float duration)
        {
            if (duration <= 0f) return;
            // 이미 스턴 중이면 남은 시간이 더 긴 쪽을 유지한다.
            if (!IsStunned || duration > _remain) _remain = duration;
            IsStunned = true;
        }

        /// <returns>이번 프레임에 기존 행동을 막아야 하면 true</returns>
        public bool Tick(float deltaTime)
        {
            if (!IsStunned) return false;

            _remain -= deltaTime;
            if (_remain <= 0f)
            {
                IsStunned = false;
                _remain = 0f;
            }
            return true;
        }

        public void Clear()
        {
            IsStunned = false;
            _remain = 0f;
        }
    }
}
