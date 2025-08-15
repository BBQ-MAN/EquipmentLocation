# Equipment Location System - API Documentation

## Core Classes

### EquipmentData
**Purpose**: Data model for equipment location information

#### Properties
| Property | Type | Description |
|----------|------|-------------|
| equipmentId | string | Unique identifier for equipment |
| timestamp | float | Time in seconds |
| position | Vector3 | 3D coordinates (x, y, z) |
| eventType | string | Type of event (e.g., "alarm") |
| description | string | Event description |

#### Constructor
```csharp
public EquipmentData(string id, float time, Vector3 pos, string evt = "", string desc = "")
```

---

### ExcelFileReader
**Purpose**: Parse CSV files containing equipment location data

#### Methods

##### LoadCSVFile
```csharp
public static List<EquipmentData> LoadCSVFile(string filePath)
```
**Parameters**:
- `filePath`: Full path to CSV file

**Returns**: List of EquipmentData sorted by timestamp

**Exceptions**:
- FileNotFoundException: If file doesn't exist
- FormatException: If data parsing fails

---

### SimpleLocationController : MonoBehaviour
**Purpose**: Main controller managing the entire system

#### Public Properties
| Property | Type | Description |
|----------|------|-------------|
| loadButton | Button | File load UI button |
| timeSlider | Slider | Timeline control slider |
| playButton | Button | Start playback button |
| pauseButton | Button | Pause playback button |
| equipmentPrefab | GameObject | Template for equipment objects |
| terrain | GameObject | Terrain reference |

#### Key Methods

##### LoadFile()
```csharp
void LoadFile()
```
Initiates file loading process

##### Play()
```csharp
void Play()
```
Starts timeline playback

##### Pause()
```csharp
void Pause()
```
Pauses timeline playback

##### SetTime(float value)
```csharp
void SetTime(float value)
```
Sets current timeline position

##### UpdatePositions()
```csharp
void UpdatePositions()
```
Updates all equipment positions based on current time

---

### EquipmentMovementController : MonoBehaviour
**Purpose**: Handle smooth movement interpolation for individual equipment

#### Methods

##### Initialize
```csharp
public void Initialize(string id, List<EquipmentData> data)
```
**Parameters**:
- `id`: Equipment identifier
- `data`: Movement trajectory data

##### UpdatePosition
```csharp
public void UpdatePosition(float currentTime, Terrain terrain)
```
**Parameters**:
- `currentTime`: Current timeline position
- `terrain`: Terrain reference for height alignment

**Features**:
- Smooth interpolation between points
- Terrain height alignment
- Rotation towards movement direction

---

### AlarmMarkerSystem : MonoBehaviour
**Purpose**: Manage alarm markers on timeline

#### Public Properties
| Property | Type | Description |
|----------|------|-------------|
| timelineSlider | Slider | Reference to timeline slider |
| alarmMarkerPrefab | GameObject | Template for alarm markers |
| markerContainer | Transform | Parent for marker objects |

#### Methods

##### AddAlarmEvent
```csharp
public void AddAlarmEvent(float time, string description)
```
**Parameters**:
- `time`: Timestamp for alarm
- `description`: Alarm description

##### ClearMarkers
```csharp
public void ClearMarkers()
```
Removes all alarm markers

##### SetupSampleAlarms
```csharp
public void SetupSampleAlarms()
```
Creates sample alarm events for testing

---

## Usage Examples

### Loading and Processing Data
```csharp
// Load CSV file
string filePath = "path/to/data.csv";
List<EquipmentData> dataList = ExcelFileReader.LoadCSVFile(filePath);

// Process by equipment ID
Dictionary<string, List<EquipmentData>> trajectories = new Dictionary<string, List<EquipmentData>>();
foreach (var data in dataList)
{
    if (!trajectories.ContainsKey(data.equipmentId))
        trajectories[data.equipmentId] = new List<EquipmentData>();
    trajectories[data.equipmentId].Add(data);
}
```

### Creating Equipment Objects
```csharp
// Create equipment GameObject
GameObject equipmentObj = Instantiate(equipmentPrefab);
equipmentObj.name = "Equipment_" + equipmentId;

// Add movement controller
var controller = equipmentObj.AddComponent<EquipmentMovementController>();
controller.Initialize(equipmentId, trajectoryData);
```

### Timeline Control
```csharp
// Set timeline range
timelineSlider.minValue = minTime;
timelineSlider.maxValue = maxTime;

// Update position at specific time
float targetTime = 50.0f;
controller.UpdatePosition(targetTime, terrain);
```

### Adding Alarm Markers
```csharp
// Create alarm system
AlarmMarkerSystem alarmSystem = gameObject.AddComponent<AlarmMarkerSystem>();
alarmSystem.timelineSlider = timelineSlider;
alarmSystem.markerContainer = markerParent;

// Add alarm events
alarmSystem.AddAlarmEvent(25.0f, "Equipment Warning");
alarmSystem.AddAlarmEvent(75.0f, "System Alert");
```

---

## Events and Callbacks

### Timeline Events
- `Slider.onValueChanged`: Triggered when timeline position changes
- `Button.onClick`: Triggered for play/pause/load actions

### Custom Events (Extension Points)
```csharp
// Add to SimpleLocationController for custom events
public UnityEvent<float> OnTimeChanged;
public UnityEvent<EquipmentData> OnAlarmTriggered;
public UnityEvent OnPlaybackStarted;
public UnityEvent OnPlaybackPaused;
```

---

## Performance Considerations

### Optimization Tips
1. **Batch Updates**: Update all equipment positions in single frame
2. **LOD System**: Implement level-of-detail for distant equipment
3. **Object Pooling**: Reuse equipment GameObjects
4. **Interpolation Caching**: Cache interpolated positions

### Memory Management
- Clear unused trajectory data after processing
- Destroy equipment objects when not needed
- Use object pooling for large datasets

---

## Error Handling

### Common Errors and Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| NullReferenceException | Missing UI references | Check Inspector assignments |
| FileNotFoundException | Invalid file path | Verify file exists |
| FormatException | Invalid CSV format | Check data format |
| IndexOutOfRangeException | Empty trajectory | Validate data before use |

### Logging
```csharp
Debug.Log("File loaded: " + filePath);
Debug.LogWarning("No data for equipment: " + id);
Debug.LogError("Failed to parse line: " + line);
```