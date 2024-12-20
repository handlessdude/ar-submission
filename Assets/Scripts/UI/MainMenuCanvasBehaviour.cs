using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuCanvasBehaviour : MonoBehaviour
{
    private Button ContinueButton;
    
    private Button NewPetButton;
    
    private Button ClearButton;
    
    private GameObject MenuContainer;
    
    private GameObject LoadingCaption;

    private void Start()
    {
        
        var ContinueButtonObj = ComponentFinder.FindChildWithTag(gameObject, "ContinueButton");
        if (ContinueButtonObj != null)
        {
            ContinueButton = ContinueButtonObj.GetComponent<Button>();
            ContinueButton.onClick.AddListener(() =>
            {
                ToggleLoadingCaption();
                SceneManager.LoadScene("TamagochiScene");
            });
            if (UserSavesManager.GetNumberOfSaves() == 0)
            {
                DisableContinueButton();
            }
        }
        else
        {
            MyLogger.Log("ContinueButton not found");
        }
        
        var NewPetButtonObj = ComponentFinder.FindChildWithTag(gameObject, "NewPetButton");
        if (NewPetButtonObj != null)
        {
            NewPetButton = NewPetButtonObj.GetComponent<Button>();
            NewPetButton.onClick.AddListener(() =>
            {
                UserSavesManager.ClearAllSaves();
                ToggleLoadingCaption();
                SceneManager.LoadScene("TamagochiScene");
            });
        }
        else
        {
            MyLogger.Log("NewPetButton not found");
        }
        
        var ClearButtonObj = ComponentFinder.FindChildWithTag(gameObject, "ClearButton");
        if (ClearButtonObj != null)
        {
            ClearButton = ClearButtonObj.GetComponent<Button>();
            ClearButton.onClick.AddListener(() =>
            {
                UserSavesManager.ClearAllSaves();
                DisableContinueButton();
            });
        }
        else
        {
            MyLogger.Log("ClearButton not found");
        }
        
        MenuContainer = ComponentFinder.FindChildWithTag(gameObject, "MenuContainer");
        if (MenuContainer == null)
        {
            MyLogger.Log("MenuContainer not found");
        }
        
        LoadingCaption = ComponentFinder.FindChildWithTag(gameObject, "LoadingCaption");
        if (LoadingCaption == null)
        {
            MyLogger.Log("LoadingCaption not found");
        }
    }

    private void DisableContinueButton()
    {
        TMP_Text buttonText = ContinueButton.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.color = Color.gray;
        }
        ContinueButton.interactable = false;
    }

    private void ToggleLoadingCaption()
    {
        if (LoadingCaption != null && MenuContainer != null)
        {
            LoadingCaption.SetActive(true);
            MenuContainer.SetActive(false);
        }
    }
}