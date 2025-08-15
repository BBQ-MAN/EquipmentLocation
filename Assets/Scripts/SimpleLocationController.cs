using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SimpleLocationController : MonoBehaviour
{
    public Button loadButton;
    public Slider timeSlider;
    public Button playButton;
    public Button pauseButton;
    public GameObject equipmentPrefab;
    public GameObject terrain;
    
    private bool isPlaying = false;
    private float currentTime = 0f;
    private float maxTime = 100f;
    private List<EquipmentData> dataList = new List<EquipmentData>();
    private Dictionary<string, GameObject> equipment = new Dictionary<string, GameObject>();
    
    void Start()
    {
        if (loadButton) loadButton.onClick.AddListener(LoadFile);
        if (playButton) playButton.onClick.AddListener(Play);
        if (pauseButton) pauseButton.onClick.AddListener(Pause);
        if (timeSlider) timeSlider.onValueChanged.AddListener(SetTime);
        
        CreateSampleData();
    }
    
    void LoadFile()
    {
        Debug.Log("File loading initiated");
        CreateSampleData();
    }
    
    void CreateSampleData()
    {
        dataList.Clear();
        for (int i = 0; i < 10; i++)
        {
            float time = i * 10f;
            Vector3 pos = new Vector3(i * 5, 1, i * 3);
            dataList.Add(new EquipmentData("EQ1", time, pos));
        }
        
        maxTime = 100f;
        if (timeSlider)
        {
            timeSlider.maxValue = maxTime;
            timeSlider.value = 0;
        }
        
        CreateEquipment();
    }
    
    void CreateEquipment()
    {
        if (!equipment.ContainsKey("EQ1"))
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = "Equipment_EQ1";
            obj.transform.localScale = Vector3.one * 2;
            equipment["EQ1"] = obj;
        }
    }
    
    void Play()
    {
        isPlaying = true;
    }
    
    void Pause()
    {
        isPlaying = false;
    }
    
    void SetTime(float value)
    {
        currentTime = value;
        UpdatePositions();
    }
    
    void Update()
    {
        if (isPlaying)
        {
            currentTime += Time.deltaTime * 10;
            if (currentTime > maxTime)
            {
                currentTime = maxTime;
                isPlaying = false;
            }
            if (timeSlider) timeSlider.value = currentTime;
            UpdatePositions();
        }
    }
    
    void UpdatePositions()
    {
        foreach (var eq in equipment)
        {
            var data = GetPositionAtTime(eq.Key, currentTime);
            if (data != null)
            {
                eq.Value.transform.position = data.position;
            }
        }
    }
    
    EquipmentData GetPositionAtTime(string id, float time)
    {
        var points = dataList.Where(d => d.equipmentId == id).OrderBy(d => d.timestamp).ToList();
        if (points.Count == 0) return null;
        
        EquipmentData before = null;
        EquipmentData after = null;
        
        foreach (var p in points)
        {
            if (p.timestamp <= time) before = p;
            if (p.timestamp >= time && after == null)
            {
                after = p;
                break;
            }
        }
        
        if (before != null && after != null && before != after)
        {
            float t = (time - before.timestamp) / (after.timestamp - before.timestamp);
            Vector3 pos = Vector3.Lerp(before.position, after.position, t);
            return new EquipmentData(id, time, pos);
        }
        
        return before;
    }
}