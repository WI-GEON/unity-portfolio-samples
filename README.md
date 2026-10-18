# Unity Client Portfolio · Code Samples

위건 포트폴리오에서 설명한 기능을 **일반적인 Unity · C# 방식으로 처음부터 다시 구현한 예시 코드**입니다.
설명한 동작이 그대로 움직이는지 **EditMode 테스트 42개**로 고정해 두었습니다.

> 회사 · 팀 프로젝트의 원본 코드, 내부 문서, 데이터를 옮기거나 복원한 것이 아닙니다.
> 포트폴리오에 적은 동작 설명만으로 새로 작성한 **독립 구현**이며, 클래스 구조와 이름은 원본과 관계없습니다.
> 코드에 나오는 수치는 출시된 게임 화면에 공개된 값만 사용했습니다.

<br>

## 모듈과 포트폴리오 연결

| 폴더 | 내용 | 포트폴리오 | 테스트 |
|---|---|---|---|
| `StatusEffects` | 빙결 · 스턴 · 둔화 · 화염 지속 피해, 효과 실행기 | Pixel Gun Master p.30–31 | 9 |
| `Perks` | 효과 인터페이스 3종, 6슬롯 장착, 성향 계산, 결정 후 일괄 적용, 저장 데이터 | Pixel Gun Master p.32–33 | 13 |
| `RoomSync` | 방 상태 결과 메시지, 스냅샷 적용과 검증, REST · 실시간 로그 | 숫자전쟁 p.25–27 | 6 |
| `Hangul` | 유니코드 음절 조합 · 분해, 자모 결합 규칙, 논리 그리드 병합 | 케이팝 천지인 p.18 | 10 |
| `StageData` | CSV 사전 검증 후에만 스테이지 데이터 갱신 | 케이팝 천지인 p.19–20 | 4 |

<br>

## 설계 포인트

### StatusEffects — 끝나는 이유를 구분한다
- 효과마다 `Apply` / `Tick`과 **종료 사유**(`Expired` · `TargetDied`)를 가진다.
- **빙결**은 시간이 끝나면 다시 움직이지만, 대상이 죽어서 끝나면 행동을 재개하지 않는다.
- **둔화**는 적용한 감소분을 기억했다가 그만큼만 되돌려, 다른 효과가 바꾼 속도를 덮어쓰지 않는다.
- **화염 지속 피해**는 `deltaTime`을 누적하고, 프레임이 길어도 놓친 틱을 모두 처리한다.
- **스턴**은 몬스터 `Update`에서 Boolean 상태로 기존 행동을 막는 방식이다 (`StunState`).

### Perks — 적용하는 쪽은 Perk 종류를 모른다
```csharp
public interface IStatPerk   { void ApplyStats(PlayerStats stats); }
public interface IOnHitPerk  { void OnHit(HitContext hit); }
public interface IAttackPerk { void ModifyAttack(AttackSpec attack); }
```
- Perk는 필요한 인터페이스만 구현하고, `PerkApplier`는 인터페이스로만 분기한다. 새 Perk는 클래스 하나만 추가하면 된다.
- 장착 · 해제는 **미리보기만** 바꾸고, 능력치는 「결정」 후 플레이어가 생성될 때 **한 번만** 적용한다.
- 성향은 비용 합이 아니라 **기사 계열 수 − 몬스터 · 수인 계열 수**로 계산하고, 단계는 최대 3이다.
- 저장하는 것은 **해금 여부와 장착 ID 목록**뿐이다. 효과 수치는 Perk 데이터에 둔다.

### RoomSync — 일부만 고치지 않고, 통째로 바꾼다
- 결과 메시지에는 **현재 방의 인원과 플레이어 정보 전체**를 담는다.
- 클라이언트는 결과를 **전체 스냅샷**으로 받아 슬롯을 통째로 교체하므로, 나간 플레이어가 남지 않는다.
- 정보가 빠졌거나 인원 수가 맞지 않는 결과는 **적용하지 않고 이전 상태를 유지**하며, 이유를 로그로 남긴다.
- REST 응답과 실시간 수신을 같은 형식으로 기록해, 서버 로그와 시간순으로 맞춰 볼 수 있게 한다.

### Hangul — 문자 조합과 그리드 배치를 분리한다
- 음절 = `0xAC00 + (초성 × 21 + 중성) × 28 + 종성` — `Compose(6, 8, 16) == '몸'`
- `HangulRule`은 글자 조합만, `GridBoard`는 배치와 병합만 맡는다. 병합에 실패하면 아무것도 바꾸지 않는다.

### StageData — 검증을 통과해야 반영한다
- CSV를 끝까지 읽으며 형식 · 위치 오류를 **행 번호와 함께 모두** 모은다.
- 오류가 하나라도 있으면 저장소를 **전혀 건드리지 않는다**.

<br>

## 폴더 구조

```
Assets/PortfolioSamples/
├─ Runtime/                  순수 C# (UnityEngine 참조 없음) · PortfolioSamples.asmdef
│  ├─ StatusEffects/
│  ├─ Perks/
│  ├─ RoomSync/
│  ├─ Hangul/
│  └─ StageData/
├─ Unity/                    MonoBehaviour · JsonUtility 연결 예시 · PortfolioSamples.Unity.asmdef
└─ Tests/EditMode/           NUnit 테스트 42개 · PortfolioSamples.Tests.asmdef
```

핵심 로직을 UnityEngine에서 분리해, 에디터를 실행하지 않고도 EditMode 테스트로 빠르게 검증할 수 있게 했습니다.

<br>

## 실행 방법

1. Unity **2022.3 LTS 이상**으로 새 프로젝트를 만듭니다.
2. 이 저장소의 `Assets/PortfolioSamples` 폴더를 프로젝트의 `Assets` 아래에 복사합니다.
3. **Window → General → Test Runner → EditMode → Run All**

<br>

## 검증 기록

| 날짜 | 환경 | 결과 |
|---|---|---|
| 2026-10-18 | Mono C# 컴파일러 · NUnit 3.13.3 (NUnitLite) | `Runtime` + `Tests` 42 / 42 Passed |

- `Unity/` 폴더의 연결 예시는 UnityEngine이 필요해 위 환경에서는 컴파일하지 않았습니다.

<br>

## 작성 방식

- 동작 기준(무엇이 어떻게 끝나야 하는지)은 포트폴리오에 적은 설명에 맞췄습니다.
- 코드와 테스트 초안 작성에는 생성형 AI를 활용했고, 동작 기준과 결과 검토는 본인이 했습니다.

<br>

---

**위건 (WI GEON)** · Unity 게임 클라이언트 프로그래머 · wegeon.dev@gmail.com
