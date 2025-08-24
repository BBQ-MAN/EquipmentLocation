# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Unity 3D project for visualizing and replaying equipment location tracking data from CSV files. The system displays time-based position data with 3D visualization, playback controls, and trajectory tracking.

## Key Commands

### Unity Development
- **Open Unity Editor**: Unity 2021.3 LTS or higher required
- **Play Mode**: Click Play button in Unity Editor to test with sample CSV data
- **Build**: File → Build Settings → Select platform → Build

### Code Development
- **Main Scene**: `Assets/Scenes/EquipmentLocationScene.unity`
- **Primary Manager**: `PlaybackManager.cs` - Singleton that coordinates all components
- **CSV Data Location**: `Documents/위츠측위 테스트 데이터/Raw Data/`

## Architecture

### Core Components (All managed by PlaybackManager singleton)

1. **Data Layer** (`Assets/Scripts/DataModels/`)
   - `LocationData.cs`: Data model for position entries (date, time, milliseconds, pointNumber, X/Y coordinates)
   - `CSVDataLoader.cs`: Loads and manages CSV data, handles time-based queries and interpolation

2. **Playback System** (`Assets/Scripts/Playback/`)
   - `TimelinePlaybackController.cs`: Controls time-based playback (play/pause/stop, speed control, timeline seeking)

3. **Visualization** (`Assets/Scripts/Visualization/`)
   - `LocationVisualizer.cs`: 3D rendering of position data with trails, labels, and point markers

4. **UI Management** (`Assets/Scripts/UI/`)
   - `PlaybackUIManager.cs`: Manages all UI controls (buttons, sliders, text displays)

### CSV Data Format
```csv
DATE,HHMM,SEC,PTNUM,location (X) / m,location (Y) / m
20250717,1034,29946,1,1.75,2.3
```
- DATE: YYYYMMDD format
- HHMM: Hour and minute
- SEC: Milliseconds within the time period
- PTNUM: Point identifier (1-6)
- location (X/Y): Coordinates in meters

### Component Setup Pattern
The PlaybackManager uses auto-setup to create and configure all necessary components:
1. Checks for existing component on GameObject
2. If not found, adds component automatically
3. Connects components via references
4. Enables auto-load and optional auto-play

## Key Implementation Details

### Singleton Pattern
PlaybackManager implements a lazy-initialized singleton accessible via `PlaybackManager.Instance`

### Event System
- `OnDataLoaded`: Fired when CSV data is loaded
- `OnPlaybackUpdate`: Fired each frame during playback with current time and data
- `OnPlaybackStateChanged`: Fired when play/pause/stop state changes

### Data Interpolation
The system interpolates position data between discrete time points for smooth movement when `interpolateMovement` is enabled.

### Time Management
- Uses `DateTime` for timestamp calculations
- Maintains elapsed time in seconds from first data point
- Supports normalized time (0-1) for timeline scrubbing

### Visualization Features
- Color-coded points per device/point number
- Trail Renderer for movement history
- Real-time coordinate labels
- Configurable world scale and height offset

## Unity-Specific Patterns

### Component Dependencies
Components are designed to work independently but are coordinated through PlaybackManager. Each component can be tested in isolation.

### Inspector Configuration
Key settings exposed in Unity Inspector:
- `autoCreateComponents`: Automatically set up required components
- `autoLoadDataOnStart`: Load CSV data on Start()
- `autoPlayOnLoad`: Begin playback after data loads
- `playbackSpeed`: Adjustable from 0.25x to 8x
- `loop`: Enable continuous playback
- `interpolateMovement`: Smooth movement between data points

### Prefab Usage
`EquipmentPrefab.prefab` is instantiated dynamically for each unique point number in the data.

## Performance Considerations

- CSV data is loaded once and cached in memory
- Uses binary search for time-based data lookups
- Trail Renderer duration affects performance (adjustable via `trailDuration`)
- Label rendering can be toggled for performance

## Testing Approach

The project uses manual testing in Unity Editor:
1. Enter Play Mode
2. Data loads automatically from CSV files
3. Use UI controls or keyboard shortcuts for playback control
4. Monitor Console for debug output

## Common Tasks

### Adding New CSV Files
1. Place CSV files in `Documents/위츠측위 테스트 데이터/Raw Data/`
2. Update file paths in `CSVDataLoader.cs`
3. Reload data via `PlaybackManager.Instance.ReloadData()`

### Customizing Visualization
- Modify `pointColors` array in LocationVisualizer for different colors
- Adjust `worldScale` to change coordinate mapping
- Configure `trailDuration` for trajectory length

### Extending Functionality
- Subscribe to events for custom behaviors
- Add new UI controls by extending PlaybackUIManager
- Implement additional data sources by extending CSVDataLoader