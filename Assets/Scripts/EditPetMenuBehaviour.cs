using UnityEngine;
using TMPro;

public class EditPetMenuBehaviour : MonoBehaviour
{
    public WardrobeBehaviour wardrobe;

    private GameObject hatCarousel;
    private TextMeshProUGUI hatCarouselText;

    private void OnEnable()
    {
        hatCarousel = GameObject.Find("HatCarousel");

        if (hatCarousel == null)
        {
            Debug.LogError("HatCarousel GameObject not found.");
        }
        else
        {
            // Locate TextMeshProUGUI within HatCarousel
            hatCarouselText = hatCarousel.GetComponentInChildren<TextMeshProUGUI>();

            if (hatCarouselText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found in HatCarousel.");
            }
        }

        // Attach event handler
        if (wardrobe != null)
        {
            wardrobe.OnHatAccessoryChange += HandleHatAccessoryChange;
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
            wardrobe.OnHatAccessoryChange -= HandleHatAccessoryChange;
        }
    }

    public void HandleHatAccessoryChange(string accessoryType)
    {
        if (hatCarouselText != null)
        {
            hatCarouselText.text = accessoryType;
        }
        else
        {
            Debug.LogError("Cannot update HatCarousel text: TextMeshProUGUI component not assigned.");
        }
    }
}