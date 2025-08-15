# Equipment Location Tracking System

Unity 기반 장비 위치 추적 및 시각화 애플리케이션

## 개요

이 애플리케이션은 Excel/CSV 파일에서 장비 위치 데이터를 읽어 Unity 3D 환경에서 시간에 따른 장비 이동을 시각화합니다. 타임라인 기반 재생 기능과 알람 표시 기능을 제공합니다.

## 주요 기능

- **CSV/Excel 파일 로딩**: 장비 위치 데이터를 파일에서 읽기
- **3D 지형 시각화**: 지형 위에 장비 위치 표시
- **시간 기반 재생**: 타임라인 슬라이더를 통한 시간 제어
- **부드러운 이동 애니메이션**: 위치 간 자연스러운 보간
- **알람 마커**: 타임라인에 중요 이벤트 표시

## 시스템 요구사항

- Unity 2021.3 LTS 이상
- .NET Framework 4.7.1 이상
- Windows/Mac/Linux 지원

## 프로젝트 구조

```
Assets/
├── Scenes/
│   └── EquipmentLocationScene.unity    # 메인 씬
├── Scripts/
│   ├── EquipmentData.cs                # 장비 데이터 모델
│   ├── ExcelFileReader.cs              # CSV 파일 파서
│   ├── SimpleLocationController.cs     # 메인 컨트롤러
│   ├── EquipmentMovementController.cs  # 이동 보간 시스템
│   └── AlarmMarkerSystem.cs           # 알람 마커 관리
└── Prefabs/
    └── EquipmentPrefab.prefab          # 장비 표시용 프리팹
```

## 데이터 형식

### CSV 파일 구조
```csv
EquipmentID,Timestamp,X,Y,Z,EventType,Description
EQ001,0,10,0,10,,
EQ001,10,20,0,15,,
EQ001,20,30,0,20,alarm,Equipment Alert
EQ002,0,5,0,5,,
EQ002,10,15,0,10,,
```

### 필드 설명
- **EquipmentID**: 장비 고유 식별자
- **Timestamp**: 시간 (초 단위)
- **X, Y, Z**: 3D 공간 좌표
- **EventType**: 이벤트 유형 (옵션, "alarm" 등)
- **Description**: 이벤트 설명 (옵션)

## 사용 방법

### 1. 씬 실행
1. Unity Editor에서 프로젝트 열기
2. `Assets/Scenes/EquipmentLocationScene` 씬 로드
3. Play 버튼 클릭

### 2. 데이터 로드
1. "Load File" 버튼 클릭
2. CSV 파일 선택
3. 데이터가 자동으로 로드되고 정렬됨

### 3. 재생 제어
- **타임라인 슬라이더**: 특정 시간으로 이동
- **Play 버튼**: 실시간 재생 시작
- **Pause 버튼**: 재생 일시정지
- **재생 속도**: SimpleLocationController의 playbackSpeed 조정

## 스크립트 상세

### EquipmentData.cs
```csharp
[System.Serializable]
public class EquipmentData
{
    public string equipmentId;    // 장비 ID
    public float timestamp;        // 시간
    public Vector3 position;       // 위치
    public string eventType;       // 이벤트 유형
    public string description;     // 설명
}
```

### ExcelFileReader.cs
CSV 파일을 파싱하여 EquipmentData 리스트로 변환
- 자동 시간순 정렬
- 에러 처리 포함

### SimpleLocationController.cs
메인 컨트롤러 - 전체 시스템 관리
- UI 이벤트 처리
- 타임라인 제어
- 장비 생성 및 업데이트

### EquipmentMovementController.cs
개별 장비의 부드러운 이동 처리
- 선형 보간 (Lerp)
- 지형 높이 맞춤
- 이동 방향 회전

### AlarmMarkerSystem.cs
타임라인 알람 마커 관리
- 동적 마커 생성
- 이벤트 시각화

## 커스터마이징

### 장비 모양 변경
1. `Assets/Prefabs/EquipmentPrefab` 수정
2. 메시, 머티리얼, 크기 조정

### 지형 설정
1. Terrain 오브젝트 선택
2. Terrain 컴포넌트에서 높이맵, 텍스처 수정

### UI 스타일
1. UICanvas 하위 오브젝트 수정
2. 버튼, 슬라이더 스타일 변경

## 문제 해결

### 카메라가 보이지 않음
- Main Camera 오브젝트 확인
- 위치: (0, 10, -30)
- 태그: MainCamera

### UI 버튼이 작동하지 않음
- EventSystem 오브젝트 확인
- StandaloneInputModule 컴포넌트 필요

### 파일 로드 실패
- CSV 파일 형식 확인
- 헤더 라인 필수
- UTF-8 인코딩 권장

## 확장 기능 아이디어

1. **실시간 데이터 스트리밍**: 네트워크를 통한 실시간 위치 업데이트
2. **다중 장비 필터링**: 특정 장비만 표시/숨기기
3. **경로 시각화**: 이동 경로 라인 렌더링
4. **3D 카메라 컨트롤**: 자유로운 카메라 이동
5. **데이터 내보내기**: 분석 결과 CSV 저장

## 라이선스

이 프로젝트는 교육 및 연구 목적으로 자유롭게 사용 가능합니다.

## 기술 지원

문제가 발생하거나 기능 개선이 필요한 경우, Unity MCP를 통해 지원받을 수 있습니다.