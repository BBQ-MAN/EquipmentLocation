using System.Collections.Generic;
using UnityEngine;

public class EquipmentMovementController : MonoBehaviour
{
    private List<EquipmentData> trajectory = new List<EquipmentData>();
    private string equipmentId;
    
    public void Initialize(string id, List<EquipmentData> data)
    {
        equipmentId = id;
        trajectory = data;
    }
    
    public void UpdatePosition(float currentTime, Terrain terrain)
    {
        if (trajectory == null || trajectory.Count == 0) return;
        
        // Find interpolation points
        EquipmentData before = null;
        EquipmentData after = null;
        
        for (int i = 0; i < trajectory.Count; i++)
        {
            if (trajectory[i].timestamp <= currentTime)
            {
                before = trajectory[i];
            }
            if (trajectory[i].timestamp >= currentTime && after == null)
            {
                after = trajectory[i];
                break;
            }
        }
        
        Vector3 targetPosition;
        
        // Smooth interpolation between points
        if (before != null && after != null && before != after)
        {
            float t = (currentTime - before.timestamp) / (after.timestamp - before.timestamp);
            t = Mathf.SmoothStep(0, 1, t); // Smooth interpolation
            targetPosition = Vector3.Lerp(before.position, after.position, t);
        }
        else if (before != null)
        {
            targetPosition = before.position;
        }
        else
        {
            return;
        }
        
        // Align to terrain
        if (terrain != null)
        {
            float terrainHeight = terrain.SampleHeight(targetPosition);
            targetPosition.y = terrainHeight + 0.5f;
        }
        
        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
        
        // Look at direction of movement
        if (before != null && after != null && before != after)
        {
            Vector3 direction = after.position - before.position;
            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }
    }
}