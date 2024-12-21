using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

public class ExitButtonBehaviour : MonoBehaviour
{
    private GameObject LoadingCaption;
    private GameObject UserMenu;
    
    private GameObject DraggingCaption;
    
    private void Start()
    {
        //  trackedImageModel = GameObject.FindGameObjectWithTag("TrackedImageModel");

        LoadingCaption = GameObject.FindGameObjectWithTag("LoadingCaption");
        if (LoadingCaption == null)
        {
            MyLogger.Log("LoadingCaption not found");
        }
        UserMenu = GameObject.FindGameObjectWithTag("UserMenu");
        if (UserMenu == null)
        {
            MyLogger.Log("UserMenu not found");
        }
        
        DraggingCaption = GameObject.FindGameObjectWithTag("DraggingCaption");
        if (DraggingCaption == null)
        {
            MyLogger.Log("DraggingCaption not found");
        }
        
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            ToggleLoadingCaption();
            SceneManager.LoadScene("MainMenuScene");
        });
    }
    
    private void ToggleLoadingCaption()
    {
        if (LoadingCaption != null && UserMenu != null)
        {
            DraggingCaption.SetActive(false);
            LoadingCaption.SetActive(true);
            UserMenu.SetActive(false);
        }
    }
}