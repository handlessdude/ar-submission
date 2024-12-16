using UnityEngine;
using TMPro;

public class EditPetMenuBehaviour : MonoBehaviour
{
    public WardrobeBehaviour wardrobe;

    private GameObject headAccessoryCarousel;
    private TextMeshProUGUI headAccessoryCarouselText;   

    private GameObject neckAccessoryCarousel;
    private TextMeshProUGUI neckAccessoryCarouselText;

    private void OnEnable()
    {
        InitializeHatCarousel();
        InitializeNeckAccessoryCarousel();
        
        if (wardrobe != null)
        {
            wardrobe.OnHatAccessoryChange += HandleHeadAccessoryChange;
            wardrobe.OnNeckAccessoryChange += HandleNeckAccessoryChange;
        }
        else
        {
            Debug.LogError("WardrobeBehaviour component not identified.");
        }
    }
    
    private void OnDisable()
    {
        // Detach event handler to prevent memory leaks
        if (wardrobe != null)
        {
            wardrobe.OnHatAccessoryChange -= HandleHeadAccessoryChange;
            wardrobe.OnNeckAccessoryChange -= HandleNeckAccessoryChange;
        }
    }

    private void InitializeHatCarousel() 
    {
         headAccessoryCarousel = GameObject.Find("HeadAccessoryCarousel");

        if (headAccessoryCarousel == null)
        {
            Debug.LogError("headAccessoryCarousel GameObject not found.");
        }
        else
        {
            // Locate TextMeshProUGUI within headAccessoryCarousel
            headAccessoryCarouselText = headAccessoryCarousel.GetComponentInChildren<TextMeshProUGUI>();

            if (headAccessoryCarouselText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found in headAccessoryCarousel.");
            }
        }
    }
    
    private void InitializeNeckAccessoryCarousel() 
    {
        neckAccessoryCarousel = GameObject.Find("NeckAccessoryCarousel");

        if (neckAccessoryCarousel == null)
        {
            Debug.LogError("NeckAccessoryCarousel GameObject not found.");
        }
        else
        {
            neckAccessoryCarouselText = neckAccessoryCarousel.GetComponentInChildren<TextMeshProUGUI>();

            if (neckAccessoryCarouselText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found in NeckAccessoryCarousel.");
            }
        }
    }


    public void HandleHeadAccessoryChange(string accessoryType)
    {
        if (headAccessoryCarouselText != null)
        {
            headAccessoryCarouselText.text = accessoryType;
        }
        else
        {
            Debug.LogError("Cannot update HeadAccessoryCarouselText text: TextMeshProUGUI component not assigned.");
        }
    }
    
    public void HandleNeckAccessoryChange(string accessoryType)
    {
        if (neckAccessoryCarouselText != null)
        {
            neckAccessoryCarouselText.text = accessoryType;
        }
        else
        {
            Debug.LogError("Cannot update NeckAccessoryCarousel text: TextMeshProUGUI component not assigned.");
        }
    }
}