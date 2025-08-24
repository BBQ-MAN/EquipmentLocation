# Equipment Location Tracking System

Unity 기반 위치 측위 데이터 재생 및 시각화 시스템

## 개요

이 애플리케이션은 CSV 파일에서 위치 측위 데이터를 읽어 Unity 3D 환경에서 시간에 따른 위치 변화를 시각화합니다. 실제 측정된 스마트폰 위치 데이터를 시간의 흐름대로 재생하며, 다양한 재생 제어 기능을 제공합니다.

## 주요 기능

- **다중 CSV 파일 로딩**: 여러 기기의 위치 데이터 동시 처리
- **시간 기반 재생 시스템**: 실제 타임스탬프 기반 정확한 재생
- **실시간 3D 시각화**: 위치 데이터의 3D 공간 표현
- **재생 제어**: 재생/일시정지/정지, 속도 조절 (0.25x ~ 8x)
- **타임라인 탐색**: 슬라이더를 통한 시간 이동
- **위치 보간**: 부드러운 움직임을 위한 데이터 보간
- **이동 궤적 표시**: Trail Renderer를 통한 경로 시각화
- **포인트별 라벨**: 위치 정보 실시간 표시

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
│   ├── DataModels/
│   │   ├── LocationData.cs             # 위치 데이터 모델
│   │   └── CSVDataLoader.cs            # CSV 데이터 로더
│   ├── Playback/
│   │   └── TimelinePlaybackController.cs # 시간 기반 재생 제어
│   ├── Visualization/
│   │   └── LocationVisualizer.cs       # 3D 위치 시각화
│   ├── UI/
│   │   └── PlaybackUIManager.cs        # UI 인터페이스 관리
│   └── PlaybackManager.cs              # 메인 시스템 매니저
├── Prefabs/
│   └── EquipmentPrefab.prefab          # 장비 표시용 프리팹
└── ../Documents/
    └── 위츠측위 테스트 데이터/
        └── Raw Data/
            ├── 위치측위 Raw Data(스마트폰_1).csv
            └── 위치측위 Raw Data(스마트폰_2).csv
```

## 데이터 형식

### CSV 파일 구조
```csv
DATE,HHMM,SEC,PTNUM,location (X) / m,location (Y) / m
20250717,1034,29946,1,1.75,2.3
20250717,1034,30925,1,1.366053623,1.073841422
20250717,1034,31952,1,1.56720418,1.709844
```

### 필드 설명
- **DATE**: 측정 날짜 (YYYYMMDD 형식)
- **HHMM**: 측정 시간 (HHMM 형식)
- **SEC**: 밀리초 단위 시간
- **PTNUM**: 측정 포인트 번호 (1~6)
- **location (X) / m**: X 좌표 (미터 단위)
- **location (Y) / m**: Y 좌표 (미터 단위)

## 사용 방법

### 1. 빠른 시작
1. Unity Editor에서 프로젝트 열기
2. 빈 GameObject 생성 → "PlaybackManager" 이름 설정
3. PlaybackManager.cs 스크립트 추가
4. Play 버튼 클릭 → 자동으로 CSV 데이터 로드 및 시각화

### 2. 재생 제어
- **재생/일시정지**: Space 키 또는 Play/Pause 버튼
- **정지**: Stop 버튼
- **속도 조절**: +/- 버튼 또는 슬라이더 (0.25x ~ 8x)
- **타임라인 탐색**: 타임라인 슬라이더로 특정 시간 이동

### 3. 시각화 옵션
- **궤적 표시**: Trail 토글로 이동 경로 표시/숨김
- **라벨 표시**: 포인트 번호와 좌표 정보 표시
- **보간 모드**: 부드러운 움직임 활성화/비활성화
- **루프 재생**: 자동 반복 재생 설정

## 핵심 컴포넌트

### LocationData.cs
위치 데이터 모델 클래스
```csharp
public class LocationData {
    public string date;           // 날짜 (YYYYMMDD)
    public string time;           // 시간 (HHMM)
    public int milliseconds;      // 밀리초
    public int pointNumber;       // 포인트 번호
    public float locationX;       // X 좌표 (m)
    public float locationY;       // Y 좌표 (m)
    public DateTime GetTimestamp() // 타임스탬프 계산
}
```

### CSVDataLoader.cs
CSV 파일 로딩 및 데이터 관리
- 다중 CSV 파일 동시 로드
- 시간순 자동 정렬 및 병합
- 특정 시간의 데이터 추출
- 위치 보간 기능

### TimelinePlaybackController.cs
시간 기반 재생 제어 시스템
- 재생/일시정지/정지 제어
- 속도 조절 (0.25x ~ 8x)
- 타임라인 스크러빙
- 루프 재생 및 보간 설정

### LocationVisualizer.cs
3D 공간 시각화 엔진
- 포인트별 색상 구분
- Trail Renderer를 통한 궤적 표시
- 실시간 라벨 업데이트
- 동적 오브젝트 생성 및 관리

### PlaybackUIManager.cs
사용자 인터페이스 관리
- 재생 컨트롤 버튼
- 속도 조절 UI
- 타임라인 슬라이더
- 데이터 정보 표시

### PlaybackManager.cs
시스템 통합 매니저 (싱글톤)
- 모든 컴포넌트 자동 설정
- 중앙 제어 인터페이스
- 이벤트 시스템 관리

## API 사용 예제

### 기본 사용법
```csharp
// 재생 시작
PlaybackManager.Instance.StartPlayback();

// 속도 설정
PlaybackManager.Instance.SetPlaybackSpeed(2.0f);

// 특정 시간으로 이동
PlaybackManager.Instance.SeekToTime(30f);
```

### 이벤트 구독
```csharp
// 재생 업데이트 이벤트
playbackController.OnPlaybackUpdate += (time, data) => {
    Debug.Log($"현재 시간: {time:F2}초");
};

// 데이터 로드 완료 이벤트
dataLoader.OnDataLoaded += () => {
    Debug.Log("데이터 로드 완료!");
};
```

### 시각화 설정
```csharp
// 궤적 표시 설정
visualizer.SetShowTrails(true);
visualizer.trailDuration = 10f;

// 포인트 색상 커스터마이징
visualizer.pointColors = new Color[] {
    Color.red, Color.blue, Color.green
};
```

## 문제 해결

### CSV 파일을 찾을 수 없음
```csharp
// CSVDataLoader.cs에서 경로 확인
csvFilePath1 = "Documents/위츠측위 테스트 데이터/Raw Data/위치측위 Raw Data(스마트폰_1).csv"
```

### 재생이 안 됨
- PlaybackManager의 `autoLoadDataOnStart = true` 확인
- 데이터가 로드되었는지 콘솔 로그 확인

### 포인트가 보이지 않음
- Scene 뷰에서 카메라 위치 조정
- LocationVisualizer의 `heightOffset` 값 조정 (기본: 0.5)
- `worldScale` 값 조정 (기본: 1.0)

## 성능 최적화

| 설정 | 성능 향상 | 영향 |
|------|----------|------|
| 보간 비활성화 | +20% | 움직임이 끊김 |
| Trail 시간 감소 | +15% | 짧은 궤적 |
| 라벨 비활성화 | +10% | 정보 표시 없음 |

## 확장 기능 아이디어

1. **실시간 데이터 스트리밍**: 네트워크를 통한 실시간 위치 업데이트
2. **다중 데이터 소스**: 더 많은 기기의 데이터 동시 처리
3. **고급 시각화**: 히트맵, 3D 경로, 속도 표시
4. **데이터 분석**: 통계, 패턴 분석, 이상 감지
5. **데이터 내보내기**: 분석 결과 CSV/JSON 저장

## 라이선스

이 프로젝트는 교육 및 연구 목적으로 자유롭게 사용 가능합니다.

## 기술 지원

문제가 발생하거나 기능 개선이 필요한 경우, Unity MCP를 통해 지원받을 수 있습니다.