using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CSVDataLoader : MonoBehaviour
{
    [Header("CSV File Settings")]
    [SerializeField] private string csvFilePath1 = "Documents/위츠측위 테스트 데이터/Raw Data/위치측위 Raw Data(스마트폰_1).csv";
    [SerializeField] private string csvFilePath2 = "Documents/위츠측위 테스트 데이터/Raw Data/위치측위 Raw Data(스마트폰_2).csv";
    
    [Header("Loaded Data")]
    public List<LocationData> smartphone1Data = new List<LocationData>();
    public List<LocationData> smartphone2Data = new List<LocationData>();
    public List<LocationData> allDataCombined = new List<LocationData>();
    
    public float totalDuration = 0f;
    public DateTime startTime;
    public DateTime endTime;
    
    public delegate void DataLoadedEvent();
    public event DataLoadedEvent OnDataLoaded;
    
    void Start()
    {
        // Don't auto-load data - wait for explicit LoadAllData() call
        Debug.Log("CSVDataLoader: Ready to load data. Waiting for user to click Load button.");
    }
    
    public void LoadAllData()
    {
        smartphone1Data.Clear();
        smartphone2Data.Clear();
        allDataCombined.Clear();
        
        // Load data from both CSV files
        LoadCSVData(csvFilePath1, smartphone1Data, 1);
        LoadCSVData(csvFilePath2, smartphone2Data, 2);
        
        // Combine and sort all data by timestamp
        allDataCombined.AddRange(smartphone1Data);
        allDataCombined.AddRange(smartphone2Data);
        allDataCombined = allDataCombined.OrderBy(d => d.GetTimestamp()).ToList();
        
        if (allDataCombined.Count > 0)
        {
            startTime = allDataCombined[0].GetTimestamp();
            endTime = allDataCombined[allDataCombined.Count - 1].GetTimestamp();
            totalDuration = (float)(endTime - startTime).TotalSeconds;
            
            Debug.Log($"Data loaded successfully!");
            Debug.Log($"Total points: {allDataCombined.Count}");
            Debug.Log($"Duration: {totalDuration:F2} seconds");
            Debug.Log($"Start time: {startTime:HH:mm:ss.fff}");
            Debug.Log($"End time: {endTime:HH:mm:ss.fff}");
            
            OnDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogWarning("No data loaded from CSV files!");
        }
    }
    
    public void LoadSingleFile(string filePath)
    {
        Debug.Log($"CSVDataLoader: LoadSingleFile called with: {filePath}");
        
        // Clear all data
        smartphone1Data.Clear();
        smartphone2Data.Clear();
        allDataCombined.Clear();
        
        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file not found: {filePath}");
            return;
        }
        
        // Load the single CSV file
        // All data will be treated as coming from a single device
        List<LocationData> singleFileData = new List<LocationData>();
        LoadCSVDataAbsolute(filePath, singleFileData, 1); // Use device ID 1 for single file
        
        // Process data by point number (PTNUM)
        // Group data by point number to create separate visualizations
        var groupedByPoint = singleFileData.GroupBy(d => d.pointNumber);
        
        Debug.Log($"Found {groupedByPoint.Count()} unique point numbers in the file");
        
        // Assign each unique point number to a virtual device for visualization
        int virtualDeviceId = 1;
        foreach (var group in groupedByPoint)
        {
            foreach (var data in group)
            {
                // Override deviceId to create separate visualizations for each point
                data.deviceId = virtualDeviceId;
                allDataCombined.Add(data);
            }
            virtualDeviceId++;
            
            Debug.Log($"Point {group.Key}: {group.Count()} data points assigned to Device {virtualDeviceId - 1}");
        }
        
        // Sort by timestamp
        allDataCombined = allDataCombined.OrderBy(d => d.GetTimestamp()).ToList();
        
        if (allDataCombined.Count > 0)
        {
            startTime = allDataCombined[0].GetTimestamp();
            endTime = allDataCombined[allDataCombined.Count - 1].GetTimestamp();
            totalDuration = (float)(endTime - startTime).TotalSeconds;
            
            Debug.Log($"Single CSV file loaded successfully!");
            Debug.Log($"File: {Path.GetFileName(filePath)}");
            Debug.Log($"Total points: {allDataCombined.Count}");
            Debug.Log($"Unique devices/points: {groupedByPoint.Count()}");
            Debug.Log($"Duration: {totalDuration:F2} seconds");
            Debug.Log($"Time range: {startTime:HH:mm:ss.fff} to {endTime:HH:mm:ss.fff}");
            
            OnDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogWarning("No data loaded from CSV file!");
        }
    }
    
    public void LoadFromPath(string folderPath)
    {
        Debug.Log($"CSVDataLoader: LoadFromPath called with: {folderPath}");
        
        smartphone1Data.Clear();
        smartphone2Data.Clear();
        allDataCombined.Clear();
        
        // Look for smartphone1.csv and smartphone2.csv in the selected folder
        string path1 = Path.Combine(folderPath, "smartphone1.csv");
        string path2 = Path.Combine(folderPath, "smartphone2.csv");
        
        bool file1Exists = File.Exists(path1);
        bool file2Exists = File.Exists(path2);
        
        Debug.Log($"Looking for files:");
        Debug.Log($"  {path1}: {(file1Exists ? "Found" : "Not found")}");
        Debug.Log($"  {path2}: {(file2Exists ? "Found" : "Not found")}");
        
        if (!file1Exists && !file2Exists)
        {
            Debug.LogError($"No CSV files found in {folderPath}. Please select a folder containing smartphone1.csv and/or smartphone2.csv");
            return;
        }
        
        // Load available files
        if (file1Exists)
        {
            LoadCSVDataAbsolute(path1, smartphone1Data, 1);
        }
        
        if (file2Exists)
        {
            LoadCSVDataAbsolute(path2, smartphone2Data, 2);
        }
        
        // Combine and sort all data by timestamp
        allDataCombined.AddRange(smartphone1Data);
        allDataCombined.AddRange(smartphone2Data);
        allDataCombined = allDataCombined.OrderBy(d => d.GetTimestamp()).ToList();
        
        if (allDataCombined.Count > 0)
        {
            startTime = allDataCombined[0].GetTimestamp();
            endTime = allDataCombined[allDataCombined.Count - 1].GetTimestamp();
            totalDuration = (float)(endTime - startTime).TotalSeconds;
            
            Debug.Log($"Data loaded successfully from external folder!");
            Debug.Log($"Total points: {allDataCombined.Count}");
            Debug.Log($"Duration: {totalDuration:F2} seconds");
            Debug.Log($"Start time: {startTime:HH:mm:ss.fff}");
            Debug.Log($"End time: {endTime:HH:mm:ss.fff}");
            
            OnDataLoaded?.Invoke();
        }
        else
        {
            Debug.LogWarning("No data loaded from CSV files!");
        }
    }
    
    private void LoadCSVData(string relativePath, List<LocationData> dataList, int deviceId)
    {
        string fullPath = Path.Combine(Application.dataPath, "..", relativePath);
        LoadCSVDataAbsolute(fullPath, dataList, deviceId);
    }
    
    private void LoadCSVDataAbsolute(string fullPath, List<LocationData> dataList, int deviceId)
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"CSV file not found: {fullPath}");
            return;
        }
        
        try
        {
            // Use FileShare.ReadWrite to handle files that might be open in other applications
            string[] lines;
            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string content = sr.ReadToEnd();
                lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
            
            // Skip header line
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                
                string[] values = line.Split(',');
                
                if (values.Length >= 6)
                {
                    LocationData data = new LocationData
                    {
                        date = values[0].Trim(),
                        time = values[1].Trim(),
                        milliseconds = int.Parse(values[2].Trim()),
                        pointNumber = int.Parse(values[3].Trim()),
                        locationX = float.Parse(values[4].Trim()),
                        locationY = float.Parse(values[5].Trim()),
                        deviceId = deviceId  // Set the device ID based on which CSV file
                    };
                    
                    dataList.Add(data);
                }
            }
            
            Debug.Log($"Loaded {dataList.Count} data points from Device {deviceId} ({Path.GetFileName(fullPath)})");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading CSV file {fullPath}: {e.Message}");
        }
    }
    
    public List<LocationData> GetDataAtTime(float elapsedSeconds)
    {
        if (allDataCombined.Count == 0) return new List<LocationData>();
        
        DateTime targetTime = startTime.AddSeconds(elapsedSeconds);
        List<LocationData> currentData = new List<LocationData>();
        
        // Find the most recent data for each point number
        Dictionary<int, LocationData> latestPoints = new Dictionary<int, LocationData>();
        
        foreach (var data in allDataCombined)
        {
            if (data.GetTimestamp() <= targetTime)
            {
                latestPoints[data.pointNumber] = data;
            }
            else
            {
                break;
            }
        }
        
        currentData.AddRange(latestPoints.Values);
        return currentData;
    }
    
    public LocationData GetInterpolatedDataForDevice(int deviceId, float elapsedSeconds)
    {
        DateTime targetTime = startTime.AddSeconds(elapsedSeconds);
        
        // Get data for specific device from allDataCombined
        List<LocationData> deviceData = allDataCombined
            .Where(d => d.deviceId == deviceId)
            .OrderBy(d => d.GetTimestamp())
            .ToList();
        
        if (deviceData.Count == 0)
        {
            Debug.LogWarning($"CSVDataLoader: No data found for device {deviceId}");
            return null;
        }
        
        // Find surrounding data points for interpolation
        LocationData before = null;
        LocationData after = null;
        
        for (int i = 0; i < deviceData.Count; i++)
        {
            if (deviceData[i].GetTimestamp() <= targetTime)
            {
                before = deviceData[i];
            }
            else
            {
                after = deviceData[i];
                break;
            }
        }
        
        if (before == null) 
        {
            Debug.Log($"CSVDataLoader: Device {deviceId} - No data before time {elapsedSeconds:F2}, returning first point");
            var first = deviceData[0];
            first.deviceId = deviceId;
            return first;
        }
        if (after == null) 
        {
            Debug.Log($"CSVDataLoader: Device {deviceId} - No data after time {elapsedSeconds:F2}, returning last point");
            before.deviceId = deviceId;
            return before;
        }
        
        // Interpolate between before and after
        float t1 = (float)(before.GetTimestamp() - startTime).TotalSeconds;
        float t2 = (float)(after.GetTimestamp() - startTime).TotalSeconds;
        float t = Mathf.InverseLerp(t1, t2, elapsedSeconds);
        
        LocationData interpolated = new LocationData
        {
            date = before.date,
            time = before.time,
            milliseconds = before.milliseconds,
            pointNumber = before.pointNumber,
            locationX = Mathf.Lerp(before.locationX, after.locationX, t),
            locationY = Mathf.Lerp(before.locationY, after.locationY, t),
            deviceId = deviceId
        };
        
        // Log interpolation details periodically
        if (Time.frameCount % 60 == 0) // Log every 2 seconds at 30fps
        {
            Debug.Log($"CSVDataLoader: Device {deviceId} interpolated - " +
                     $"Time: {elapsedSeconds:F2}s, t={t:F3}, " +
                     $"Before: ({before.locationX:F2}, {before.locationY:F2}), " +
                     $"After: ({after.locationX:F2}, {after.locationY:F2}), " +
                     $"Result: ({interpolated.locationX:F2}, {interpolated.locationY:F2})");
        }
        
        return interpolated;
    }
    
    public LocationData GetInterpolatedDataForPoint(int pointNumber, float elapsedSeconds)
    {
        DateTime targetTime = startTime.AddSeconds(elapsedSeconds);
        
        var pointData = allDataCombined.Where(d => d.pointNumber == pointNumber).ToList();
        if (pointData.Count == 0) 
        {
            Debug.LogWarning($"CSVDataLoader: No data found for point {pointNumber}");
            return null;
        }
        
        // Find surrounding data points for interpolation
        LocationData before = null;
        LocationData after = null;
        
        for (int i = 0; i < pointData.Count; i++)
        {
            if (pointData[i].GetTimestamp() <= targetTime)
            {
                before = pointData[i];
            }
            else
            {
                after = pointData[i];
                break;
            }
        }
        
        if (before == null) 
        {
            Debug.Log($"CSVDataLoader: Point {pointNumber} - No data before time {elapsedSeconds:F2}, returning first point");
            return pointData[0];
        }
        if (after == null) 
        {
            Debug.Log($"CSVDataLoader: Point {pointNumber} - No data after time {elapsedSeconds:F2}, returning last point");
            return before;
        }
        
        // Interpolate between before and after
        float t1 = (float)(before.GetTimestamp() - startTime).TotalSeconds;
        float t2 = (float)(after.GetTimestamp() - startTime).TotalSeconds;
        float t = Mathf.InverseLerp(t1, t2, elapsedSeconds);
        
        LocationData interpolated = new LocationData
        {
            date = before.date,
            time = before.time,
            milliseconds = before.milliseconds,
            pointNumber = pointNumber,
            locationX = Mathf.Lerp(before.locationX, after.locationX, t),
            locationY = Mathf.Lerp(before.locationY, after.locationY, t)
        };
        
        // Log interpolation details periodically
        if (Time.frameCount % 60 == 0) // Log every 2 seconds at 30fps
        {
            Debug.Log($"CSVDataLoader: Point {pointNumber} interpolated - " +
                     $"Time: {elapsedSeconds:F2}s, t={t:F3}, " +
                     $"Before: ({before.locationX:F2}, {before.locationY:F2}), " +
                     $"After: ({after.locationX:F2}, {after.locationY:F2}), " +
                     $"Result: ({interpolated.locationX:F2}, {interpolated.locationY:F2})");
        }
        
        return interpolated;
    }
}