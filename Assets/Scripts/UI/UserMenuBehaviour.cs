using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;

public class UserMenuBehaviour : MonoBehaviour
{
    private Button BackButton;
    private Button NextButton;
    private GameObject ActionsTab;
    private GameObject WardrobeTab;
  	private TextMeshProUGUI modelValueText;
 
    private int currentTabIndex = 0;
    public GameObject[] tabs;

    private void Awake()
    {
		MyLogger.Log("Awake UserMenuBehaviour");

		var tabCarousel = ComponentFinder.FindChildWithTag(gameObject, "TabSwitcher");
        modelValueText = ComponentFinder.FindChildWithTag(tabCarousel, "ModelValue").GetComponent<TextMeshProUGUI>();
        BackButton = ComponentFinder.FindChildWithTag(gameObject, "BackButton").GetComponent<Button>();
        NextButton = ComponentFinder.FindChildWithTag(gameObject, "NextButton").GetComponent<Button>();

        ActionsTab = ComponentFinder.FindChildWithTag(gameObject, "Actions");
        WardrobeTab = ComponentFinder.FindChildWithTag(gameObject, "Wardrobe");
        
        
         if (WardrobeTab == null)
         {
             Debug.LogError("WardrobeTab not found as a child of Carousel.");
             MyLogger.Log("WardrobeTab not found as a child of Carousel.");
         }
         else
         {
             tabs = tabs.Append(WardrobeTab).ToArray();
         }
        if (ActionsTab == null)
        {
            Debug.LogError("ActionsTab not found as a child of Carousel.");
			MyLogger.Log("ActionsTab not found as a child of Carousel.");
        }
        else
        {
            tabs = tabs.Append(ActionsTab).ToArray();
        }
        
        if (BackButton == null)
        {
            Debug.LogError("BackButton not found as a child of Carousel.");
			MyLogger.Log("BackButton not found as a child of Carousel.");
        }
        else
        {
            BackButton.onClick.AddListener(OnBackButtonClick);
        }
        if (NextButton == null)
        {
            Debug.LogError("NextButton not found as a child of Carousel.");
			MyLogger.Log("NextButton not found as a child of Carousel.");
        }
        else
        {
            NextButton.onClick.AddListener(OnNextButtonClick);
        }
    }

    private void Start()
    {
        UpdateTabDisplay();
    }

 	public void OnBackButtonClick()
    {
        currentTabIndex = (currentTabIndex - 1 + tabs.Length) % tabs.Length;
        UpdateTabDisplay();
    }

    public void OnNextButtonClick()
    {
        currentTabIndex = (currentTabIndex + 1) % tabs.Length;
        UpdateTabDisplay();
    }

    private void UpdateTabDisplay()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(i == currentTabIndex);
        }

        if (modelValueText != null && currentTabIndex >= 0 && currentTabIndex < tabs.Length)
        {
            modelValueText.text = tabs[currentTabIndex].tag;
        }
    }
}