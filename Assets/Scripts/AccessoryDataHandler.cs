using System.IO;
using UnityEngine;

public static class AccessoryDataHandler
{
    public static string GetLastSaveFileName()
    {
        if (UserSavesManager.GetNumberOfSaves() == 0)
        {
            UserSavesManager.CreateNewEmptySave();
        }
        return UserSavesManager.GetLastSaveFileName();
    }
    
    public static void SaveData(int headIndex, int neckIndex)
    {
        UserSaveData data = new UserSaveData
        {
            HeadAccessoryIndex = headIndex,
            NeckAccessoryIndex = neckIndex
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetLastSaveFileName(), json);
        MyLogger.Log($"Accessory data saved: {json}");
    }

    public static UserSaveData LoadData()
    {
        string filePath = GetLastSaveFileName();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            UserSaveData data = JsonUtility.FromJson<UserSaveData>(json);
            MyLogger.Log($"Accessory data loaded: {json}");
            return data;
        }

        MyLogger.Log("Accessory data file not found. Returning default data.");
        return new UserSaveData(); // Return default data if no file exists
    }
}