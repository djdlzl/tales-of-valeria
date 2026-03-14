# 테일즈오브발레리아 — 진행 상황 & 결정 기록

> 틈틈이 개발하므로, 언제든 이 파일을 보면 현재 상태와 이전 결정을 파악할 수 있음.
> 새 세션 시작 시 이 파일부터 확인할 것.

---

## 현재 위치

```
[✅ 0단계: 기획] → [✅ 1단계: 환경 세팅] → [✅ 2단계: 프로토타입] → [🔄 3단계: 핵심 시스템] → ...
```

**3단계 진행 중** — 6개 서브시스템 중 첫 번째(숙련도) 구현 중

---

## 3단계 서브시스템 진행 순서 (결정됨: A안)

```
🔄 1. 숙련도 시스템   ← 현재 여기
   2. 역할 자동 배정
   3. 스킬 트리
   4. 인벤토리
   5. NPC 대화
   6. 용병 길드 UI
```

---

## 서브시스템 1: 숙련도 시스템

### 결정 사항

| 항목 | 결정 | 대안 중 선택 |
|------|------|-------------|
| 무기 범위 | 검(근접) + 활(원거리) 2개만 | A안 — 최소로 빠르게 검증 |
| 수치 방식 | 경험치 풀 (레벨업 필요량 점증) | B안 — RPG답고 성장 곡선 조절 가능 |
| 역할 배정 | 임계값 방식 (Lv.3 이상 → 자격 획득) | B안 — 2차 전직과 자연스럽게 연결 |
| 데이터 관리 | ScriptableObject로 분리 | C안 — Inspector에서 밸런싱 조절 |

### 레벨 커브 (기본값, Inspector에서 조절 가능)

| 레벨 | 필요 경험치 | 누적 | 의미 |
|------|-----------|------|------|
| Lv.1 | 0 (시작) | 0 | 초보 |
| Lv.2 | 100 | 100 | 기초 습득 |
| Lv.3 | 250 | 350 | **역할 자격 획득** |
| Lv.4 | 500 | 850 | 숙련 |
| Lv.5 | 1000 | 1850 | **2차 전직 분기점** |

### 경험치 획득량 (기본값)
- 공격 1회: +5
- 적 처치: +20

### 구현 완료 파일

| 파일 | 상태 | 설명 |
|------|------|------|
| `Assets/Scripts/Proficiency/WeaponType.cs` | ✅ | 무기 타입 열거형 |
| `Assets/Scripts/Proficiency/ProficiencyConfig.cs` | ✅ | ScriptableObject 설정 |
| `Assets/Scripts/Proficiency/ProficiencySystem.cs` | ✅ | 경험치 추적, 레벨업, 역할 판정 |
| `Assets/Scripts/Proficiency/ProficiencyDebugUI.cs` | ✅ | 디버그 UI (좌상단 실시간 표시) |
| `Assets/Scripts/Character/PlayerCombat.cs` | ✅ 수정 | 숙련도 연동 (공격·처치 시 경험치) |
| `Assets/Scripts/Enemy/EnemyHealth.cs` | ✅ 수정 | OnDeath 이벤트 추가 |

### Unity에서 해야 할 것 (수동)

- [ ] ProficiencyConfig 에셋 생성: Project 우클릭 → Create → Valeria → ProficiencyConfig
- [ ] Player에 `ProficiencySystem` 컴포넌트 추가
- [ ] Player에 `ProficiencyDebugUI` 컴포넌트 추가
- [ ] ProficiencySystem의 Config 필드에 에셋 연결
- [ ] Play → 적 공격 → 좌상단에 숙련도 레벨업 확인

### 다음 할 일

- 숙련도 시스템 플레이테스트 후 밸런싱 조절
- **서브시스템 2: 역할 자동 배정** 설계 및 구현 착수

---

## 2단계: 프로토타입 (완료)

### 구현 완료

| 파일 | 설명 |
|------|------|
| `Assets/Scripts/Camera/IsometricCamera.cs` | 쿼터뷰 카메라 (45°+45°, 정투영) |
| `Assets/Scripts/Character/PlayerController.cs` | 클릭 이동 + 적 타겟 지정 |
| `Assets/Scripts/Character/PlayerCombat.cs` | 자동 공격 (범위 내 자동, 쿨다운) |
| `Assets/Scripts/Enemy/EnemyAI.cs` | 상태 머신 AI (Idle→Chase→Attack→Return) |
| `Assets/Scripts/Enemy/EnemyHealth.cs` | HP + 사망 처리 |

---

## 기획 핵심 참고

- **게임 철학**: "사랑이 승리한다"
- **7대 역할**: 검사, 마법사, 엔지니어, 궁수, 약제사, 척후, 전령
- **역할 배정**: 선택이 아닌 행동 기반 자동 배정
- **2차 전직**: 숙련도 Lv.5에서 분기
- **상세 설계**: `docs/plans/2026-03-09-valeria-game-design.md`
