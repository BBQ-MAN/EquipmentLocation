using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlaybackUIManager : MonoBehaviour
{
    [Header("Playback UI Panel")]
    [SerializeField] private GameObject playbackPanel;
    
    [Header("Control Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button resetButton;
    
    [Header("Speed Controls")]
    [SerializeField] private Button speedUpButton;
    [SerializeField] private Button speedDownButton;
    [SerializeField] private Text speedText;
    [SerializeField] private Slider speedSlider;
    
    [Header("Timeline")]
    [SerializeField] private Slider timelineSlider;
    [SerializeField] private Text currentTimeText;
    [SerializeField] private Text totalTimeText;
    [SerializeField] private Text progressPercentageText;
    
    [Header("Options")]
    [SerializeField] private Toggle loopToggle;
    [SerializeField] private Toggle interpolationToggle;
    [SerializeField] private Toggle showTrailToggle;
    
    [Header("Data Display")]
    [SerializeField] private Text dataPointCountText;
    [SerializeField] private Text currentPointsText;
    [SerializeField] private Text dataSourceText;
    
    [Header("References")]
    [SerializeField] private TimelinePlaybackController playbackController;
    [SerializeField] private CSVDataLoader dataLoader;
    
    private bool isInitialized = false;
    
    void Start()
    {
        // DISABLED: This component is replaced by PlaybackController
        Debug.LogWarning("PlaybackUIManager is deprecated. Use PlaybackController instead.");
        this.enabled = false;
        return;
        
        /*
        if (playbackController == null)
            playbackController = FindObjectOfType<TimelinePlaybackController>();
        
        if (dataLoader == null)
            dataLoader = FindObjectOfType<CSVDataLoader>();
        
        if (playbackController != null && dataLoader != null)
        {
            InitializeUI();
            
            // Subscribe to events
            playbackController.OnPlaybackStateChanged += OnPlaybackStateChanged;
            playbackController.OnPlaybackUpdate += OnPlaybackUpdate;
            dataLoader.OnDataLoaded += OnDataLoaded;
        }
        else
        {
            Debug.LogError("PlaybackUIManager: Missing required components!");
        }
        */
    }
    
    void InitializeUI()
    {
        // Play/Pause buttons
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => {
                Debug.Log("PlaybackUIManager: Play button clicked");
                if (playbackController != null)
                {
                    Debug.Log($"PlaybackUIManager: Calling playbackController.Play() - controller exists");
                    playbackController.Play();
                }
                else
                {
                    Debug.LogError("PlaybackUIManager: playbackController is null!");
                }
            });
            Debug.Log($"PlaybackUIManager: Play button listener added - button={playButton.name}");
        }
        else
        {
            Debug.LogWarning("PlaybackUIManager: playButton is null!");
        }
        
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() => playbackController.Pause());
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(() => playbackController.Stop());
        }
        
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(() => {
                playbackController.Stop();
                dataLoader.LoadAllData();
            });
        }
        
        // Speed controls
        if (speedUpButton != null)
        {
            speedUpButton.onClick.RemoveAllListeners();
            speedUpButton.onClick.AddListener(() => playbackController.IncreaseSpeed());
        }
        
        if (speedDownButton != null)
        {
            speedDownButton.onClick.RemoveAllListeners();
            speedDownButton.onClick.AddListener(() => playbackController.DecreaseSpeed());
        }
        
        if (speedSlider != null)
        {
            speedSlider.minValue = 0.25f;
            speedSlider.maxValue = 8f;
            speedSlider.value = 1f;
            speedSlider.onValueChanged.RemoveAllListeners();
            speedSlider.onValueChanged.AddListener(OnSpeedSliderChanged);
        }
        
        // Timeline slider
        if (timelineSlider != null)
        {
            timelineSlider.minValue = 0f;
            timelineSlider.maxValue = 1f;
            timelineSlider.value = 0f;
            timelineSlider.onValueChanged.RemoveAllListeners();
            timelineSlider.onValueChanged.AddListener(OnTimelineSliderChanged);
        }
        
        // Options toggles
        if (loopToggle != null)
        {
            loopToggle.onValueChanged.RemoveAllListeners();
            loopToggle.onValueChanged.AddListener(OnLoopToggleChanged);
        }
        
        if (interpolationToggle != null)
        {
            interpolationToggle.isOn = true;
            interpolationToggle.onValueChanged.RemoveAllListeners();
            interpolationToggle.onValueChanged.AddListener(OnInterpolationToggleChanged);
        }
        
        isInitialized = true;
        UpdateButtonStates(false);
    }
    
    void OnDataLoaded()
    {
        if (!isInitialized) return;
        
        // Update data display
        if (dataPointCountText != null)
        {
            int totalPoints = dataLoader.allDataCombined.Count;
            int device1Points = dataLoader.smartphone1Data.Count;
            int device2Points = dataLoader.smartphone2Data.Count;
            dataPointCountText.text = $"Total: {totalPoints} (D1: {device1Points}, D2: {device2Points})";
        }
        
        if (totalTimeText != null)
        {
            float duration = dataLoader.totalDuration;
            int minutes = Mathf.FloorToInt(duration / 60f);
            int seconds = Mathf.FloorToInt(duration % 60f);
            int milliseconds = Mathf.FloorToInt((duration % 1f) * 1000f);
            totalTimeText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
        }
        
        if (dataSourceText != null)
        {
            dataSourceText.text = "CSV Data Loaded";
        }
        
        UpdateButtonStates(false);
    }
    
    void OnPlaybackStateChanged(bool isPlaying)
    {
        UpdateButtonStates(isPlaying);
    }
    
    void OnPlaybackUpdate(float currentTime, List<LocationData> currentData)
    {
        if (!isInitialized) return;
        
        // Update timeline
        if (timelineSlider != null)
        {
            timelineSlider.value = playbackController.normalizedTime;
        }
        
        // Update current time display
        if (currentTimeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            int milliseconds = Mathf.FloorToInt((currentTime % 1f) * 1000f);
            currentTimeText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
        }
        
        // Update progress percentage
        if (progressPercentageText != null)
        {
            float percentage = playbackController.normalizedTime * 100f;
            progressPercentageText.text = $"{percentage:F1}%";
        }
        
        // Update current points display
        if (currentPointsText != null)
        {
            currentPointsText.text = $"Active Points: {currentData.Count}";
        }
        
        // Update speed display
        if (speedText != null)
        {
            speedText.text = $"{playbackController.playbackSpeed:F2}x";
        }
        
        if (speedSlider != null)
        {
            speedSlider.value = playbackController.playbackSpeed;
        }
    }
    
    void UpdateButtonStates(bool isPlaying)
    {
        if (playButton != null)
            playButton.interactable = !isPlaying && dataLoader.allDataCombined.Count > 0;
        
        if (pauseButton != null)
            pauseButton.interactable = isPlaying;
        
        if (stopButton != null)
            stopButton.interactable = isPlaying || playbackController.currentTime > 0;
    }
    
    void OnTimelineSliderChanged(float value)
    {
        if (playbackController != null)
        {
            playbackController.SeekToNormalizedTime(value);
        }
    }
    
    void OnSpeedSliderChanged(float value)
    {
        if (playbackController != null)
        {
            playbackController.SetPlaybackSpeed(value);
        }
    }
    
    void OnLoopToggleChanged(bool value)
    {
        if (playbackController != null)
        {
            playbackController.SetLoop(value);
        }
    }
    
    void OnInterpolationToggleChanged(bool value)
    {
        if (playbackController != null)
        {
            playbackController.SetInterpolation(value);
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (playbackController != null)
        {
            playbackController.OnPlaybackStateChanged -= OnPlaybackStateChanged;
            playbackController.OnPlaybackUpdate -= OnPlaybackUpdate;
        }
        
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded -= OnDataLoaded;
        }
    }
}