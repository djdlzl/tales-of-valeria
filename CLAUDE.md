# 테일즈오브발레리아 (Tales of Valeria) — 프로젝트 컨텍스트

이 파일은 새 세션 시작 시 자동으로 읽힘.
작업 전 반드시 현재 단계와 다음 할 일을 확인할 것.

---

## 현재 단계

**2단계 프로토타입 — Unity Editor 씬 구성 필요 (수동 작업 대기 중)**

---

## 다음 할 일

> 코드는 모두 완성. Unity Editor에서 아래 순서대로 씬을 구성해야 함.

1. **플레이어 오브젝트** 생성
   - 컴포넌트: NavMeshAgent, PlayerController, PlayerCombat
   - 태그: `Player`
2. **적 오브젝트** 생성
   - 컴포넌트: NavMeshAgent, EnemyHealth, EnemyAI
   - 레이어: `Enemy`
3. **지면(Ground)** 레이어: `Ground` 설정
4. **NavMesh Bake** (Window → AI → Navigation → Bake)
5. **카메라** IsometricCamera 연결, Target = 플레이어
6. Play 버튼으로 테스트 — "클릭 이동 + 적 전투"가 되면 2단계 완료

---

## 완료된 작업 (최근순)

- ✅ EnemyAI.cs — 적 AI FSM (감지 → 추적 → 공격 → 복귀)
- ✅ PlayerCombat.cs — 플레이어 자동 공격 시스템
- ✅ EnemyHealth.cs — 적 HP + 사망 처리
- ✅ PlayerController.cs — 클릭 이동 + 타겟 지정
- ✅ IsometricCamera.cs — 쿼터뷰 카메라 추적
- ✅ GitHub 원격 저장소 연동

---

## 프로젝트 구조

```
workspace/game/               ← 게임 카테고리 폴더
└── Tales of Valeria/         ← Unity 프로젝트 루트 (여기서 Claude 실행)
    ├── CLAUDE.md             ← 이 파일
    ├── GAME_DESIGN.md
    ├── docs/
    └── Assets/Scripts/Camera/IsometricCamera.cs
```

---

## 핵심 문서 위치

| 문서 | 경로 | 용도 |
|------|------|------|
| 게임 요약 | `GAME_DESIGN.md` | 항상 참고 |
| 상세 설계 | `docs/plans/2026-03-09-valeria-game-design.md` | 시스템 설계 전체 |
| 개발 순서 | `docs/DEVELOPMENT_ROADMAP.md` | 단계별 진행 상황 |

---

## 기술 스택

- **엔진**: Unity (C#)
- **플랫폼**: PC + 콘솔
- **시점**: 쿼터뷰 아이소메트릭
- **개발**: 1인 + AI 에이전트

---

## 게임 한 줄 요약

빅토리아 판타지 오픈월드 RPG. 핵심 철학: "사랑이 승리한다."
부패한 권위(부유성·아크론교)에 맞서 해방을 쟁취하는 이야기.
