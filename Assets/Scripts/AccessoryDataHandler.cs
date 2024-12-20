using System.IO;
using UnityEngine;

public class AccessoryDataHandler : MonoBehaviour
{
    private const string FileName = "AccessoryData.json";

    [System.Serializable]
    public class AccessoryIndices
    {
        public int HeadAccessoryIndex = -1;
        public int NeckAccessoryIndex = -1;
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, FileName);
    }

    public void SaveData(int headIndex, int neckIndex)
    {
        AccessoryIndices data = new AccessoryIndices
        {
            HeadAccessoryIndex = headIndex,
            NeckAccessoryIndex = neckIndex
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetFilePath(), json);
        Debug.Log($"Accessory data saved: {json}");
    }

    public AccessoryIndices LoadData()
    {
        string filePath = GetFilePath();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            AccessoryIndices data = JsonUtility.FromJson<AccessoryIndices>(json);
            Debug.Log($"Accessory data loaded: {json}");
            return data;
        }

        Debug.LogWarning("Accessory data file not found. Returning default data.");
        return new AccessoryIndices(); // Return default data if no file exists
    }
}