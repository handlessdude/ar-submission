using UnityEngine;

public class UIElementsManager : MonoBehaviour
{
    [SerializeField]
    public GameObject[] uiElements;
    public GameObject[] inverseVisibilityUiElements;
    
    public void OnModelVisibilityChanged(bool newVal)
    {
        var isModelVisibleStr = newVal ? "true" : "false";
        MyLogger.Log($"isModelVisible: {isModelVisibleStr}");
        
        foreach (var element in uiElements)
        {
            element.SetActive(newVal);
        }
        foreach (var element in inverseVisibilityUiElements)
        {
            element.SetActive(!newVal);
        }
    }
    
    // sorry
    public GameObject[] idleUiElements;
    public GameObject[] walkUiElements;
    
    public void OnAnimationStateChanged(AnimationState newVal)
    {
        var str = newVal == AnimationState.Walk ? "Walk" : "Idle";
        MyLogger.Log($"Animation state: {str}");
        
        switch(newVal) 
        {
            case AnimationState.Unknown:
                foreach (var element in walkUiElements)
                {
                    element.SetActive(false);
                }
                foreach (var element in idleUiElements)
                {
                    element.SetActive(false);
                }
                break;
            case AnimationState.Walk:
                foreach (var element in walkUiElements)
                {
                    element.SetActive(true);
                }
                foreach (var element in idleUiElements)
                {
                    element.SetActive(false);
                }
                break;
            case AnimationState.Idle:
                foreach (var element in walkUiElements)
                {
                    element.SetActive(false);
                }
                foreach (var element in idleUiElements)
                {
                    element.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
}