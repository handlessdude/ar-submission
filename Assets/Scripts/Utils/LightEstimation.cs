/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
public class LightEstimation : MonoBehaviour
{
    public ARCameraManager ARCameraManager;
    public Light Light;
    private SettingsManager settingsManager;

    private float lightIntensityModifier = 1f;
    
    private float initialLightIntensity = 1f;
    
    private void Start()
    {
        ARCameraManager.frameReceived += FrameReceived;

        Light = GetComponent<Light>();
        initialLightIntensity = Light.intensity;
        
        InitializeSettingsManager();
    }

    private void InitializeSettingsManager()
    {
        var settingsManagerObject = GameObject.FindGameObjectWithTag("SettingsManager");
        if (settingsManagerObject != null)
        {
            settingsManager = settingsManagerObject.GetComponent<SettingsManager>();
            UpdateLightIntensityModifier(settingsManager.LightIntensity);
            settingsManager.OnLightIntensityModifierChange += UpdateLightIntensityModifier;
            UpdateLightIntensityFromInitial();
        }
        else
        {
            MyLogger.Log("SettingsManager not found!");
        }
    }

    private void onDestroy()
    {
        settingsManager.OnLightIntensityModifierChange -= UpdateLightIntensityModifier;
    }
    
    private void UpdateLightIntensityModifier(float value)
    {
        lightIntensityModifier = value;
    }
    
    private void UpdateLightIntensityFromInitial()
    {
        MyLogger.Log($"{lightIntensityModifier}");
        Light.intensity = initialLightIntensity * lightIntensityModifier;
    }
    
    private void FrameReceived(ARCameraFrameEventArgs args)
    {
        ARLightEstimationData lightEstimation = args.lightEstimation;
        
        if (lightEstimation.averageBrightness.HasValue)
        {
            Light.intensity = lightEstimation.averageBrightness.Value * lightIntensityModifier;
        }
        else if (lightEstimation.mainLightIntensityLumens.HasValue)
        {
            Light.intensity = lightEstimation.averageMainLightBrightness.Value * lightIntensityModifier;
        }
        else
        {
            UpdateLightIntensityFromInitial();
        }
        
        if (lightEstimation.averageColorTemperature.HasValue)
        {
            Light.colorTemperature = lightEstimation.averageColorTemperature.Value;
        }

        if (lightEstimation.colorCorrection.HasValue)
        {
            Light.color = lightEstimation.colorCorrection.Value;
        }

        if (lightEstimation.mainLightDirection.HasValue)
        {
            Light.transform.rotation = Quaternion.LookRotation(lightEstimation.mainLightDirection.Value);
        }

        if (lightEstimation.mainLightColor.HasValue)
        {
            Light.color = lightEstimation.mainLightColor.Value;
        }
        
        if (lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = lightEstimation.ambientSphericalHarmonics.Value;
        }
    }
}
