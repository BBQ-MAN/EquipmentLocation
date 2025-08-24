using System;
using UnityEngine;

[Serializable]
public class LocationData
{
    public string date;           // DATE (ex: 20250717)
    public string time;           // HHMM (ex: 1034)
    public int milliseconds;      // SEC (milliseconds)
    public int pointNumber;       // PTNUM (not used - always 1 in data)
    public float locationX;       // location (X) / m
    public float locationY;       // location (Y) / m
    public int deviceId;          // Device ID (1 for smartphone 1, 2 for smartphone 2)
    
    // Combined timestamp for easier sorting and playback
    public DateTime GetTimestamp()
    {
        if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
            return DateTime.MinValue;
            
        try
        {
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(4, 2));
            int day = int.Parse(date.Substring(6, 2));
            int hour = int.Parse(time.Substring(0, 2));
            int minute = int.Parse(time.Substring(2, 2));
            
            DateTime baseTime = new DateTime(year, month, day, hour, minute, 0);
            return baseTime.AddMilliseconds(milliseconds);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
    
    public float GetTimeInSeconds()
    {
        var timestamp = GetTimestamp();
        if (timestamp == DateTime.MinValue)
            return 0;
            
        // Convert to seconds from start of day
        return (float)(timestamp.TimeOfDay.TotalSeconds);
    }
}