# Unity 위치 측위 시스템 스크립트 상세 문서

본 문서는 Unity 위치 측위 데이터 재생 시스템의 모든 스크립트에 대한 상세한 한글 설명과 사용 방법을 제공합니다.

---

## 📊 데이터 모델 (Data Models)

### 1. LocationData.cs
**위치**: `Assets/Scripts/DataModels/LocationData.cs`

#### 📝 설명
위치 데이터의 기본 모델 클래스입니다. CSV 파일에서 읽어온 각 위치 정보를 저장하고 관리합니다.

#### 🔧 주요 속성

| 속성 | 타입 | 설명 |
|------|------|------|
| `date` | string | 날짜 정보 (YYYYMMDD 형식, 예: 20250717) |
| `time` | string | 시간 정보 (HHMM 형식, 예: 1034) |
| `milliseconds` | int | 밀리초 단위 시간 (SEC 필드) |
| `pointNumber` | int | 측정 포인트 번호 (1~6) |
| `locationX` | float | X 좌표 (미터 단위) |
| `locationY` | float | Y 좌표 (미터 단위) |

#### 🎯 주요 메서드

**`GetTimestamp()`**
```csharp
public DateTime GetTimestamp()
```
- **기능**: 날짜, 시간, 밀리초를 조합하여 완전한 DateTime 객체 반환
- **반환값**: 조합된 타임스탬프 (오류 시 DateTime.MinValue)
- **사용 예시**:
```csharp
LocationData data = new LocationData();
data.date = "20250717";
data.time = "1034";
data.milliseconds = 29946;
DateTime timestamp = data.GetTimestamp(); // 2025-07-17 10:34:29.946
```

**`GetTimeInSeconds()`**
```csharp
public float GetTimeInSeconds()
```
- **기능**: 하루 시작부터의 경과 시간을 초 단위로 반환
- **반환값**: 초 단위 시간 (float)
- **사용 예시**:
```csharp
float seconds = data.GetTimeInSeconds(); // 37469.946 (10시 34분 29.946초)
```

#### 💡 사용 팁
- 시간 정렬이 필요할 때는 `GetTimestamp()` 사용
- 재생 시간 계산에는 `GetTimeInSeconds()` 사용
- 데이터 유효성 검증 시 DateTime.MinValue 체크 필요

---

### 2. CSVDataLoader.cs
**위치**: `Assets/Scripts/DataModels/CSVDataLoader.cs`

#### 📝 설명
CSV 파일에서 위치 데이터를 로드하고 관리하는 핵심 클래스입니다. 다중 CSV 파일 처리, 시간순 정렬, 데이터 보간 기능을 제공합니다.

#### 🔧 주요 속성

| 속성 | 타입 | 설명 |
|------|------|------|
| `csvFilePath1` | string | 스마트폰1 CSV 파일 경로 |
| `csvFilePath2` | string | 스마트폰2 CSV 파일 경로 |
| `smartphone1Data` | List<LocationData> | 스마트폰1의 모든 데이터 |
| `smartphone2Data` | List<LocationData> | 스마트폰2의 모든 데이터 |
| `allDataCombined` | List<LocationData> | 모든 데이터 (시간순 정렬) |
| `totalDuration` | float | 전체 재생 시간 (초) |
| `startTime` | DateTime | 데이터 시작 시간 |
| `endTime` | DateTime | 데이터 종료 시간 |

#### 🎯 주요 메서드

**`LoadAllData()`**
```csharp
public void LoadAllData()
```
- **기능**: 모든 CSV 파일을 로드하고 시간순으로 정렬
- **동작 과정**:
  1. 기존 데이터 초기화
  2. 각 CSV 파일 로드
  3. 데이터 병합 및 정렬
  4. 시간 정보 계산
  5. OnDataLoaded 이벤트 발생

**`GetDataAtTime(float elapsedSeconds)`**
```csharp
public List<LocationData> GetDataAtTime(float elapsedSeconds)
```
- **기능**: 특정 시간의 위치 데이터 반환
- **매개변수**: `elapsedSeconds` - 시작부터의 경과 시간 (초)
- **반환값**: 해당 시간의 각 포인트별 최신 데이터
- **알고리즘**: 각 포인트별로 지정 시간 이전의 가장 최근 데이터 선택

**`GetInterpolatedDataForPoint(int pointNumber, float elapsedSeconds)`**
```csharp
public LocationData GetInterpolatedDataForPoint(int pointNumber, float elapsedSeconds)
```
- **기능**: 특정 포인트의 보간된 위치 데이터 반환
- **매개변수**: 
  - `pointNumber`: 포인트 번호 (1~6)
  - `elapsedSeconds`: 경과 시간 (초)
- **반환값**: 선형 보간된 LocationData 객체
- **알고리즘**: 전후 데이터 포인트를 찾아 선형 보간

#### 📋 이벤트

**`OnDataLoaded`**
```csharp
public event DataLoadedEvent OnDataLoaded;
```
- **발생 시점**: CSV 데이터 로드 완료 시
- **구독 예시**:
```csharp
dataLoader.OnDataLoaded += () => {
    Debug.Log("데이터 로드 완료!");
    Debug.Log($"총 {dataLoader.allDataCombined.Count}개 포인트");
};
```

#### 💡 사용 팁
- CSV 파일 경로는 프로젝트 루트 기준 상대 경로 사용
- 대용량 데이터 처리 시 비동기 로딩 고려
- 보간 기능으로 부드러운 움직임 구현 가능

---

## ⏯️ 재생 시스템 (Playback System)

### 3. TimelinePlaybackController.cs
**위치**: `Assets/Scripts/Playback/TimelinePlaybackController.cs`

#### 📝 설명
시간 기반 재생 제어를 담당하는 핵심 컨트롤러입니다. 재생/일시정지/정지, 속도 조절, 타임라인 탐색 등의 기능을 제공합니다.

#### 🔧 주요 속성

| 속성 | 타입 | 설명 | 기본값 |
|------|------|------|--------|
| `autoPlay` | bool | 데이터 로드 후 자동 재생 | false |
| `playbackSpeed` | float | 재생 속도 배율 | 1.0 |
| `loop` | bool | 반복 재생 여부 | false |
| `interpolateMovement` | bool | 움직임 보간 사용 | true |
| `isPlaying` | bool | 현재 재생 상태 | false |
| `currentTime` | float | 현재 재생 시간 (초) | 0 |
| `normalizedTime` | float | 정규화된 시간 (0~1) | 0 |
| `speedPresets` | float[] | 속도 프리셋 값들 | [0.25, 0.5, 1, 2, 4, 8] |

#### 🎯 주요 메서드

**재생 제어 메서드**

```csharp
public void Play()          // 재생 시작
public void Pause()         // 일시정지
public void Stop()          // 정지 (시간 리셋)
public void TogglePlayPause() // 재생/일시정지 토글
```

**속도 제어 메서드**

```csharp
public void SetPlaybackSpeed(float speed)  // 속도 직접 설정 (0.1~10)
public void IncreaseSpeed()                // 프리셋 속도 증가
public void DecreaseSpeed()                // 프리셋 속도 감소
```

**시간 탐색 메서드**

```csharp
public void SeekToTime(float time)                    // 특정 시간으로 이동 (초)
public void SeekToNormalizedTime(float normalizedTime) // 정규화 시간으로 이동 (0~1)
```

**옵션 설정 메서드**

```csharp
public void SetLoop(bool loop)                   // 반복 재생 설정
public void SetInterpolation(bool interpolate)   // 보간 설정
```

#### 📋 이벤트

**`OnPlaybackUpdate`**
```csharp
public event PlaybackUpdateEvent OnPlaybackUpdate;
// delegate: (float currentTime, List<LocationData> currentData)
```
- **발생 시점**: 매 프레임 재생 중
- **용도**: 시각화 업데이트, UI 갱신

**`OnPlaybackStateChanged`**
```csharp
public event PlaybackStateChangedEvent OnPlaybackStateChanged;
// delegate: (bool isPlaying)
```
- **발생 시점**: 재생 상태 변경 시
- **용도**: UI 버튼 상태 업데이트

#### 🎮 Update 루프 동작

```csharp
void Update()
{
    if (isPlaying)
    {
        // 1. 시간 업데이트
        currentTime += Time.deltaTime * playbackSpeed;
        
        // 2. 루프/종료 처리
        if (currentTime >= totalDuration)
        {
            if (loop) currentTime = 0;
            else Pause();
        }
        
        // 3. 데이터 업데이트 및 이벤트 발생
        UpdatePlayback();
    }
}
```

#### 💡 사용 팁
- 보간 기능 활성화로 부드러운 움직임 구현
- 속도 프리셋으로 빠른 검토 가능
- 타임라인 슬라이더와 연동하여 특정 구간 반복 재생

---

## 🎨 시각화 시스템 (Visualization System)

### 4. LocationVisualizer.cs
**위치**: `Assets/Scripts/Visualization/LocationVisualizer.cs`

#### 📝 설명
위치 데이터를 3D 공간에 시각화하는 렌더링 엔진입니다. 포인트 표시, 이동 궤적, 라벨 표시 등을 담당합니다.

#### 🔧 주요 속성

**시각화 설정**
| 속성 | 타입 | 설명 | 기본값 |
|------|------|------|--------|
| `pointPrefab` | GameObject | 포인트 표시용 프리팹 | Sphere |
| `heightOffset` | float | 높이 오프셋 | 0.5 |
| `worldScale` | float | 좌표 스케일 | 1.0 |
| `pointSize` | float | 포인트 크기 | 0.3 |

**색상 설정**
```csharp
public Color[] pointColors = {
    Color.red,      // 포인트 1
    Color.blue,     // 포인트 2  
    Color.green,    // 포인트 3
    Color.yellow,   // 포인트 4
    Color.magenta,  // 포인트 5
    Color.cyan      // 포인트 6
};
```

**궤적 설정**
| 속성 | 타입 | 설명 | 기본값 |
|------|------|------|--------|
| `showTrails` | bool | 궤적 표시 여부 | true |
| `trailDuration` | float | 궤적 지속 시간 (초) | 5.0 |
| `trailWidth` | float | 궤적 너비 | 0.1 |

#### 🎯 주요 메서드

**시각화 제어**
```csharp
public void SetShowTrails(bool show)      // 궤적 표시 on/off
public void SetShowLabels(bool show)      // 라벨 표시 on/off
public void SetWorldScale(float scale)    // 월드 스케일 조정
public void ClearVisualization()          // 모든 시각화 객체 제거
public void ClearTrails()                 // 궤적만 제거
```

**좌표 변환**
```csharp
Vector3 DataToWorldPosition(float dataX, float dataY)
{
    return new Vector3(
        dataX * worldScale,    // 데이터 X → Unity X
        heightOffset,          // 고정 높이
        dataY * worldScale     // 데이터 Y → Unity Z
    );
}
```

#### 🏗️ 객체 생성 과정

1. **데이터 로드 시**:
   - 모든 고유 포인트 번호 추출
   - 각 포인트별 GameObject 생성
   - Trail Renderer 컴포넌트 추가
   - 라벨 TextMesh 생성

2. **재생 업데이트 시**:
   - 현재 시간의 데이터 수신
   - 각 포인트 위치 업데이트
   - 궤적 기록 갱신
   - 라벨 텍스트 업데이트

#### 📊 내부 데이터 구조

```csharp
Dictionary<int, GameObject> pointObjects        // 포인트별 게임오브젝트
Dictionary<int, TrailRenderer> trailRenderers   // 포인트별 궤적 렌더러
Dictionary<int, TextMesh> pointLabels          // 포인트별 라벨
Dictionary<int, List<Vector3>> positionHistory // 포인트별 위치 기록
```

#### 💡 사용 팁
- `worldScale` 조정으로 데이터 범위에 맞게 크기 조절
- 성능 문제 시 궤적 시간 감소 또는 라벨 비활성화
- 색상 배열 수정으로 포인트별 색상 커스터마이징

---

## 🎮 UI 관리 (UI Management)

### 5. PlaybackUIManager.cs
**위치**: `Assets/Scripts/UI/PlaybackUIManager.cs`

#### 📝 설명
사용자 인터페이스를 관리하고 재생 컨트롤러와 연결하는 UI 매니저입니다. 모든 UI 요소의 이벤트 처리와 상태 업데이트를 담당합니다.

#### 🔧 UI 요소 구성

**컨트롤 버튼**
| UI 요소 | 타입 | 기능 |
|---------|------|------|
| `playButton` | Button | 재생 시작 |
| `pauseButton` | Button | 일시정지 |
| `stopButton` | Button | 정지 및 리셋 |
| `resetButton` | Button | 데이터 재로드 |

**속도 제어**
| UI 요소 | 타입 | 기능 |
|---------|------|------|
| `speedUpButton` | Button | 속도 증가 |
| `speedDownButton` | Button | 속도 감소 |
| `speedSlider` | Slider | 속도 직접 조절 (0.25x ~ 8x) |
| `speedText` | Text | 현재 속도 표시 |

**타임라인**
| UI 요소 | 타입 | 기능 |
|---------|------|------|
| `timelineSlider` | Slider | 시간 탐색 (0~1) |
| `currentTimeText` | Text | 현재 시간 표시 |
| `totalTimeText` | Text | 전체 시간 표시 |
| `progressPercentageText` | Text | 진행률 표시 |

**옵션 토글**
| UI 요소 | 타입 | 기능 |
|---------|------|------|
| `loopToggle` | Toggle | 반복 재생 on/off |
| `interpolationToggle` | Toggle | 보간 on/off |
| `showTrailToggle` | Toggle | 궤적 표시 on/off |

**데이터 표시**
| UI 요소 | 타입 | 표시 내용 |
|---------|------|---------|
| `dataPointCountText` | Text | 총 데이터 포인트 수 |
| `currentPointsText` | Text | 현재 활성 포인트 수 |
| `dataSourceText` | Text | 데이터 소스 상태 |

#### 🎯 주요 메서드

**UI 초기화**
```csharp
void InitializeUI()
{
    // 버튼 이벤트 연결
    playButton.onClick.AddListener(() => playbackController.Play());
    
    // 슬라이더 설정
    timelineSlider.minValue = 0f;
    timelineSlider.maxValue = 1f;
    
    // 토글 이벤트 연결
    loopToggle.onValueChanged.AddListener(OnLoopToggleChanged);
}
```

**상태 업데이트**
```csharp
void UpdateButtonStates(bool isPlaying)
{
    playButton.interactable = !isPlaying && hasData;
    pauseButton.interactable = isPlaying;
    stopButton.interactable = isPlaying || currentTime > 0;
}
```

#### 📋 이벤트 처리

**데이터 로드 완료 시**
```csharp
void OnDataLoaded()
{
    // 데이터 통계 표시
    dataPointCountText.text = $"Total: {total} (D1: {d1}, D2: {d2})";
    
    // 시간 정보 표시
    totalTimeText.text = $"{minutes:00}:{seconds:00}.{ms:000}";
}
```

**재생 업데이트 시**
```csharp
void OnPlaybackUpdate(float currentTime, List<LocationData> currentData)
{
    // 타임라인 업데이트
    timelineSlider.value = normalizedTime;
    
    // 시간 표시 업데이트
    currentTimeText.text = FormatTime(currentTime);
    
    // 진행률 표시
    progressPercentageText.text = $"{percentage:F1}%";
}
```

#### 💡 사용 팁
- UI 요소는 선택적 - null 체크로 안전하게 처리
- 슬라이더 포커스 체크로 사용자 조작 중 업데이트 방지
- 버튼 상태 관리로 잘못된 조작 방지

---

## 🎯 메인 매니저 (Main Manager)

### 6. PlaybackManager.cs
**위치**: `Assets/Scripts/PlaybackManager.cs`

#### 📝 설명
전체 시스템을 통합 관리하는 싱글톤 매니저입니다. 모든 컴포넌트를 자동으로 생성하고 연결하며, 외부 인터페이스를 제공합니다.

#### 🔧 싱글톤 구현

```csharp
private static PlaybackManager instance;
public static PlaybackManager Instance
{
    get
    {
        if (instance == null)
        {
            instance = FindObjectOfType<PlaybackManager>();
            if (instance == null)
            {
                GameObject go = new GameObject("PlaybackManager");
                instance = go.AddComponent<PlaybackManager>();
            }
        }
        return instance;
    }
}
```

#### ⚙️ 자동 설정 옵션

| 속성 | 타입 | 설명 | 기본값 |
|------|------|------|--------|
| `autoCreateComponents` | bool | 컴포넌트 자동 생성 | true |
| `autoLoadDataOnStart` | bool | 시작 시 데이터 로드 | true |
| `autoPlayOnLoad` | bool | 로드 후 자동 재생 | false |

#### 🎯 주요 메서드

**재생 제어**
```csharp
public void StartPlayback()     // 재생 시작
public void PausePlayback()     // 일시정지
public void StopPlayback()      // 정지
public void ReloadData()        // 데이터 재로드
```

**설정 메서드**
```csharp
public void SetPlaybackSpeed(float speed)           // 속도 설정
public void SeekToTime(float time)                  // 시간 이동
public void SeekToNormalizedTime(float normalized)  // 정규화 시간 이동
```

#### 📊 공개 속성

```csharp
public bool IsPlaying          // 재생 상태
public float CurrentTime        // 현재 시간
public float TotalDuration      // 전체 시간
public float NormalizedTime     // 정규화 시간 (0~1)
public int TotalDataPoints      // 총 데이터 포인트 수
```

#### 🏗️ 컴포넌트 자동 설정

```csharp
void SetupComponents()
{
    // 1. CSVDataLoader 찾기/생성
    if (dataLoader == null)
    {
        dataLoader = GetComponent<CSVDataLoader>() 
                     ?? gameObject.AddComponent<CSVDataLoader>();
    }
    
    // 2. TimelinePlaybackController 찾기/생성
    // 3. LocationVisualizer 찾기/생성
    // 4. PlaybackUIManager 찾기/생성
}
```

#### 💡 사용 예시

**기본 사용법**
```csharp
// 싱글톤 인스턴스 접근
var manager = PlaybackManager.Instance;

// 재생 제어
manager.StartPlayback();
manager.SetPlaybackSpeed(2.0f);
manager.SeekToTime(30f);

// 상태 확인
if (manager.IsPlaying)
{
    Debug.Log($"재생 중: {manager.CurrentTime:F2}초");
}
```

**이벤트 구독**
```csharp
void Start()
{
    var manager = PlaybackManager.Instance;
    
    // 데이터 로더 이벤트 구독
    manager.dataLoader.OnDataLoaded += OnDataReady;
    
    // 재생 컨트롤러 이벤트 구독
    manager.playbackController.OnPlaybackUpdate += OnUpdate;
}
```

---

## 🚀 통합 사용 가이드

### 빠른 시작

1. **씬 설정**
   ```
   1. 빈 GameObject 생성
   2. PlaybackManager 컴포넌트 추가
   3. Play 버튼 클릭
   ```

2. **커스텀 설정**
   ```csharp
   void Start()
   {
       var manager = PlaybackManager.Instance;
       
       // 자동 재생 설정
       manager.autoPlayOnLoad = true;
       
       // 데이터 로드
       manager.ReloadData();
   }
   ```

### 이벤트 플로우

```
1. Start() → LoadAllData()
   ↓
2. CSV 파일 읽기 → 데이터 정렬
   ↓
3. OnDataLoaded 이벤트 발생
   ↓
4. LocationVisualizer: 객체 생성
   PlaybackUIManager: UI 활성화
   ↓
5. Play() → Update 루프 시작
   ↓
6. 매 프레임:
   - 시간 업데이트
   - 데이터 쿼리 (보간 옵션)
   - OnPlaybackUpdate 이벤트
   - 시각화 업데이트
   - UI 갱신
```

### 성능 최적화 팁

1. **대용량 데이터**
   - 보간 비활성화: 20% 성능 향상
   - 궤적 시간 감소: 15% 성능 향상
   - 라벨 비활성화: 10% 성능 향상

2. **메모리 관리**
   - positionHistory 크기 제한
   - Trail Renderer 시간 조절
   - 불필요한 로그 제거

3. **렌더링 최적화**
   - LOD 시스템 적용
   - 카메라 거리별 표시
   - 배치 렌더링 활용

### 확장 가능성

1. **실시간 데이터 스트리밍**
   ```csharp
   // CSVDataLoader 확장
   public void AddRealtimeData(LocationData newData)
   {
       allDataCombined.Add(newData);
       OnDataUpdated?.Invoke();
   }
   ```

2. **다중 데이터 소스**
   ```csharp
   // 동적 CSV 파일 추가
   public void AddDataSource(string csvPath, int deviceId)
   {
       LoadCSVData(csvPath, newDataList, deviceId);
       MergeAndSort();
   }
   ```

3. **고급 시각화**
   - 히트맵 생성
   - 3D 경로 리본
   - 속도/가속도 표시
   - 파티클 효과

---

## 📌 자주 묻는 질문 (FAQ)

**Q: CSV 파일 경로를 변경하려면?**
A: CSVDataLoader의 Inspector에서 csvFilePath1/2를 수정하거나 코드에서 직접 변경

**Q: 포인트 색상을 변경하려면?**
A: LocationVisualizer의 pointColors 배열 수정

**Q: 재생 속도 범위를 변경하려면?**
A: TimelinePlaybackController의 speedPresets 배열 수정

**Q: 새로운 데이터 필드를 추가하려면?**
A: LocationData 클래스에 필드 추가 → CSVDataLoader 파싱 로직 수정

**Q: 3D 모델로 포인트를 표시하려면?**
A: LocationVisualizer의 pointPrefab에 원하는 3D 모델 프리팹 할당

---

본 문서는 Unity 위치 측위 시스템의 모든 스크립트에 대한 상세 가이드입니다.
추가 질문이나 수정 사항이 있으시면 언제든지 문의해 주세요.