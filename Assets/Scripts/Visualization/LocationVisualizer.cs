using System.Collections.Generic;
using UnityEngine;

public class LocationVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private Transform visualizationParent;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private float worldScale = 10.0f; // Increased for better visibility
    
    [Header("Point Appearance")]
    [SerializeField] private Color[] pointColors = {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };
    [SerializeField] private float pointSize = 0.3f;
    
    [Header("Trail Settings")]
    [SerializeField] private bool showTrails = true;
    [SerializeField] private float trailDuration = 5.0f;
    [SerializeField] private float trailWidth = 0.1f;
    
    [Header("Label Settings")]
    [SerializeField] private bool showLabels = true;
    [SerializeField] private GameObject labelPrefab;
    
    
    // References - managed internally
    private PlaybackController playbackController;
    private CSVDataLoader dataLoader;
    
    // Tracking objects for each point
    private Dictionary<int, GameObject> pointObjects = new Dictionary<int, GameObject>();
    private Dictionary<int, TrailRenderer> trailRenderers = new Dictionary<int, TrailRenderer>();
    private Dictionary<int, TextMesh> pointLabels = new Dictionary<int, TextMesh>();
    private Dictionary<int, List<Vector3>> positionHistory = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3> previousPositions = new Dictionary<int, Vector3>();
    
    // Bounds for visualization
    private Vector3 minBounds;
    private Vector3 maxBounds;
    
    void Start()
    {
        Debug.Log("LocationVisualizer: Start called");
        
        if (playbackController == null)
            playbackController = FindObjectOfType<PlaybackController>();
        
        if (dataLoader == null)
            dataLoader = FindObjectOfType<CSVDataLoader>();
        
        if (visualizationParent == null)
        {
            GameObject parent = new GameObject("Visualization Parent");
            visualizationParent = parent.transform;
        }
        
        // Note: PlaybackController calls OnPlaybackUpdate via reflection
        // No event subscription needed
        if (playbackController != null)
        {
            Debug.Log("LocationVisualizer: PlaybackController found");
        }
        else
        {
            Debug.LogWarning("LocationVisualizer: playbackController is null!");
        }
        
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded += OnDataLoaded;
            Debug.Log("LocationVisualizer: Subscribed to OnDataLoaded");
            
            // If data is already loaded, call OnDataLoaded manually
            if (dataLoader.allDataCombined != null && dataLoader.allDataCombined.Count > 0)
            {
                Debug.Log("LocationVisualizer: Data already loaded, calling OnDataLoaded manually");
                OnDataLoaded();
            }
        }
        else
        {
            Debug.LogWarning("LocationVisualizer: dataLoader is null!");
        }
        
        // Create default point prefab if not assigned
        if (pointPrefab == null)
        {
            pointPrefab = CreateDefaultPointPrefab();
            // Hide the template from the scene
            pointPrefab.SetActive(false);
        }
        
        if (labelPrefab == null && showLabels)
        {
            labelPrefab = CreateDefaultLabelPrefab();
            // Hide the template from the scene
            labelPrefab.SetActive(false);
        }
    }
    
    GameObject CreateDefaultPointPrefab()
    {
        GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        prefab.transform.localScale = Vector3.one * pointSize;
        prefab.name = "PointPrefab";
        
        // Don't deactivate - we'll use this as the template
        return prefab;
    }
    
    GameObject CreateDefaultLabelPrefab()
    {
        GameObject label = new GameObject("LabelPrefab");
        TextMesh textMesh = label.AddComponent<TextMesh>();
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.characterSize = 0.1f;
        
        // Don't deactivate - we'll use this as the template
        return label;
    }
    
    void OnDataLoaded()
    {
        Debug.Log("LocationVisualizer: OnDataLoaded called");
        
        // Calculate bounds
        CalculateBounds();
        
        // Clear existing visualization
        ClearVisualization();
        
        // Create one object for smartphone 1 (device ID 1)
        CreatePointObject(1);
        Debug.Log("LocationVisualizer: Created object for Device 1 (Smartphone 1)");
        
        // Create one object for smartphone 2 (device ID 2)
        CreatePointObject(2);
        Debug.Log("LocationVisualizer: Created object for Device 2 (Smartphone 2)");
        
        Debug.Log("LocationVisualizer: All device objects created");
    }
    
    void CalculateBounds()
    {
        if (dataLoader == null || dataLoader.allDataCombined.Count == 0)
            return;
        
        minBounds = new Vector3(float.MaxValue, 0, float.MaxValue);
        maxBounds = new Vector3(float.MinValue, 0, float.MinValue);
        
        foreach (var data in dataLoader.allDataCombined)
        {
            minBounds.x = Mathf.Min(minBounds.x, data.locationX);
            minBounds.z = Mathf.Min(minBounds.z, data.locationY);
            maxBounds.x = Mathf.Max(maxBounds.x, data.locationX);
            maxBounds.z = Mathf.Max(maxBounds.z, data.locationY);
        }
        
        Debug.Log($"Visualization bounds: Min({minBounds.x:F2}, {minBounds.z:F2}) Max({maxBounds.x:F2}, {maxBounds.z:F2})");
    }
    
    void CreatePointObject(int pointNumber)
    {
        // Create point object - properly instantiate from the prefab
        GameObject point;
        if (pointPrefab != null)
        {
            // If we have a prefab, make sure it's not in the scene
            if (pointPrefab.scene.IsValid())
            {
                // It's in the scene, so create a copy
                point = Instantiate(pointPrefab, visualizationParent);
            }
            else
            {
                // It's a proper prefab
                point = Instantiate(pointPrefab, visualizationParent);
            }
        }
        else
        {
            // Create a default sphere if no prefab
            point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.SetParent(visualizationParent);
            point.transform.localScale = Vector3.one * pointSize;
        }
        
        point.name = $"Point_{pointNumber}";
        point.SetActive(false); // Initially hidden until playback
        
        // Set color
        Renderer renderer = point.GetComponent<Renderer>();
        if (renderer != null)
        {
            int colorIndex = (pointNumber - 1) % pointColors.Length;
            renderer.material.color = pointColors[colorIndex];
        }
        
        // Add trail if enabled
        if (showTrails)
        {
            TrailRenderer trail = point.AddComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = point.GetComponentInChildren<TrailRenderer>();
            }
            
            if (trail != null)
            {
                trail.time = trailDuration;
                trail.startWidth = trailWidth;
                trail.endWidth = trailWidth * 0.3f;
                trail.material = new Material(Shader.Find("Sprites/Default"));
                
                int colorIndex = (pointNumber - 1) % pointColors.Length;
                Color trailColor = pointColors[colorIndex];
                trailColor.a = 0.5f;
                trail.startColor = trailColor;
                trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0.1f);
                
                trailRenderers[pointNumber] = trail;
            }
        }
        
        // Add label if enabled
        if (showLabels && labelPrefab != null)
        {
            GameObject label = Instantiate(labelPrefab, point.transform);
            label.name = $"Label_{pointNumber}";
            label.transform.localPosition = Vector3.up * 0.5f;
            label.SetActive(true);
            
            TextMesh textMesh = label.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = $"P{pointNumber}";
                int colorIndex = (pointNumber - 1) % pointColors.Length;
                textMesh.color = pointColors[colorIndex];
                pointLabels[pointNumber] = textMesh;
            }
        }
        
        pointObjects[pointNumber] = point;
        positionHistory[pointNumber] = new List<Vector3>();
        previousPositions[pointNumber] = Vector3.zero; // Initialize previous position
    }
    
    void OnPlaybackUpdate(float currentTime, List<LocationData> currentData)
    {
        Debug.Log($"LocationVisualizer: OnPlaybackUpdate called - Time: {currentTime:F2}, Devices: {currentData.Count}");
        
        // Hide all points first
        foreach (var kvp in pointObjects)
        {
            kvp.Value.SetActive(false);
        }
        
        // Update positions for current data based on deviceId
        foreach (var data in currentData)
        {
            int deviceId = data.deviceId;
            
            if (pointObjects.ContainsKey(deviceId))
            {
                GameObject point = pointObjects[deviceId];
                
                // Convert data coordinates to world position
                Vector3 worldPos = DataToWorldPosition(data.locationX, data.locationY);
                
                // Get previous position for this device
                Vector3 oldPos = previousPositions.ContainsKey(deviceId) ? previousPositions[deviceId] : worldPos;
                
                point.transform.position = worldPos;
                point.SetActive(true);
                
                // Rotate to face movement direction (X-axis forward)
                if (oldPos != worldPos && Vector3.Distance(oldPos, worldPos) > 0.01f)
                {
                    Vector3 moveDirection = (worldPos - oldPos);
                    moveDirection.y = 0; // Keep rotation only on horizontal plane
                    
                    if (moveDirection.magnitude > 0.01f)
                    {
                        moveDirection.Normalize();
                        
                        // Method 1: Direct rotation from Vector3.right to movement direction
                        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, moveDirection);
                        
                        // Apply rotation immediately
                        point.transform.rotation = targetRotation;
                        
                        float angle = Mathf.Atan2(moveDirection.z, moveDirection.x) * Mathf.Rad2Deg;
                        Debug.Log($"Device {deviceId}: Moving ({moveDirection.x:F2}, {moveDirection.z:F2}) Angle: {angle:F1}°");
                    }
                }
                
                // Store current position as previous for next update
                previousPositions[deviceId] = worldPos;
                
                // Log movement and rotation for each device
                float distance = Vector3.Distance(oldPos, worldPos);
                float rotationAngle = point.transform.rotation.eulerAngles.y;
                Debug.Log($"Device {deviceId}: Pos({data.locationX:F2}, {data.locationY:F2}) → World({worldPos.x:F2}, {worldPos.z:F2}) | Distance: {distance:F3}m | Rotation: {rotationAngle:F1}°");
                
                // Update position history using deviceId
                if (!positionHistory.ContainsKey(deviceId))
                {
                    positionHistory[deviceId] = new List<Vector3>();
                }
                
                var history = positionHistory[deviceId];
                if (history.Count == 0 || Vector3.Distance(history[history.Count - 1], worldPos) > 0.01f)
                {
                    history.Add(worldPos);
                    Debug.Log($"Device {deviceId}: Added to history (total: {history.Count} positions)");
                    
                    // Limit history size
                    int maxHistorySize = Mathf.CeilToInt(trailDuration * 30); // Assuming 30 FPS
                    if (history.Count > maxHistorySize)
                    {
                        history.RemoveAt(0);
                    }
                }
                
                // Update label if exists
                if (pointLabels.ContainsKey(deviceId))
                {
                    var label = pointLabels[deviceId];
                    if (label != null)
                    {
                        label.text = $"Device {deviceId}\n({data.locationX:F2}, {data.locationY:F2})";
                    }
                }
            }
            else
            {
                Debug.LogWarning($"LocationVisualizer: Device object not found for device ID {deviceId}");
            }
        }
        
        Debug.Log($"LocationVisualizer: Active devices after update: {currentData.Count}");
    }
    
    Vector3 DataToWorldPosition(float dataX, float dataY)
    {
        // Convert data coordinates to Unity world coordinates
        // Data uses X,Y but Unity uses X,Z for horizontal plane
        // worldScale amplifies the movement for better visibility
        Vector3 worldPos = new Vector3(
            dataX * worldScale,
            heightOffset,
            dataY * worldScale
        );
        
        // Log conversion for debugging (only log periodically to avoid spam)
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"DataToWorld: ({dataX:F2}, {dataY:F2}) → ({worldPos.x:F2}, {worldPos.z:F2}) with scale {worldScale}");
        }
        
        return worldPos;
    }
    
    public void SetShowTrails(bool show)
    {
        showTrails = show;
        foreach (var trail in trailRenderers.Values)
        {
            if (trail != null)
            {
                trail.enabled = show;
            }
        }
    }
    
    public void SetShowLabels(bool show)
    {
        showLabels = show;
        foreach (var label in pointLabels.Values)
        {
            if (label != null)
            {
                label.gameObject.SetActive(show);
            }
        }
    }
    
    public void SetWorldScale(float scale)
    {
        worldScale = scale;
    }
    
    public void ClearVisualization()
    {
        foreach (var obj in pointObjects.Values)
        {
            if (obj != null)
                Destroy(obj);
        }
        
        pointObjects.Clear();
        trailRenderers.Clear();
        pointLabels.Clear();
        positionHistory.Clear();
        previousPositions.Clear();
    }
    
    public void ClearTrails()
    {
        foreach (var trail in trailRenderers.Values)
        {
            if (trail != null)
            {
                trail.Clear();
            }
        }
        
        foreach (var history in positionHistory.Values)
        {
            history.Clear();
        }
    }
    
    void OnDestroy()
    {
        // Clean up
        
        if (dataLoader != null)
        {
            dataLoader.OnDataLoaded -= OnDataLoaded;
        }
        
        ClearVisualization();
    }
}