using UnityEngine;

public class UIIntegrationEventManager : MonoBehaviour
{
    public TrackedImageModelManager trackedImageModelManager;
    public UIElementsManager uiElementsManager;

    void OnEnable()
    {
        if (trackedImageModelManager != null)
        {
            trackedImageModelManager.OnModelVisibilityChanged += HandleModelVisibilityChanged;
            trackedImageModelManager.OnAnimationStateChanged += HandleAnimationStateChanged;

        }
    }

    void OnDisable()
    {
        if (trackedImageModelManager != null)
        {
            trackedImageModelManager.OnModelVisibilityChanged -= HandleModelVisibilityChanged;
            trackedImageModelManager.OnAnimationStateChanged -= HandleAnimationStateChanged;

        }
    }

    private void HandleModelVisibilityChanged(bool newVal)
    {
        if (uiElementsManager != null)
        {
            uiElementsManager.OnModelVisibilityChanged(newVal);
        }
    }
    
    private void HandleAnimationStateChanged(AnimationState newVal)
    {
        if (uiElementsManager != null)
        {
            uiElementsManager.OnAnimationStateChanged(newVal);
        }
    }
}