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
        LoadAllData();
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
    
    private void LoadCSVData(string relativePath, List<LocationData> dataList, int deviceId)
    {
        string fullPath = Path.Combine(Application.dataPath, "..", relativePath);
        
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"CSV file not found: {fullPath}");
            return;
        }
        
        try
        {
            string[] lines = File.ReadAllLines(fullPath);
            
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
            
            Debug.Log($"Loaded {dataList.Count} data points from Device {deviceId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading CSV file {relativePath}: {e.Message}");
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
        
        // Get data for specific device
        List<LocationData> deviceData;
        if (deviceId == 1)
        {
            deviceData = smartphone1Data;
        }
        else if (deviceId == 2)
        {
            deviceData = smartphone2Data;
        }
        else
        {
            Debug.LogWarning($"CSVDataLoader: Invalid device ID {deviceId}");
            return null;
        }
        
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