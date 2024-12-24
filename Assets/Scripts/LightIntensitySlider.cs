using UnityEngine;
using UnityEngine.UI;

public class LightIntensitySlider : MonoBehaviour
{
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 1.0f;

    private Slider slider;
    
    private SettingsManager settingsManager;
    
    private void Awake()
    {
        var settingsManagerObject = GameObject.FindGameObjectWithTag("SettingsManager");
        if (settingsManagerObject != null)
        {
            settingsManager = settingsManagerObject.GetComponent<SettingsManager>();
        }
        else
        {
            MyLogger.Log("SettingsManager not found!!!");
        }
        
        
        // Get the Slider component
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("No Slider component found on the GameObject.");
            return;
        } 
        
        if (settingsManager == null)
        {
            Debug.LogError("No settingsManager component found");
            return;
        }

        // Set the slider's range
        slider.minValue = minValue;
        slider.maxValue = maxValue;
    }

    private void OnEnable()
    {
        // Load the saved value or set it to default
        float savedValue = settingsManager.LightIntensity;
        slider.value = savedValue;

        // Add a listener to save the value when the slider changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDisable()
    {
        // Remove the listener to prevent memory leaks
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Save the slider's value to PlayerPrefs
        settingsManager.LightIntensity = value;
    }

    public float GetCurrentValue()
    {
        return slider.value;
    }
}