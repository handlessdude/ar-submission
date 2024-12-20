using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class WardrobeBehaviour : MonoBehaviour
{
    [SerializeField] private AccessoriesData accessoriesData;

    private GameObject pet;
    private GameObject headAccessorySlot;
    private GameObject neckAccessorySlot;
    private GameObject currentHeadAccessory;
    private GameObject currentNeckAccessory;

    private const string AccessorySaveFileName = "AccessorySaveData.json";
    
    public event Action<string> OnHatAccessoryChange;
    
    public event Action<string> OnNeckAccessoryChange;
    
    // New Methods
    [Serializable]
    private class AccessorySaveData
    {
        public int HeadAccessoryIndex;
        public int NeckAccessoryIndex;
    }
    
    private void Start()
    {
        // Find the pet game object by tag
        pet = GameObject.FindGameObjectWithTag("Pet");

        if (pet == null)
        {
            Debug.LogError("No game object with tag 'Pet' found.");
            return;
        }

        // Find accessory slots by tag
        headAccessorySlot = ComponentFinder.FindChildWithTag(pet, "HeadAccessorySlot");
        neckAccessorySlot = ComponentFinder.FindChildWithTag(pet, "NeckAccessorySlot");

        if (headAccessorySlot == null || neckAccessorySlot == null)
        {
            Debug.LogError("Accessory slots not found on pet.");
            return;
        }

        /*// Retrieve current head and face accessories (if any)
        currentHeadAccessory = headAccessorySlot.transform.childCount > 0
            ? headAccessorySlot.transform.GetChild(0).gameObject
            : null;
        
        currentNeckAccessory = neckAccessorySlot.transform.childCount > 0
            ? neckAccessorySlot.transform.GetChild(0).gameObject
            : null;*/
        
        LoadAccessoryData();
    }

    public void SetNextHeadAccessory() => SwitchHeadAccessory(true);
    public void SetPreviousHeadAccessory() => SwitchHeadAccessory(false);
    public void SetNextNeckAccessory() => SwitchNeckAccessory(true);
    public void SetPreviousNeckAccessory() => SwitchNeckAccessory(false);
    
    public void SwitchHeadAccessory(bool isNext = true)
    {
        SwitchAccessory(
            ref currentHeadAccessory,
            headAccessorySlot,
            accessoriesData.HeadAccessories,
            updatedAccessory => currentHeadAccessory = updatedAccessory,
            isNext
        );

        OnHatAccessoryChange?.Invoke(getAccessoryType(ref currentHeadAccessory));
    }

    public void SwitchNeckAccessory(bool isNext = true)
    {
        SwitchAccessory(
            ref currentNeckAccessory,
            neckAccessorySlot,
            accessoriesData.NeckAccessories,
            updatedAccessory => currentNeckAccessory = updatedAccessory,
            isNext
        );
        
        OnNeckAccessoryChange?.Invoke(getAccessoryType(ref currentNeckAccessory));
    }
    
    private void SwitchAccessory(
        ref GameObject currentAccessory,
        GameObject accessorySlot,
        List<GameObject> accessories,
        Action<GameObject> updateCurrentAccessory,
        bool isNext = true)
    {
        if (accessories == null || accessories.Count == 0)
        {
            Debug.LogError("Accessories list is empty.");
            return;
        }

        int threshold = isNext ? accessories.Count : -1;
        int step = isNext ? 1 : -1;
        int defaultAccessoryIndex = (accessories.Count + threshold) % accessories.Count;

        var capturedCurrentAccessory = currentAccessory;
        
        // Remove the current accessory
        if (capturedCurrentAccessory != null)
        {
            Destroy(capturedCurrentAccessory);

            // Find the current accessory's index in the list
            int currentIndex = accessories.FindIndex(
                accessory => capturedCurrentAccessory.name.StartsWith(accessory.name)
            );

            if (currentIndex == -1)
            {
                Debug.LogError("Accessory not found in list!");
                return;
            }

            int nextIndex = currentIndex + step;

            if (nextIndex == threshold)
            {
                currentAccessory = null;
            }
            else
            {
                currentAccessory = Instantiate(accessories[nextIndex], accessorySlot.transform);
            }
        }
        else
        {
            currentAccessory = Instantiate(accessories[defaultAccessoryIndex], accessorySlot.transform);
        }

        if (currentAccessory != null)
        {
            currentAccessory.transform.localPosition = Vector3.zero;
            currentAccessory.transform.localRotation = Quaternion.identity;
        }

        // Update the current accessory reference
        updateCurrentAccessory(currentAccessory);

        SaveAccessoryData();
    }
    
    private string getAccessoryType(ref GameObject currentAccessory)
    {
        if (currentAccessory == null)
        {
            return "None";
        }

        var accessoryIdentifier = currentAccessory.GetComponent<AccessoryIdentifier>();

        if (accessoryIdentifier == null)
        {
            Debug.LogError("Accessory identifier not found");
        }
        
        return accessoryIdentifier.AccessoryID;
    }
    
    // serialization
    
    public void SaveAccessoryData()
    {
        int headIndex = GetAccessoryIndex(accessoriesData.HeadAccessories, currentHeadAccessory);
        int neckIndex = GetAccessoryIndex(accessoriesData.NeckAccessories, currentNeckAccessory);

        AccessorySaveData saveData = new AccessorySaveData
        {
            HeadAccessoryIndex = headIndex,
            NeckAccessoryIndex = neckIndex
        };

        string json = JsonUtility.ToJson(saveData);
        string path = Path.Combine(Application.persistentDataPath, AccessorySaveFileName);
        File.WriteAllText(path, json);

        Debug.Log($"Accessory data saved: {json}");
    }
    
    public void LoadAccessoryData()
    {
        string path = Path.Combine(Application.persistentDataPath, AccessorySaveFileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found. No data to load.");
            return;
        }

        string json = File.ReadAllText(path);
        AccessorySaveData saveData = JsonUtility.FromJson<AccessorySaveData>(json);

        SetAccessoryFromIndex(saveData.HeadAccessoryIndex, accessoriesData.HeadAccessories, headAccessorySlot, ref currentHeadAccessory);
        SetAccessoryFromIndex(saveData.NeckAccessoryIndex, accessoriesData.NeckAccessories, neckAccessorySlot, ref currentNeckAccessory);

        Debug.Log("Accessory data loaded.");
    }

    public void ResetAccessories()
    {
        Destroy(currentHeadAccessory);
        Destroy(currentNeckAccessory);

        currentHeadAccessory = null;
        currentNeckAccessory = null;

        SaveAccessoryData();
        Debug.Log("Accessories reset.");
    }

    private int GetAccessoryIndex(List<GameObject> accessories, GameObject currentAccessory)
    {
        if (currentAccessory == null)
        {
            return -1;
        }

        return accessories.FindIndex(accessory => currentAccessory.name.StartsWith(accessory.name));
    }

    private void SetAccessoryFromIndex
    (
        int index,
        List<GameObject> accessories,
        GameObject accessorySlot,
        ref GameObject currentAccessory
    )
    {
        if (index < 0 || index >= accessories.Count)
        {
            currentAccessory = null;
            return;
        }

        currentAccessory = Instantiate(accessories[index], accessorySlot.transform);
        currentAccessory.transform.localPosition = Vector3.zero;
        currentAccessory.transform.localRotation = Quaternion.identity;
    }
}
