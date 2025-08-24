using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelinePlaybackController : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private CSVDataLoader dataLoader;
    
    [Header("Playback Settings")]
    [SerializeField] private bool autoPlay = false;
    [SerializeField] public float playbackSpeed = 1.0f;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool interpolateMovement = true;
    
    [Header("Playback State")]
    public bool isPlaying = false;
    public float currentTime = 0f;
    public float normalizedTime = 0f; // 0 to 1
    
    // UI References - managed internally
    private Slider timelineSlider;
    private Text timeDisplay;
    private Text speedDisplay;
    private Button playPauseButton;
    private Button stopButton;
    private Button speedUpButton;
    private Button speedDownButton;
    
    [Header("Speed Settings")]
    [SerializeField] private float[] speedPresets = { 0.25f, 0.5f, 1.0f, 2.0f, 4.0f, 8.0f };
    private int currentSpeedIndex = 2; // Default to 1.0x
    
    // Events
    public delegate void PlaybackUpdateEvent(float currentTime, List<LocationData> currentData);
    public event PlaybackUpdateEvent OnPlaybackUpdate;
    
    public delegate void PlaybackStateChangedEvent(bool isPlaying);
    public event PlaybackStateChangedEvent OnPlaybackStateChanged;
    
    void Awake()
    {
        Debug.Log($"TimelinePlaybackController: Awake called - GameObject: {gameObject.name}, " +
                 $"Active: {gameObject.activeSelf}, Component enabled: {enabled}");
    }
    
    void OnEnable()
    {
        Debug.Log("TimelinePlaybackController: OnEnable called");
    }
    
    void OnDisable()
    {
        Debug.Log("TimelinePlaybackController: OnDisable called");
    }
    
    void Start()
    {
        Debug.Log("TimelinePlaybackController: Start called");
        
        if (dataLoader == null)
        {
            dataLoader = GetComponent<CSVDataLoader>();
            Debug.Log($"TimelinePlaybackController: Found dataLoader: {dataLoader != null}");
        }
        
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded += OnDataLoaded;
            Debug.Log($"TimelinePlaybackController: Subscribed to OnDataLoaded, totalDuration: {dataLoader.totalDuration}");
        }
        else
        {
            Debug.LogError("TimelinePlaybackController: dataLoader is null!");
        }
        
        SetupUI();
        
        if (autoPlay && dataLoader != null && dataLoader.totalDuration > 0)
        {
            Debug.Log("TimelinePlaybackController: Auto-playing");
            Play();
        }
    }
    
    void OnDataLoaded()
    {
        currentTime = 0f;
        normalizedTime = 0f;
        UpdateUI();
        
        if (autoPlay)
        {
            Play();
        }
    }
    
    void SetupUI()
    {
        if (playPauseButton != null)
        {
            playPauseButton.onClick.RemoveAllListeners();
            playPauseButton.onClick.AddListener(TogglePlayPause);
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(Stop);
        }
        
        if (speedUpButton != null)
        {
            speedUpButton.onClick.RemoveAllListeners();
            speedUpButton.onClick.AddListener(IncreaseSpeed);
        }
        
        if (speedDownButton != null)
        {
            speedDownButton.onClick.RemoveAllListeners();
            speedDownButton.onClick.AddListener(DecreaseSpeed);
        }
        
        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0f;
            timelineSlider.maxValue = 1f;
            timelineSlider.onValueChanged.RemoveAllListeners();
            timelineSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        // Always log status every 30 frames
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"TimelinePlaybackController: Update called - dataLoader={dataLoader != null}, " +
                     $"totalDuration={dataLoader?.totalDuration:F2}, isPlaying={isPlaying}, " +
                     $"currentTime={currentTime:F2}");
        }
        
        if (dataLoader == null || dataLoader.totalDuration <= 0)
        {
            if (Time.frameCount % 30 == 0)
            {
                Debug.LogWarning($"TimelinePlaybackController: Update returning early - " +
                                $"dataLoader null: {dataLoader == null}, " +
                                $"totalDuration: {dataLoader?.totalDuration}");
            }
            return;
        }
        
        if (isPlaying)
        {
            // Log every few frames to avoid spam
            if (Time.frameCount % 30 == 0) // Log every 30 frames (about once per second at 30fps)
            {
                Debug.Log($"TimelinePlaybackController: PLAYING - currentTime={currentTime:F2}, normalizedTime={normalizedTime:F3}");
            }
            
            // Update playback time
            float deltaTime = Time.deltaTime * playbackSpeed;
            currentTime += deltaTime;
            
            // Handle loop
            if (currentTime >= dataLoader.totalDuration)
            {
                if (loop)
                {
                    currentTime = 0f;
                    Debug.Log("TimelinePlaybackController: Looping playback");
                }
                else
                {
                    currentTime = dataLoader.totalDuration;
                    Debug.Log("TimelinePlaybackController: Reached end, pausing");
                    Pause();
                }
            }
            
            normalizedTime = currentTime / dataLoader.totalDuration;
            
            // Update data and UI
            UpdatePlayback();
            UpdateUI();
        }
    }
    
    void UpdatePlayback()
    {
        if (dataLoader == null) 
        {
            Debug.LogWarning("TimelinePlaybackController: UpdatePlayback - dataLoader is null");
            return;
        }
        
        List<LocationData> currentData;
        
        if (interpolateMovement)
        {
            // Get interpolated data for smooth movement
            currentData = new List<LocationData>();
            
            // Get data for Device 1 (Smartphone 1)
            var device1Data = dataLoader.GetInterpolatedDataForDevice(1, currentTime);
            if (device1Data != null)
            {
                currentData.Add(device1Data);
            }
            
            // Get data for Device 2 (Smartphone 2)
            var device2Data = dataLoader.GetInterpolatedDataForDevice(2, currentTime);
            if (device2Data != null)
            {
                currentData.Add(device2Data);
            }
            
            Debug.Log($"TimelinePlaybackController: Interpolated {currentData.Count} devices at time {currentTime:F2}");
        }
        else
        {
            // Get discrete data points
            currentData = dataLoader.GetDataAtTime(currentTime);
            Debug.Log($"TimelinePlaybackController: Got {currentData.Count} discrete data points at time {currentTime:F2}");
        }
        
        // Notify listeners
        if (OnPlaybackUpdate != null)
        {
            Debug.Log($"TimelinePlaybackController: Invoking OnPlaybackUpdate with {currentData.Count} data points");
            OnPlaybackUpdate.Invoke(currentTime, currentData);
        }
        else
        {
            Debug.LogWarning("TimelinePlaybackController: OnPlaybackUpdate has no listeners!");
        }
    }
    
    void UpdateUI()
    {
        if (timelineSlider != null)
        {
            timelineSlider.value = normalizedTime;
        }
        
        if (timeDisplay != null && dataLoader != null)
        {
            DateTime currentDateTime = dataLoader.startTime.AddSeconds(currentTime);
            timeDisplay.text = $"{currentDateTime:HH:mm:ss.fff} / {dataLoader.endTime:HH:mm:ss.fff}";
        }
        
        if (speedDisplay != null)
        {
            speedDisplay.text = $"Speed: {playbackSpeed:F2}x";
        }
        
        // Update PlayPause button text using child Text component
        if (playPauseButton != null)
        {
            Text buttonText = playPauseButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isPlaying ? "Pause" : "Play";
            }
        }
    }
    
    public void Play()
    {
        Debug.Log($"Play() called - dataLoader: {dataLoader != null}, totalDuration: {dataLoader?.totalDuration}");
        
        if (dataLoader == null || dataLoader.totalDuration <= 0)
        {
            Debug.LogWarning($"Cannot play: dataLoader={dataLoader != null}, totalDuration={dataLoader?.totalDuration}");
            return;
        }
        
        Debug.Log("Starting playback...");
        isPlaying = true;
        OnPlaybackStateChanged?.Invoke(isPlaying);
        UpdateUI();
        Debug.Log($"Playback started. isPlaying={isPlaying}");
    }
    
    public void Pause()
    {
        isPlaying = false;
        OnPlaybackStateChanged?.Invoke(isPlaying);
        UpdateUI();
    }
    
    public void TogglePlayPause()
    {
        Debug.Log($"TogglePlayPause called - isPlaying before: {isPlaying}");
        if (isPlaying)
            Pause();
        else
            Play();
        Debug.Log($"TogglePlayPause finished - isPlaying after: {isPlaying}");
    }
    
    public void Stop()
    {
        isPlaying = false;
        currentTime = 0f;
        normalizedTime = 0f;
        OnPlaybackStateChanged?.Invoke(isPlaying);
        UpdatePlayback();
        UpdateUI();
    }
    
    public void SetPlaybackSpeed(float speed)
    {
        playbackSpeed = Mathf.Clamp(speed, 0.1f, 10f);
        UpdateUI();
    }
    
    public void IncreaseSpeed()
    {
        if (currentSpeedIndex < speedPresets.Length - 1)
        {
            currentSpeedIndex++;
            playbackSpeed = speedPresets[currentSpeedIndex];
            UpdateUI();
        }
    }
    
    public void DecreaseSpeed()
    {
        if (currentSpeedIndex > 0)
        {
            currentSpeedIndex--;
            playbackSpeed = speedPresets[currentSpeedIndex];
            UpdateUI();
        }
    }
    
    public void SeekToTime(float time)
    {
        if (dataLoader == null || dataLoader.totalDuration <= 0)
            return;
        
        currentTime = Mathf.Clamp(time, 0f, dataLoader.totalDuration);
        normalizedTime = currentTime / dataLoader.totalDuration;
        UpdatePlayback();
        UpdateUI();
    }
    
    public void SeekToNormalizedTime(float normalizedTime)
    {
        if (dataLoader == null || dataLoader.totalDuration <= 0)
            return;
        
        this.normalizedTime = Mathf.Clamp01(normalizedTime);
        currentTime = this.normalizedTime * dataLoader.totalDuration;
        UpdatePlayback();
        UpdateUI();
    }
    
    void OnSliderValueChanged(float value)
    {
        SeekToNormalizedTime(value);
    }
    
    public void SetLoop(bool loop)
    {
        this.loop = loop;
    }
    
    public void SetInterpolation(bool interpolate)
    {
        this.interpolateMovement = interpolate;
    }
}