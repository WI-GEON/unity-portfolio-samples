using PortfolioSamples.StatusEffects;
using UnityEngine;

namespace PortfolioSamples.Unity
{
    /// <summary>
    /// 순수 C# 상태 이상 로직을 몬스터 MonoBehaviour에 연결하는 예시.
    /// 로직은 Runtime 쪽에 있고, 여기서는 프레임 시간만 넘긴다.
    /// </summary>
    public sealed class MonsterStatusBehaviour : MonoBehaviour, IEffectTarget
    {
        [SerializeField] private float _baseSpeed = 3f;
        [SerializeField] private int _hp = 30;

        private StatusEffectRunner _effects;
        private readonly StunState _stun = new StunState();

        public bool IsDead => _hp <= 0;
        public bool CanMove { get; set; } = true;
        public bool DealsContactDamage { get; set; } = true;
        public float BaseSpeed => _baseSpeed;
        public float SpeedModifier { get; set; }

        private void Awake() => _effects = new StatusEffectRunner(this);

        public void Freeze(float seconds) => _effects.Add(new FreezeEffect(seconds));
        public void Slow(float seconds, float ratio) => _effects.Add(new SlowEffect(seconds, ratio));
        public void Ignite(float seconds, float interval, int damage) => _effects.Add(new FireDotEffect(seconds, interval, damage));
        public void Stun(float seconds) => _stun.Begin(seconds);

        public void TakeDamage(int amount)
        {
            if (IsDead) return;
            _hp -= amount;
            if (IsDead) _stun.Clear();
        }

        private void Update()
        {
            _effects.Tick(Time.deltaTime);

            if (IsDead) return;
            if (_stun.Tick(Time.deltaTime)) return;   // 스턴 중: 기존 행동 차단
            if (!CanMove) return;                     // 빙결 중: 이동 제한

            RunBehaviour(_baseSpeed + SpeedModifier);
        }

        private void RunBehaviour(float speed)
        {
            transform.position += Vector3.left * (speed * Time.deltaTime);
        }
    }
}
