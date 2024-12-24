using UnityEngine;
using System;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private string playerPrefKey = "LightIntensityModifier";
    private float _lightIntensity = 1f;

    public event Action<float> OnLightIntensityModifierChange;
    
    public float LightIntensity
    {
        get
        {
            return _lightIntensity;
        }

        set
        {
            _lightIntensity = value;
            PlayerPrefs.SetFloat(playerPrefKey, value);
            PlayerPrefs.Save();
            OnLightIntensityModifierChange?.Invoke(value);
        }
    }
    
    private void Awake()
    {
        _lightIntensity = PlayerPrefs.GetFloat(playerPrefKey, 1f);
        OnLightIntensityModifierChange?.Invoke(_lightIntensity);
    }
}