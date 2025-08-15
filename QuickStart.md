# Equipment Location System - Quick Start Guide

## 5분 안에 시작하기

### 1. Unity 프로젝트 열기
```
1. Unity Hub 실행
2. "Open" 클릭
3. EquipmentLocation 폴더 선택
```

### 2. 씬 실행
```
1. Project 창에서 Assets > Scenes 폴더 열기
2. EquipmentLocationScene 더블클릭
3. Unity 상단의 Play 버튼 (▶) 클릭
```

### 3. 샘플 데이터 준비

`sample_data.csv` 파일 생성:
```csv
EquipmentID,Timestamp,X,Y,Z,EventType,Description
FORKLIFT01,0,0,0,0,,시작 위치
FORKLIFT01,10,10,0,5,,이동 중
FORKLIFT01,20,20,0,10,alarm,충돌 경고
FORKLIFT01,30,30,0,15,,
FORKLIFT01,40,35,0,20,,
FORKLIFT01,50,40,0,25,,목적지 도착
CRANE01,0,50,0,50,,크레인 시작
CRANE01,15,45,0,45,,
CRANE01,30,40,0,40,alarm,과부하 경고
CRANE01,45,35,0,35,,
CRANE01,60,30,0,30,,작업 완료
```

### 4. 데이터 로드 및 재생
```
1. Play 모드에서 "Load File" 버튼 클릭
2. 생성한 CSV 파일 선택
3. 타임라인 슬라이더로 시간 조정
4. Play 버튼으로 애니메이션 재생
```

## 주요 컨트롤

| 컨트롤 | 기능 | 단축키 |
|--------|------|--------|
| Load File 버튼 | CSV 파일 로드 | - |
| Play 버튼 | 재생 시작 | Space |
| Pause 버튼 | 일시정지 | Space |
| 타임라인 슬라이더 | 시간 이동 | 마우스 드래그 |

## 카메라 조작 (Scene View)

| 동작 | 방법 |
|------|------|
| 회전 | Alt + 마우스 왼쪽 드래그 |
| 이동 | 마우스 가운데 버튼 드래그 |
| 줌 | 마우스 휠 스크롤 |

## 빠른 커스터마이징

### 장비 색상 변경
1. Hierarchy에서 Equipment 오브젝트 선택
2. Inspector > Mesh Renderer > Materials
3. 색상 변경

### 재생 속도 조정
1. GameManager 선택
2. Inspector > Simple Location Controller
3. Playback Speed 값 변경 (기본: 10)

### 지형 크기 변경
1. Terrain 오브젝트 선택
2. Transform > Scale 조정

## 문제 발생 시

### 아무것도 보이지 않을 때
- Main Camera 위치 확인: (0, 10, -30)
- Directional Light 활성화 확인

### 버튼이 작동하지 않을 때
- EventSystem 오브젝트 존재 확인
- Play 모드인지 확인

### 장비가 나타나지 않을 때
- CSV 파일 형식 확인
- 콘솔 창에서 에러 메시지 확인

## 다음 단계

1. **더 많은 장비 추가**: CSV 파일에 새로운 EquipmentID 추가
2. **알람 설정**: EventType 열에 "alarm" 입력
3. **경로 시각화**: Trail Renderer 컴포넌트 추가
4. **UI 개선**: 장비 목록, 필터 기능 추가

## 유용한 팁

💡 **팁 1**: CSV 파일은 Excel에서 직접 편집 가능
💡 **팁 2**: 큰 데이터셋의 경우 재생 속도를 높여서 빠르게 확인
💡 **팁 3**: Scene 뷰와 Game 뷰를 나란히 배치하면 편리
💡 **팁 4**: 콘솔 창(Ctrl+Shift+C)에서 디버그 메시지 확인