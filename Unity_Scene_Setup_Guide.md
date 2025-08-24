# Unity 씬 구성 상세 가이드

## 📋 전체 씬 구조 개요

```
EquipmentLocationScene
├── Main Camera
├── Directional Light
├── PlaybackManager (핵심 시스템)
├── UI Canvas
│   ├── Control Panel
│   ├── Timeline Panel
│   └── Info Panel
├── Visualization
│   └── (동적 생성되는 포인트들)
└── Environment (선택사항)
    └── Ground Plane
```

---

## 🎯 Step 1: 새 씬 생성 및 기본 설정

### 1.1 새 씬 생성
1. **File → New Scene** 또는 **Ctrl+N**
2. **Basic (Built-in)** 템플릿 선택
3. **File → Save As** → `Assets/Scenes/EquipmentLocationScene.unity`

### 1.2 기본 오브젝트 확인
- **Main Camera**: 위치 (0, 10, -10), 회전 (30, 0, 0)
- **Directional Light**: 기본 설정 유지

---

## 🎮 Step 2: PlaybackManager 설정 (핵심)

### 2.1 PlaybackManager GameObject 생성
```
1. Hierarchy 우클릭 → Create Empty
2. 이름을 "PlaybackManager"로 변경
3. Transform Reset (위치: 0,0,0)
```

### 2.2 필수 컴포넌트 추가
```
Inspector에서 Add Component:
1. PlaybackManager.cs
2. CSVDataLoader.cs  
3. TimelinePlaybackController.cs
```

### 2.3 PlaybackManager 설정
```
PlaybackManager (Script)
├── [✓] Auto Create Components  
├── [✓] Auto Load Data On Start
└── [ ] Auto Play On Load
```

---

## 📊 Step 3: 시각화 시스템 설정

### 3.1 LocationVisualizer GameObject 생성
```
1. Hierarchy 우클릭 → Create Empty
2. 이름을 "LocationVisualizer"로 변경
3. Add Component → LocationVisualizer.cs
```

### 3.2 LocationVisualizer 컴포넌트 설정
```
Location Visualizer (Script)
├── Visualization Settings
│   ├── Point Prefab: (자동 생성됨)
│   ├── Height Offset: 0.5
│   └── World Scale: 1.0
├── Point Appearance
│   ├── Point Colors: (6개 색상 배열)
│   │   ├── Element 0: Red
│   │   ├── Element 1: Blue
│   │   ├── Element 2: Green
│   │   ├── Element 3: Yellow
│   │   ├── Element 4: Magenta
│   │   └── Element 5: Cyan
│   └── Point Size: 0.3
├── Trail Settings
│   ├── [✓] Show Trails
│   ├── Trail Duration: 5
│   └── Trail Width: 0.1
└── Label Settings
    └── [✓] Show Labels
```

### 3.3 포인트 프리팹 생성 (선택사항)
```
수동으로 프리팹을 만들려면:
1. GameObject → 3D Object → Sphere
2. Scale: (0.3, 0.3, 0.3)
3. Material 생성 및 적용
4. Project 폴더로 드래그 → Prefabs/EquipmentPrefab.prefab
5. 씬에서 원본 삭제
6. LocationVisualizer의 Point Prefab 슬롯에 할당
```

---

## 🎨 Step 4: UI 시스템 구성

### 4.1 Canvas 생성
```
1. Hierarchy 우클릭 → UI → Canvas
2. Canvas Scaler 설정:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080
   - Screen Match Mode: 0.5
```

### 4.2 UI Manager GameObject
```
1. Canvas 아래에 Create Empty → "UIManager"
2. Add Component → PlaybackUIManager.cs
```

### 4.3 Control Panel 생성
```
Canvas 우클릭 → Create Empty → "ControlPanel"
├── Position: Top-Left (Anchor Preset: Top-Left)
├── Rect Transform: (10, -10, 300, 150)
└── 하위 요소:
    ├── PlayButton (UI → Button - TextMeshPro)
    │   └── Text: "▶ Play"
    ├── PauseButton  
    │   └── Text: "❚❚ Pause"
    ├── StopButton
    │   └── Text: "■ Stop"
    └── ResetButton
        └── Text: "↻ Reset"
```

### 4.4 Timeline Panel 생성
```
Canvas 우클릭 → Create Empty → "TimelinePanel"
├── Position: Bottom (Anchor Preset: Bottom-Stretch)
├── Rect Transform: (10, 10, -10, 80)
└── 하위 요소:
    ├── TimelineSlider (UI → Slider)
    │   ├── Min Value: 0
    │   ├── Max Value: 1
    │   └── Value: 0
    ├── CurrentTimeText (UI → Text - TextMeshPro)
    │   └── Text: "00:00.000"
    └── TotalTimeText
        └── Text: "00:00.000"
```

### 4.5 Speed Control Panel
```
Canvas 우클릭 → Create Empty → "SpeedControlPanel"
├── Position: Top-Right (Anchor Preset: Top-Right)
├── Rect Transform: (-310, -10, 300, 100)
└── 하위 요소:
    ├── SpeedUpButton (Text: "▲")
    ├── SpeedDownButton (Text: "▼")
    ├── SpeedText (Text: "1.0x")
    └── SpeedSlider
        ├── Min Value: 0.25
        ├── Max Value: 8
        └── Value: 1
```

### 4.6 Options Panel
```
Canvas 우클릭 → Create Empty → "OptionsPanel"
├── Position: Left-Middle
└── 하위 요소:
    ├── LoopToggle (UI → Toggle)
    │   └── Label: "Loop Playback"
    ├── InterpolationToggle
    │   └── Label: "Smooth Movement"
    └── ShowTrailToggle
        └── Label: "Show Trails"
```

### 4.7 Info Display Panel
```
Canvas 우클릭 → Create Empty → "InfoPanel"
├── Position: Right-Middle
└── 하위 요소:
    ├── DataPointCountText
    │   └── Text: "Total Points: 0"
    ├── CurrentPointsText
    │   └── Text: "Active: 0"
    └── ProgressPercentageText
        └── Text: "0.0%"
```

---

## 🔗 Step 5: 컴포넌트 연결

### 5.1 PlaybackManager 참조 연결
```
PlaybackManager Inspector:
├── Data Loader: CSVDataLoader (자기 자신)
├── Playback Controller: TimelinePlaybackController (자기 자신)
├── Visualizer: LocationVisualizer (씬에서 찾기)
└── UI Manager: PlaybackUIManager (Canvas/UIManager)
```

### 5.2 TimelinePlaybackController UI 연결
```
TimelinePlaybackController Inspector:
├── Data Source
│   └── Data Loader: CSVDataLoader (자동 연결)
├── UI References
│   ├── Timeline Slider: Canvas/TimelinePanel/TimelineSlider
│   ├── Time Display: Canvas/TimelinePanel/CurrentTimeText
│   ├── Speed Display: Canvas/SpeedControlPanel/SpeedText
│   ├── Play Pause Button: Canvas/ControlPanel/PlayButton
│   ├── Stop Button: Canvas/ControlPanel/StopButton
│   ├── Speed Up Button: Canvas/SpeedControlPanel/SpeedUpButton
│   ├── Speed Down Button: Canvas/SpeedControlPanel/SpeedDownButton
│   └── Play Pause Button Text: PlayButton/Text
```

### 5.3 PlaybackUIManager 연결
```
PlaybackUIManager Inspector:
├── Control Buttons
│   ├── Play Button: ControlPanel/PlayButton
│   ├── Pause Button: ControlPanel/PauseButton
│   ├── Stop Button: ControlPanel/StopButton
│   └── Reset Button: ControlPanel/ResetButton
├── Speed Controls
│   ├── Speed Up Button: SpeedControlPanel/SpeedUpButton
│   ├── Speed Down Button: SpeedControlPanel/SpeedDownButton
│   ├── Speed Text: SpeedControlPanel/SpeedText
│   └── Speed Slider: SpeedControlPanel/SpeedSlider
├── Timeline
│   ├── Timeline Slider: TimelinePanel/TimelineSlider
│   ├── Current Time Text: TimelinePanel/CurrentTimeText
│   ├── Total Time Text: TimelinePanel/TotalTimeText
│   └── Progress Percentage Text: InfoPanel/ProgressPercentageText
├── Options
│   ├── Loop Toggle: OptionsPanel/LoopToggle
│   ├── Interpolation Toggle: OptionsPanel/InterpolationToggle
│   └── Show Trail Toggle: OptionsPanel/ShowTrailToggle
├── Data Display
│   ├── Data Point Count Text: InfoPanel/DataPointCountText
│   └── Current Points Text: InfoPanel/CurrentPointsText
└── References
    ├── Playback Controller: PlaybackManager/TimelinePlaybackController
    └── Data Loader: PlaybackManager/CSVDataLoader
```

### 5.4 LocationVisualizer 연결
```
LocationVisualizer Inspector:
├── References
│   ├── Playback Controller: PlaybackManager (TimelinePlaybackController)
│   └── Data Loader: PlaybackManager (CSVDataLoader)
```

---

## 🌍 Step 6: 환경 설정 (선택사항)

### 6.1 Ground Plane 생성
```
1. GameObject → 3D Object → Plane
2. 이름: "Ground"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (10, 1, 10)
4. Material: 기본 또는 커스텀
```

### 6.2 카메라 조정
```
Main Camera 설정:
├── Position: (0, 15, -15)
├── Rotation: (45, 0, 0)
├── Field of View: 60
└── Clear Flags: Skybox 또는 Solid Color
```

### 6.3 조명 설정
```
Directional Light:
├── Rotation: (50, -30, 0)
├── Intensity: 1
└── Shadow Type: Soft Shadows
```

---

## ✅ Step 7: 최종 확인 및 테스트

### 7.1 컴포넌트 체크리스트
- [ ] PlaybackManager가 있는가?
- [ ] CSVDataLoader가 PlaybackManager에 있는가?
- [ ] TimelinePlaybackController가 설정되었는가?
- [ ] LocationVisualizer가 씬에 있는가?
- [ ] UI Canvas와 모든 UI 요소가 생성되었는가?
- [ ] PlaybackUIManager가 UI 요소들과 연결되었는가?

### 7.2 CSV 파일 경로 확인
```
CSVDataLoader Inspector:
├── Csv File Path 1: Documents/위츠측위 테스트 데이터/Raw Data/위치측위 Raw Data(스마트폰_1).csv
└── Csv File Path 2: Documents/위츠측위 테스트 데이터/Raw Data/위치측위 Raw Data(스마트폰_2).csv
```

### 7.3 Play Mode 테스트
1. **Play 버튼 클릭**
2. **Console 창 확인**:
   - "Data loaded successfully!"
   - "Total points: XXX"
   - "Duration: XX.XX seconds"
3. **UI 동작 확인**:
   - Play/Pause 버튼 작동
   - Timeline 슬라이더 이동
   - 속도 조절 가능
4. **시각화 확인**:
   - 포인트들이 표시되는지
   - 궤적이 그려지는지
   - 라벨이 보이는지

---

## 🚨 일반적인 문제 해결

### 문제 1: "CSV file not found"
**해결책**: 
- CSV 파일이 프로젝트 루트의 Documents 폴더에 있는지 확인
- CSVDataLoader의 경로가 정확한지 확인

### 문제 2: UI가 보이지 않음
**해결책**:
- Canvas의 Render Mode가 "Screen Space - Overlay"인지 확인
- UI 요소들의 Anchor Preset 확인
- Canvas Scaler 설정 확인

### 문제 3: 포인트가 보이지 않음
**해결책**:
- LocationVisualizer의 Height Offset 조정 (기본: 0.5)
- World Scale 값 조정 (데이터 범위에 따라)
- 카메라 위치와 각도 확인

### 문제 4: 재생이 안 됨
**해결책**:
- PlaybackManager의 Auto Load Data On Start 체크
- Console에서 에러 메시지 확인
- 데이터가 제대로 로드되었는지 확인

---

## 💡 추가 팁

1. **Prefab으로 저장**: 
   - 설정 완료된 PlaybackManager를 Prefab으로 저장
   - 다른 씬에서 재사용 가능

2. **Layout 저장**:
   - Window → Layouts → Save Layout
   - Scene/Game/Console/Inspector 창 배치 저장

3. **단축키 활용**:
   - Space: Play/Pause (스크립트에 추가 가능)
   - +/-: 속도 조절 (스크립트에 추가 가능)

4. **성능 최적화**:
   - 대용량 데이터: Trail Duration 감소
   - Show Labels 비활성화로 성능 향상

---

이 가이드를 따라하면 완전한 Unity 위치 측위 시스템 씬을 구성할 수 있습니다.