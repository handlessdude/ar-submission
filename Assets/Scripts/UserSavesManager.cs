using System;
using System.IO;
using UnityEngine;

public static class UserSavesManager
{
    private const string SaveFilePrefix = "AccessoryData_";
    private const string SaveFileExtension = ".json";

    private static string GetSaveDirectory()
    {
        return Application.persistentDataPath;
    }

    private static string GenerateSaveFilePath(int index)
    {
        return Path.Combine(GetSaveDirectory(), $"{SaveFilePrefix}{index}{SaveFileExtension}");
    }

    // Method 1: Create a new empty save with indices set to -1
    public static string CreateNewEmptySave()
    {
        int saveIndex = GetNumberOfSaves();
        string filePath = GenerateSaveFilePath(saveIndex);

        var emptySaveData = new UserSaveData { HeadAccessoryIndex = -1, NeckAccessoryIndex = -1 };
        File.WriteAllText(filePath, JsonUtility.ToJson(emptySaveData));

        MyLogger.Log($"New save created: {filePath}");
        return filePath;
    }

    // Method 2: Get the filename of the last created save
    public static string GetLastSaveFileName()
    {
        int saveCount = GetNumberOfSaves();
        if (saveCount == 0)
        {
            MyLogger.Log("No save files found.");
            return null;
        }

        return GenerateSaveFilePath(saveCount - 1);
    }

    // Method 3: Get the number of save files in the directory
    public static int GetNumberOfSaves()
    {
        string[] files = Directory.GetFiles(GetSaveDirectory(), $"{SaveFilePrefix}*{SaveFileExtension}");
        MyLogger.Log(string.Join(';', files));
        return files.Length;
    }

    // Method 4: Clear all save files in the directory
    public static void ClearAllSaves()
    {
        string[] files = Directory.GetFiles(GetSaveDirectory(), $"{SaveFilePrefix}*{SaveFileExtension}");
        foreach (string file in files)
        {
            File.Delete(file);
            MyLogger.Log($"Deleted save file: {file}");
        }

        MyLogger.Log("All save files cleared.");
    }
}
