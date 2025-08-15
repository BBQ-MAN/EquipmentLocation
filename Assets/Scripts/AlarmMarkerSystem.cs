using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AlarmMarkerSystem : MonoBehaviour
{
    public Slider timelineSlider;
    public GameObject alarmMarkerPrefab;
    public Transform markerContainer;
    
    private List<AlarmEvent> alarmEvents = new List<AlarmEvent>();
    private List<GameObject> markers = new List<GameObject>();
    
    [System.Serializable]
    public class AlarmEvent
    {
        public float timestamp;
        public string description;
        public Color color = Color.red;
        
        public AlarmEvent(float time, string desc)
        {
            timestamp = time;
            description = desc;
        }
    }
    
    public void AddAlarmEvent(float time, string description)
    {
        alarmEvents.Add(new AlarmEvent(time, description));
        CreateMarker(time, description);
    }
    
    void CreateMarker(float time, string description)
    {
        if (timelineSlider == null || markerContainer == null) return;
        
        GameObject marker;
        if (alarmMarkerPrefab != null)
        {
            marker = Instantiate(alarmMarkerPrefab, markerContainer);
        }
        else
        {
            marker = new GameObject("AlarmMarker");
            marker.transform.SetParent(markerContainer);
            var img = marker.AddComponent<Image>();
            img.color = Color.red;
        }
        
        RectTransform rt = marker.GetComponent<RectTransform>();
        if (rt == null) rt = marker.AddComponent<RectTransform>();
        
        float normalizedTime = time / timelineSlider.maxValue;
        rt.anchorMin = new Vector2(normalizedTime, 0);
        rt.anchorMax = new Vector2(normalizedTime, 1);
        rt.sizeDelta = new Vector2(5, 0);
        rt.anchoredPosition = Vector2.zero;
        
        markers.Add(marker);
    }
    
    public void ClearMarkers()
    {
        foreach (var marker in markers)
        {
            Destroy(marker);
        }
        markers.Clear();
        alarmEvents.Clear();
    }
    
    public void SetupSampleAlarms()
    {
        AddAlarmEvent(25f, "Equipment Alert");
        AddAlarmEvent(50f, "Position Warning");
        AddAlarmEvent(75f, "System Notification");
    }
}