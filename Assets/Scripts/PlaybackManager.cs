using UnityEngine;

/// <summary>
/// Main manager for coordinating the CSV data playback system
/// </summary>
public class PlaybackManager : MonoBehaviour
{
    // Core Components - managed internally
    private CSVDataLoader dataLoader;
    private TimelinePlaybackController playbackController;
    private LocationVisualizer visualizer;
    private PlaybackUIManager uiManager;
    
    [Header("Auto Setup")]
    [SerializeField] private bool autoCreateComponents = true;
    [SerializeField] private bool autoLoadDataOnStart = true;
    [SerializeField] private bool autoPlayOnLoad = false;
    
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
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        if (autoCreateComponents)
        {
            SetupComponents();
        }
    }
    
    void Start()
    {
        if (autoLoadDataOnStart && dataLoader != null)
        {
            dataLoader.LoadAllData();
            
            if (autoPlayOnLoad)
            {
                // Add a small delay to ensure everything is initialized
                Invoke(nameof(StartPlayback), 0.5f);
            }
        }
    }
    
    void SetupComponents()
    {
        // Find or create CSVDataLoader
        if (dataLoader == null)
        {
            dataLoader = GetComponent<CSVDataLoader>();
            if (dataLoader == null)
            {
                dataLoader = gameObject.AddComponent<CSVDataLoader>();
            }
        }
        
        // DISABLED: TimelinePlaybackController is deprecated
        // Using PlaybackController as the main playback controller
        /*
        if (playbackController == null)
        {
            playbackController = GetComponent<TimelinePlaybackController>();
            if (playbackController == null)
            {
                playbackController = gameObject.AddComponent<TimelinePlaybackController>();
            }
        }
        */
        
        // Find or create LocationVisualizer
        if (visualizer == null)
        {
            visualizer = FindObjectOfType<LocationVisualizer>();
            if (visualizer == null)
            {
                GameObject vizGO = new GameObject("LocationVisualizer");
                visualizer = vizGO.AddComponent<LocationVisualizer>();
            }
        }
        
        // Find or create PlaybackUIManager
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<PlaybackUIManager>();
            if (uiManager == null)
            {
                GameObject uiGO = new GameObject("PlaybackUIManager");
                uiManager = uiGO.AddComponent<PlaybackUIManager>();
            }
        }
        
        Debug.Log("PlaybackManager: All components set up successfully");
    }
    
    public void StartPlayback()
    {
        if (playbackController != null)
        {
            playbackController.Play();
        }
    }
    
    public void PausePlayback()
    {
        if (playbackController != null)
        {
            playbackController.Pause();
        }
    }
    
    public void StopPlayback()
    {
        if (playbackController != null)
        {
            playbackController.Stop();
        }
    }
    
    public void ReloadData()
    {
        if (dataLoader != null)
        {
            dataLoader.LoadAllData();
        }
    }
    
    public void SetPlaybackSpeed(float speed)
    {
        if (playbackController != null)
        {
            playbackController.SetPlaybackSpeed(speed);
        }
    }
    
    public void SeekToTime(float time)
    {
        if (playbackController != null)
        {
            playbackController.SeekToTime(time);
        }
    }
    
    public void SeekToNormalizedTime(float normalizedTime)
    {
        if (playbackController != null)
        {
            playbackController.SeekToNormalizedTime(normalizedTime);
        }
    }
    
    // Properties for external access
    public bool IsPlaying => playbackController != null && playbackController.isPlaying;
    public float CurrentTime => playbackController != null ? playbackController.currentTime : 0f;
    public float TotalDuration => dataLoader != null ? dataLoader.totalDuration : 0f;
    public float NormalizedTime => playbackController != null ? playbackController.normalizedTime : 0f;
    public int TotalDataPoints => dataLoader != null ? dataLoader.allDataCombined.Count : 0;
}