using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CarouselBehaviour : MonoBehaviour
{
    [SerializeField] private UnityEvent OnBackButtonClicked; // Callback for back button
    [SerializeField] private UnityEvent OnNextButtonClicked; // Callback for next button

    private Button BackButton;
    private Button NextButton;

    private void Awake()
    {
        BackButton = transform.Find("BackButton")?.GetComponent<Button>();
        NextButton = transform.Find("NextButton")?.GetComponent<Button>();

        if (BackButton == null)
        {
            Debug.LogError("BackButton not found as a child of Carousel.");
        }
        else
        {
            BackButton.onClick.AddListener(() => OnBackButtonClicked.Invoke());
        }

        if (NextButton == null)
        {
            Debug.LogError("NextButton not found as a child of Carousel.");
        }
        else
        {
            NextButton.onClick.AddListener(() => OnNextButtonClicked.Invoke());
        }
    }
}