using UnityEngine;

[System.Serializable]
public class EquipmentData
{
    public string equipmentId;
    public float timestamp;
    public Vector3 position;
    public string eventType;
    public string description;
    
    public EquipmentData(string id, float time, Vector3 pos, string evt = "", string desc = "")
    {
        equipmentId = id;
        timestamp = time;
        position = pos;
        eventType = evt;
        description = desc;
    }
}