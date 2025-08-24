# Unity 위치 데이터 재생 시스템 - 빠른 시작 가이드

## 1분 설치

### 1단계: PlaybackManager 생성
```
1. Unity 에디터에서 빈 GameObject 생성 (Ctrl+Shift+N)
2. 이름을 "PlaybackManager"로 변경
3. PlaybackManager.cs 스크립트 추가
```

### 2단계: UI 생성 (선택사항)
```
1. Canvas 생성 (우클릭 → UI → Canvas)
2. 재생 버튼 추가 (Canvas 우클릭 → UI → Button)
3. 타임라인 슬라이더 추가 (Canvas 우클릭 → UI → Slider)
```

### 3단계: 실행
```
Play 버튼 클릭 → CSV 데이터 자동 로드 → 시각화 시작
```

## 주요 기능

### 🎮 재생 컨트롤
- **재생/일시정지**: Space 키 또는 UI 버튼
- **속도 조절**: +/- 키 (0.25x ~ 8x)
- **타임라인**: 슬라이더로 시간 이동

### 📊 데이터 시각화
- **실시간 위치 표시**: 색상별 포인트
- **이동 궤적**: Trail 효과
- **정보 라벨**: 포인트 번호와 좌표

### ⚙️ 설정 옵션
- **루프 재생**: 자동 반복
- **보간**: 부드러운 움직임
- **자동 재생**: 로드 후 자동 시작

## CSV 데이터 형식

| 필드 | 예시 | 설명 |
|------|------|------|
| DATE | 20250717 | YYYYMMDD |
| HHMM | 1034 | 시간분 |
| SEC | 29946 | 밀리초 |
| PTNUM | 1 | 포인트 번호 |
| location (X) | 1.75 | X 좌표 (m) |
| location (Y) | 2.3 | Y 좌표 (m) |

### 샘플 데이터
```csv
DATE,HHMM,SEC,PTNUM,location (X) / m,location (Y) / m
20250717,1034,29946,1,1.75,2.3
20250717,1034,30925,1,1.366053623,1.073841422
20250717,1034,31952,1,1.56720418,1.709844
```

## 코드 예제

### 기본 사용
```csharp
// 재생 시작
PlaybackManager.Instance.StartPlayback();

// 2배속 설정
PlaybackManager.Instance.SetPlaybackSpeed(2.0f);

// 30초 지점으로 이동
PlaybackManager.Instance.SeekToTime(30f);
```

### 이벤트 처리
```csharp
void Start()
{
    var controller = GetComponent<TimelinePlaybackController>();
    
    // 재생 업데이트 이벤트
    controller.OnPlaybackUpdate += (time, data) =>
    {
        Debug.Log($"시간: {time:F2}초, 포인트 수: {data.Count}");
    };
}
```

### 시각화 커스터마이징
```csharp
var visualizer = GetComponent<LocationVisualizer>();

// 궤적 설정
visualizer.SetShowTrails(true);
visualizer.trailDuration = 10f;  // 10초간 궤적 표시

// 색상 설정
visualizer.pointColors = new Color[] {
    Color.red,    // 포인트 1
    Color.blue,   // 포인트 2
    Color.green   // 포인트 3
};
```

## Inspector 설정

### PlaybackManager
- ✅ **Auto Create Components**: 자동 컴포넌트 생성
- ✅ **Auto Load Data On Start**: 시작시 데이터 로드
- ☐ **Auto Play On Load**: 로드 후 자동 재생

### TimelinePlaybackController
- **Playback Speed**: 1.0 (재생 속도)
- ✅ **Loop**: 반복 재생
- ✅ **Interpolate Movement**: 부드러운 움직임

### LocationVisualizer
- **Point Size**: 0.3 (포인트 크기)
- **World Scale**: 1.0 (월드 스케일)
- ✅ **Show Trails**: 궤적 표시
- ✅ **Show Labels**: 라벨 표시

## 문제 해결

### ❌ "CSV file not found" 오류
```csharp
// CSVDataLoader.cs에서 경로 확인
csvFilePath1 = "Documents/위츠측위 테스트 데이터/Raw Data/위치측위 Raw Data(스마트폰_1).csv"
```

### ❌ 재생이 안 됨
```csharp
// PlaybackManager에서 확인
autoLoadDataOnStart = true  // 자동 로드 활성화
autoPlayOnLoad = false      // 자동 재생 (선택)
```

### ❌ 포인트가 보이지 않음
- Scene 뷰에서 카메라 위치 조정
- Visualizer의 `heightOffset` 값 조정 (기본: 0.5)
- `worldScale` 값 조정 (기본: 1.0)

## 성능 팁

| 설정 | 성능 향상 | 영향 |
|------|----------|------|
| 보간 비활성화 | +20% | 움직임이 끊김 |
| Trail 시간 감소 | +15% | 짧은 궤적 |
| 라벨 비활성화 | +10% | 정보 표시 없음 |

## 단축키

| 키 | 기능 |
|----|------|
| Space | 재생/일시정지 |
| S | 정지 |
| + | 속도 증가 |
| - | 속도 감소 |
| L | 루프 토글 |
| T | 궤적 토글 |

## 유용한 팁

💡 **팁 1**: CSV 파일은 Excel에서 직접 편집 가능  
💡 **팁 2**: 큰 데이터셋의 경우 재생 속도를 높여서 빠르게 확인  
💡 **팁 3**: Scene 뷰와 Game 뷰를 나란히 배치하면 편리  
💡 **팁 4**: 콘솔 창(Ctrl+Shift+C)에서 디버그 메시지 확인

## 다음 단계

1. **더 많은 데이터 추가**: 추가 CSV 파일 로드
2. **고급 시각화**: 히트맵, 속도 표시 추가
3. **데이터 분석**: 통계 및 패턴 분석
4. **UI 개선**: 커스텀 컨트롤 추가

## 추가 리소스

- [Unity 공식 문서](https://docs.unity3d.com)
- CSV 샘플 데이터: `Documents/위츠측위 테스트 데이터/`
- 프로젝트 문서: README.md