using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExcelFileReader
{
    public static List<EquipmentData> LoadCSVFile(string filePath)
    {
        List<EquipmentData> dataList = new List<EquipmentData>();
        
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return dataList;
        }
        
        string dataString = File.ReadAllText(filePath);
        string[] lines = dataString.Split('\n');
        
        // Skip header line
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            
            string[] values = lines[i].Split(',');
            if (values.Length >= 5)
            {
                try
                {
                    string id = values[0].Trim();
                    float time = float.Parse(values[1].Trim());
                    float x = float.Parse(values[2].Trim());
                    float y = float.Parse(values[3].Trim());
                    float z = float.Parse(values[4].Trim());
                    string eventType = values.Length > 5 ? values[5].Trim() : "";
                    string description = values.Length > 6 ? values[6].Trim() : "";
                    
                    EquipmentData data = new EquipmentData(id, time, new Vector3(x, y, z), eventType, description);
                    dataList.Add(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing line " + i + ": " + e.Message);
                }
            }
        }
        
        // Sort by timestamp
        return dataList.OrderBy(d => d.timestamp).ToList();
    }
}