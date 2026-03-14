# 테일즈오브발레리아 (Tales of Valeria) — 프로젝트 컨텍스트

이 파일은 새 세션 시작 시 자동으로 읽힘.
작업 전 반드시 현재 단계와 다음 할 일을 확인할 것.

---

## 현재 단계

**3단계 핵심 시스템 구현 중** — 숙련도 시스템 코드 완료, Unity 연결 대기

---

## 다음 할 일

1. Unity에서 ProficiencyConfig 에셋 생성 + Player에 컴포넌트 연결 (수동)
2. 플레이테스트 후 숙련도 밸런싱 조절
3. 서브시스템 2: 역할 자동 배정 설계 착수

---

## 완료된 작업 (최근순)

- ✅ 숙련도 시스템 (ProficiencySystem, Config, DebugUI)
- ✅ EnemyAI.cs — 적 AI FSM (감지 → 추적 → 공격 → 복귀)
- ✅ PlayerCombat.cs — 플레이어 자동 공격 시스템
- ✅ EnemyHealth.cs — 적 HP + 사망 처리
- ✅ PlayerController.cs — 클릭 이동 + 타겟 지정
- ✅ IsometricCamera.cs — 쿼터뷰 카메라 추적
- ✅ GitHub 원격 저장소 연동

---

## 프로젝트 구조

```
Tales of Valeria/
├── CLAUDE.md
├── GAME_DESIGN.md
├── docs/
│   ├── PROGRESS.md              ← ★ 진행상황 & 결정 기록 (세션 시작 시 필독)
│   ├── DEVELOPMENT_ROADMAP.md
│   └── plans/
└── Assets/Scripts/
    ├── Camera/IsometricCamera.cs
    ├── Character/PlayerController.cs, PlayerCombat.cs
    ├── Enemy/EnemyAI.cs, EnemyHealth.cs
    └── Proficiency/WeaponType.cs, ProficiencyConfig.cs,
                    ProficiencySystem.cs, ProficiencyDebugUI.cs
```

---

## 핵심 문서 위치

| 문서 | 경로 | 용도 |
|------|------|------|
| 게임 요약 | `GAME_DESIGN.md` | 항상 참고 |
| 상세 설계 | `docs/plans/2026-03-09-valeria-game-design.md` | 시스템 설계 전체 |
| 개발 순서 | `docs/DEVELOPMENT_ROADMAP.md` | 단계별 로드맵 |
| **진행상황** | `docs/PROGRESS.md` | **세션 재개 시 필독 — 결정 사항 + 현재 상태** |

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
