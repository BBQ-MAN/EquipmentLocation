# API Reference - Unity 위치 데이터 재생 시스템

## PlaybackManager

싱글톤 패턴의 메인 시스템 매니저

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Instance` | PlaybackManager | 싱글톤 인스턴스 |
| `IsPlaying` | bool | 현재 재생 상태 |
| `CurrentTime` | float | 현재 재생 시간 (초) |
| `TotalDuration` | float | 전체 재생 시간 |
| `NormalizedTime` | float | 정규화된 시간 (0~1) |
| `TotalDataPoints` | int | 총 데이터 포인트 수 |

### Methods

```csharp
public void StartPlayback()
```
재생을 시작합니다.

```csharp
public void PausePlayback()
```
재생을 일시정지합니다.

```csharp
public void StopPlayback()
```
재생을 정지하고 시간을 0으로 리셋합니다.

```csharp
public void ReloadData()
```
CSV 데이터를 다시 로드합니다.

```csharp
public void SetPlaybackSpeed(float speed)
```
재생 속도를 설정합니다.
- **Parameters**: `speed` - 재생 속도 (0.1 ~ 10.0)

```csharp
public void SeekToTime(float time)
```
특정 시간으로 이동합니다.
- **Parameters**: `time` - 이동할 시간 (초)

```csharp
public void SeekToNormalizedTime(float normalizedTime)
```
정규화된 시간으로 이동합니다.
- **Parameters**: `normalizedTime` - 0~1 범위의 정규화된 시간

---

## CSVDataLoader

CSV 파일 로딩 및 데이터 관리 클래스

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `smartphone1Data` | List<LocationData> | 스마트폰1 데이터 |
| `smartphone2Data` | List<LocationData> | 스마트폰2 데이터 |
| `allDataCombined` | List<LocationData> | 모든 데이터 (시간순 정렬) |
| `totalDuration` | float | 전체 시간 (초) |
| `startTime` | DateTime | 시작 시간 |
| `endTime` | DateTime | 종료 시간 |

### Events

```csharp
public event DataLoadedEvent OnDataLoaded
```
데이터 로드 완료 시 발생

### Methods

```csharp
public void LoadAllData()
```
모든 CSV 파일을 로드하고 정렬합니다.

```csharp
public List<LocationData> GetDataAtTime(float elapsedSeconds)
```
특정 시간의 데이터를 반환합니다.
- **Parameters**: `elapsedSeconds` - 경과 시간 (초)
- **Returns**: 해당 시간의 LocationData 리스트

```csharp
public LocationData GetInterpolatedDataForPoint(int pointNumber, float elapsedSeconds)
```
특정 포인트의 보간된 데이터를 반환합니다.
- **Parameters**: 
  - `pointNumber` - 포인트 번호
  - `elapsedSeconds` - 경과 시간 (초)
- **Returns**: 보간된 LocationData

---

## TimelinePlaybackController

시간 기반 재생 제어 시스템

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `isPlaying` | bool | 재생 중 여부 |
| `currentTime` | float | 현재 시간 (초) |
| `normalizedTime` | float | 정규화된 시간 (0~1) |
| `playbackSpeed` | float | 재생 속도 |
| `loop` | bool | 루프 재생 여부 |
| `interpolateMovement` | bool | 움직임 보간 여부 |

### Events

```csharp
public event PlaybackUpdateEvent OnPlaybackUpdate
```
재생 업데이트 시 발생
- **Parameters**: `(float currentTime, List<LocationData> currentData)`

```csharp
public event PlaybackStateChangedEvent OnPlaybackStateChanged
```
재생 상태 변경 시 발생
- **Parameters**: `(bool isPlaying)`

### Methods

```csharp
public void Play()
```
재생을 시작합니다.

```csharp
public void Pause()
```
재생을 일시정지합니다.

```csharp
public void Stop()
```
재생을 정지하고 시간을 리셋합니다.

```csharp
public void TogglePlayPause()
```
재생/일시정지를 토글합니다.

```csharp
public void SetPlaybackSpeed(float speed)
```
재생 속도를 설정합니다.
- **Parameters**: `speed` - 재생 속도 (0.1 ~ 10.0)

```csharp
public void IncreaseSpeed()
```
재생 속도를 증가시킵니다 (프리셋 값).

```csharp
public void DecreaseSpeed()
```
재생 속도를 감소시킵니다 (프리셋 값).

```csharp
public void SeekToTime(float time)
```
특정 시간으로 이동합니다.
- **Parameters**: `time` - 이동할 시간 (초)

```csharp
public void SeekToNormalizedTime(float normalizedTime)
```
정규화된 시간으로 이동합니다.
- **Parameters**: `normalizedTime` - 0~1 범위의 시간

```csharp
public void SetLoop(bool loop)
```
루프 재생을 설정합니다.
- **Parameters**: `loop` - 루프 재생 여부

```csharp
public void SetInterpolation(bool interpolate)
```
움직임 보간을 설정합니다.
- **Parameters**: `interpolate` - 보간 사용 여부

---

## LocationVisualizer

3D 공간 시각화 엔진

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `pointPrefab` | GameObject | 포인트 표시용 프리팹 |
| `heightOffset` | float | 높이 오프셋 |
| `worldScale` | float | 월드 스케일 |
| `pointColors` | Color[] | 포인트별 색상 배열 |
| `pointSize` | float | 포인트 크기 |
| `showTrails` | bool | 궤적 표시 여부 |
| `trailDuration` | float | 궤적 지속 시간 |
| `showLabels` | bool | 라벨 표시 여부 |

### Methods

```csharp
public void SetShowTrails(bool show)
```
궤적 표시를 설정합니다.
- **Parameters**: `show` - 표시 여부

```csharp
public void SetShowLabels(bool show)
```
라벨 표시를 설정합니다.
- **Parameters**: `show` - 표시 여부

```csharp
public void SetWorldScale(float scale)
```
월드 스케일을 설정합니다.
- **Parameters**: `scale` - 스케일 값

```csharp
public void ClearVisualization()
```
모든 시각화 오브젝트를 제거합니다.

```csharp
public void ClearTrails()
```
모든 궤적을 지웁니다.

---

## LocationData

위치 데이터 모델 클래스

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `date` | string | 날짜 (YYYYMMDD) |
| `time` | string | 시간 (HHMM) |
| `milliseconds` | int | 밀리초 |
| `pointNumber` | int | 포인트 번호 |
| `locationX` | float | X 좌표 (미터) |
| `locationY` | float | Y 좌표 (미터) |

### Methods

```csharp
public DateTime GetTimestamp()
```
완전한 타임스탬프를 반환합니다.
- **Returns**: DateTime 객체

```csharp
public float GetTimeInSeconds()
```
하루 시작부터의 시간을 초 단위로 반환합니다.
- **Returns**: 초 단위 시간

---

## PlaybackUIManager

UI 인터페이스 관리 클래스

### UI Elements

| Element | Type | Description |
|---------|------|-------------|
| `playButton` | Button | 재생 버튼 |
| `pauseButton` | Button | 일시정지 버튼 |
| `stopButton` | Button | 정지 버튼 |
| `speedUpButton` | Button | 속도 증가 버튼 |
| `speedDownButton` | Button | 속도 감소 버튼 |
| `speedSlider` | Slider | 속도 조절 슬라이더 |
| `timelineSlider` | Slider | 타임라인 슬라이더 |
| `loopToggle` | Toggle | 루프 재생 토글 |
| `interpolationToggle` | Toggle | 보간 토글 |
| `showTrailToggle` | Toggle | 궤적 표시 토글 |

### Display Elements

| Element | Type | Description |
|---------|------|-------------|
| `currentTimeText` | Text | 현재 시간 표시 |
| `totalTimeText` | Text | 전체 시간 표시 |
| `speedText` | Text | 재생 속도 표시 |
| `dataPointCountText` | Text | 데이터 포인트 수 표시 |
| `progressPercentageText` | Text | 진행률 표시 |

---

## 사용 예제

### 기본 재생 제어
```csharp
void Start()
{
    // 싱글톤 인스턴스 접근
    var manager = PlaybackManager.Instance;
    
    // 데이터 로드
    manager.ReloadData();
    
    // 재생 시작
    manager.StartPlayback();
    
    // 2배속 설정
    manager.SetPlaybackSpeed(2.0f);
}
```

### 이벤트 구독
```csharp
void Start()
{
    var controller = GetComponent<TimelinePlaybackController>();
    
    // 재생 업데이트 이벤트 구독
    controller.OnPlaybackUpdate += OnPlaybackUpdate;
    
    // 상태 변경 이벤트 구독
    controller.OnPlaybackStateChanged += OnStateChanged;
}

void OnPlaybackUpdate(float time, List<LocationData> data)
{
    Debug.Log($"Time: {time:F2}s, Points: {data.Count}");
}

void OnStateChanged(bool isPlaying)
{
    Debug.Log($"Playing: {isPlaying}");
}
```

### 시각화 커스터마이징
```csharp
void ConfigureVisualization()
{
    var visualizer = GetComponent<LocationVisualizer>();
    
    // 궤적 설정
    visualizer.SetShowTrails(true);
    visualizer.trailDuration = 5.0f;
    
    // 색상 설정
    visualizer.pointColors = new Color[] {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };
    
    // 스케일 조정
    visualizer.SetWorldScale(2.0f);
}
```

### 커스텀 데이터 처리
```csharp
void ProcessDataAtTime(float targetTime)
{
    var loader = GetComponent<CSVDataLoader>();
    
    // 특정 시간의 데이터 가져오기
    var dataAtTime = loader.GetDataAtTime(targetTime);
    
    foreach (var data in dataAtTime)
    {
        Debug.Log($"Point {data.pointNumber}: ({data.locationX}, {data.locationY})");
    }
    
    // 보간된 데이터 가져오기
    var interpolated = loader.GetInterpolatedDataForPoint(1, targetTime);
    if (interpolated != null)
    {
        Debug.Log($"Interpolated: ({interpolated.locationX}, {interpolated.locationY})");
    }
}
```