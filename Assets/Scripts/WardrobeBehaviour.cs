using System.Collections.Generic;
using UnityEngine;
using System;

public class WardrobeBehaviour : MonoBehaviour
{
    [SerializeField] private AccessoriesData accessoriesData;

    private GameObject pet;
    private GameObject headAccessorySlot;
    private GameObject faceAccessorySlot;
    private GameObject currentHeadAccessory;
    private GameObject currentFaceAccessory;

    public event Action<string> OnHatAccessoryChange;
    
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
        faceAccessorySlot = ComponentFinder.FindChildWithTag(pet, "FaceAccessorySlot");

        if (headAccessorySlot == null || faceAccessorySlot == null)
        {
            Debug.LogError("Accessory slots not found on pet.");
            return;
        }

        // Retrieve current head and face accessories (if any)
        currentHeadAccessory = headAccessorySlot.transform.childCount > 0 ? headAccessorySlot.transform.GetChild(0).gameObject : null;
        currentFaceAccessory = faceAccessorySlot.transform.childCount > 0 ? faceAccessorySlot.transform.GetChild(0).gameObject : null;
    }

    public void SetNextHeadAccessory()
    {
        SwitchHeadAccessory(true);
    }    
    
    public void SetPreviousHeadAccessory()
    {
        SwitchHeadAccessory(false);
    }
    
    public void SwitchHeadAccessory(bool isNext = true)
    {
        if (accessoriesData.HeadAccessories == null || accessoriesData.HeadAccessories.Count == 0)
        {
            Debug.LogError("accessoriesData.HeadAccessories list is empty.");
            return;
        }

        var threshold = (isNext) ?  accessoriesData.HeadAccessories.Count : -1;
        var step = (isNext) ?  1 : -1;
        var defaultAccessoryIndex = (accessoriesData.HeadAccessories.Count + threshold) % accessoriesData.HeadAccessories.Count;
            
        // Remove the current head accessory
        if (currentHeadAccessory != null)
        {
            Destroy(currentHeadAccessory);
            
            // clones of prefab have "(Clone)" suffix in name
            int currentIndex = accessoriesData.HeadAccessories.FindIndex(
                accessory => currentHeadAccessory.name.StartsWith(accessory.name)
            );
            
            if (currentIndex == -1)
            {
                Debug.LogError($"Accessory not found in list!");
                return;
            }
            int nextIndex = currentIndex + step;
            Debug.Log($"nextIndex = {nextIndex}");
            if (nextIndex == threshold)
            {
                currentHeadAccessory = null;
                Debug.Log($"Setting currentHeadAccessory to null");
            }
            else
            {
                currentHeadAccessory = Instantiate(accessoriesData.HeadAccessories[nextIndex], headAccessorySlot.transform);
                Debug.Log($"Instantiate ${nextIndex}th accessory");
            }
            
        }
        else
        {
            currentHeadAccessory = Instantiate(accessoriesData.HeadAccessories[defaultAccessoryIndex], headAccessorySlot.transform);
            Debug.Log($"Instantiate ${defaultAccessoryIndex}th accessory as default");
        }

        if (currentHeadAccessory != null)
        {
            currentHeadAccessory.transform.localPosition = Vector3.zero;
            currentHeadAccessory.transform.localRotation = Quaternion.identity;
        }
        
        OnHatAccessoryChange?.Invoke(getAccessoryType());
    }

    private string getAccessoryType()
    {
        if (currentHeadAccessory == null)
        {
            return "None";
        }

        var accessoryIdentifier = currentHeadAccessory.GetComponent<AccessoryIdentifier>();

        if (accessoryIdentifier == null)
        {
            Debug.LogError("Accessory identifier not found");
        }
        
        return accessoryIdentifier.AccessoryID;
    }
}
