using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Main playback controller for equipment location visualization
/// Controls time-based playback of CSV data with UI integration
/// </summary>
public class PlaybackController : MonoBehaviour
{
    // Internal components - not shown in Inspector
    private CSVDataLoader dataLoader;
    private LocationVisualizer visualizer;
    private Button playButton;
    private Button pauseButton;
    private Button stopButton;
    private Button resetButton;
    private Slider timelineSlider;
    private Text timeText;
    
    [Header("Playback Settings")]
    [Range(0.25f, 4.0f)]
    [Tooltip("Speed of playback (1.0 = normal speed)")]
    public float playbackSpeed = 1.0f;
    
    [Header("Status (Read Only)")]
    [SerializeField] private bool _isPlaying = false;
    [SerializeField] private float _currentTime = 0f;
    [SerializeField] private float totalDuration = 0f;
    
    // Public properties for external access
    public bool isPlaying => _isPlaying;
    public float currentTime => _currentTime;
    public float normalizedTime => (dataLoader != null && dataLoader.totalDuration > 0) ? _currentTime / dataLoader.totalDuration : 0f;
    
    void Start()
    {
        Debug.Log("PlaybackController: Starting");
        
        // Find components
        dataLoader = FindObjectOfType<CSVDataLoader>();
        visualizer = FindObjectOfType<LocationVisualizer>();
        
        if (dataLoader == null)
        {
            Debug.LogError("PlaybackController: CSVDataLoader not found!");
            return;
        }
        
        if (visualizer == null)
        {
            Debug.LogError("PlaybackController: LocationVisualizer not found!");
            return;
        }
        
        Debug.Log($"PlaybackController: Found components. Data duration: {dataLoader.totalDuration}");
        
        // Subscribe to data loaded event
        dataLoader.OnDataLoaded += OnDataLoaded;
        
        // Setup UI buttons if they exist
        SetupUIButtons();
    }
    
    void SetupUIButtons()
    {
        try
        {
            // Try to find UI buttons from the scene
            if (playButton == null)
            {
                var playGO = GameObject.Find("PlayButton");
                if (playGO != null) playButton = playGO.GetComponent<Button>();
            }
        
        if (pauseButton == null)
        {
            var pauseGO = GameObject.Find("PauseButton");
            if (pauseGO != null) pauseButton = pauseGO.GetComponent<Button>();
        }
        
        if (stopButton == null)
        {
            var stopGO = GameObject.Find("StopButton");
            if (stopGO != null) stopButton = stopGO.GetComponent<Button>();
        }
        
        if (resetButton == null)
        {
            var resetGO = GameObject.Find("ResetButton");
            if (resetGO != null) resetButton = resetGO.GetComponent<Button>();
        }
        
        if (timelineSlider == null)
        {
            var sliderGO = GameObject.Find("TimelineSlider");
            if (sliderGO != null) timelineSlider = sliderGO.GetComponent<Slider>();
        }
        
        // Clear existing listeners and add new ones
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(Play);
            Debug.Log("PlaybackController: Play button connected");
        }
        
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(Pause);
            // Hide pause button initially
            pauseButton.gameObject.SetActive(false);
            Debug.Log("PlaybackController: Pause button connected");
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(Stop);
            Debug.Log("PlaybackController: Stop button connected");
        }
        
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(Reset);
            Debug.Log("PlaybackController: Reset button connected");
        }
        
        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0f;
            timelineSlider.maxValue = 1f;
            timelineSlider.onValueChanged.RemoveAllListeners();
            timelineSlider.onValueChanged.AddListener(OnSliderChanged);
            Debug.Log("PlaybackController: Timeline slider connected");
        }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"PlaybackController: Error setting up UI buttons - {e.Message}");
        }
    }
    
    void OnDataLoaded()
    {
        Debug.Log($"PlaybackController: Data loaded! Duration: {dataLoader.totalDuration}");
        UpdateUI();
    }
    
    public void Play()
    {
        if (dataLoader != null && dataLoader.totalDuration > 0)
        {
            _isPlaying = true;
            Debug.Log("PlaybackController: Playing");
            
            // Toggle button visibility
            if (playButton != null) playButton.gameObject.SetActive(false);
            if (pauseButton != null) pauseButton.gameObject.SetActive(true);
            
            // Update button text if using play/pause button
            UpdatePlayPauseButtonText(true);
        }
    }
    
    public void Pause()
    {
        _isPlaying = false;
        Debug.Log("PlaybackController: Paused");
        
        // Toggle button visibility
        if (playButton != null) playButton.gameObject.SetActive(true);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        
        // Update button text if using play/pause button
        UpdatePlayPauseButtonText(false);
    }
    
    public void Stop()
    {
        _isPlaying = false;
        _currentTime = 0f;
        Debug.Log("PlaybackController: Stopped");
        
        // Reset button visibility
        if (playButton != null) playButton.gameObject.SetActive(true);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
        
        // Update button text if using play/pause button
        UpdatePlayPauseButtonText(false);
        
        UpdateUI();
        UpdateVisualization();
    }
    
    public void Reset()
    {
        // Stop playback first
        Stop();
        
        Debug.Log("PlaybackController: Resetting - Reloading all data");
        
        // Reload CSV data from files
        if (dataLoader != null)
        {
            dataLoader.LoadAllData();
        }
        
        // Clear visualization trails if needed
        if (visualizer != null)
        {
            var clearTrailsMethod = visualizer.GetType().GetMethod("ClearTrails",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (clearTrailsMethod != null)
            {
                clearTrailsMethod.Invoke(visualizer, null);
            }
        }
        
        Debug.Log("PlaybackController: Reset complete");
    }
    
    public void SetPlaybackSpeed(float speed)
    {
        playbackSpeed = Mathf.Clamp(speed, 0.25f, 4.0f);
        Debug.Log($"PlaybackController: Playback speed set to {playbackSpeed}x");
    }
    
    public void SeekToTime(float time)
    {
        if (dataLoader == null || dataLoader.totalDuration <= 0)
            return;
        
        _currentTime = Mathf.Clamp(time, 0f, dataLoader.totalDuration);
        UpdateVisualization();
        UpdateUI();
    }
    
    public void SeekToNormalizedTime(float normalizedTime)
    {
        if (dataLoader == null || dataLoader.totalDuration <= 0)
            return;
        
        float targetTime = normalizedTime * dataLoader.totalDuration;
        SeekToTime(targetTime);
    }
    
    void OnSliderChanged(float value)
    {
        if (dataLoader != null && dataLoader.totalDuration > 0)
        {
            _currentTime = value * dataLoader.totalDuration;
            UpdateVisualization();
        }
    }
    
    void Update()
    {
        if (!_isPlaying || dataLoader == null || dataLoader.totalDuration <= 0)
            return;
        
        // Update time
        _currentTime += Time.deltaTime * playbackSpeed;
        
        // Handle end of playback
        if (_currentTime >= dataLoader.totalDuration)
        {
            _currentTime = dataLoader.totalDuration;
            Stop(); // Stop at the end
            return;
        }
        
        UpdateVisualization();
        UpdateUI();
    }
    
    void UpdateVisualization()
    {
        if (dataLoader == null)
        {
            Debug.LogWarning("PlaybackController: dataLoader is null in UpdateVisualization");
            return;
        }
        
        if (visualizer == null)
        {
            Debug.LogWarning("PlaybackController: visualizer is null in UpdateVisualization");
            return;
        }
            
        // Get data for current time
        var device1Data = dataLoader.GetInterpolatedDataForDevice(1, _currentTime);
        var device2Data = dataLoader.GetInterpolatedDataForDevice(2, _currentTime);
        
        // Create list for visualizer
        var currentData = new List<LocationData>();
        if (device1Data != null) currentData.Add(device1Data);
        if (device2Data != null) currentData.Add(device2Data);
        
        // Call visualizer update directly using reflection
        if (currentData.Count > 0)
        {
            try
            {
                var method = visualizer.GetType().GetMethod("OnPlaybackUpdate", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(visualizer, new object[] { _currentTime, currentData });
                }
                else
                {
                    Debug.LogWarning("PlaybackController: OnPlaybackUpdate method not found in LocationVisualizer");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"PlaybackController: Error calling OnPlaybackUpdate - {e.Message}");
            }
            
            // Log periodically
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"PlaybackController: Time={_currentTime:F2}/{dataLoader.totalDuration:F2}, Devices={currentData.Count}");
            }
        }
    }
    
    void UpdateUI()
    {
        // Update timeline slider
        if (timelineSlider != null && dataLoader != null && dataLoader.totalDuration > 0)
        {
            timelineSlider.SetValueWithoutNotify(_currentTime / dataLoader.totalDuration);
        }
        
        // Update time text
        if (timeText != null && dataLoader != null)
        {
            int currentMinutes = Mathf.FloorToInt(_currentTime / 60f);
            int currentSeconds = Mathf.FloorToInt(_currentTime % 60f);
            int totalMinutes = Mathf.FloorToInt(dataLoader.totalDuration / 60f);
            int totalSeconds = Mathf.FloorToInt(dataLoader.totalDuration % 60f);
            
            timeText.text = $"{currentMinutes:00}:{currentSeconds:00} / {totalMinutes:00}:{totalSeconds:00}";
        }
    }
    
    void UpdatePlayPauseButtonText(bool isPlaying)
    {
        // Find PlayPauseButton if it exists
        GameObject playPauseGO = GameObject.Find("PlayPauseButton");
        if (playPauseGO != null)
        {
            Button playPauseButton = playPauseGO.GetComponent<Button>();
            if (playPauseButton != null)
            {
                // Get the Text component from the button's children
                Text buttonText = playPauseButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = isPlaying ? "Pause" : "Play";
                }
            }
        }
    }
    
    // Remove OnGUI to avoid duplicate controls
    void OnDestroy()
    {
        // Clean up event subscriptions
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded -= OnDataLoaded;
        }
    }
}