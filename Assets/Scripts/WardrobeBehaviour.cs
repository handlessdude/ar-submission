using System.Collections.Generic;
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

    public event Action<string> OnHatAccessoryChange;
    
    public event Action<string> OnNeckAccessoryChange;
    
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

        // Retrieve current head and face accessories (if any)
        currentHeadAccessory = headAccessorySlot.transform.childCount > 0 ? headAccessorySlot.transform.GetChild(0).gameObject : null;
        currentNeckAccessory = neckAccessorySlot.transform.childCount > 0 ? neckAccessorySlot.transform.GetChild(0).gameObject : null;
    }

    public void SetNextHeadAccessory()
    {
        SwitchHeadAccessory(true);
    }    
    
    public void SetPreviousHeadAccessory()
    {
        SwitchHeadAccessory(false);
    }    
    
    public void SetNextNeckAccessory()
    {
        SwitchNeckAccessory(true);
    }    
    
    public void SetPreviousNeckAccessory()
    {
        SwitchNeckAccessory(false);
    }
    
    public void SwitchHeadAccessory(bool isNext = true)
    {
        SwitchAccessory(
            ref currentHeadAccessory,
            headAccessorySlot,
            accessoriesData.HeadAccessories,
            updatedAccessory => currentHeadAccessory = updatedAccessory,
            isNext
        );

        OnHatAccessoryChange?.Invoke(getAccessoryType());
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
        
        OnNeckAccessoryChange?.Invoke(getAccessoryType());
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
