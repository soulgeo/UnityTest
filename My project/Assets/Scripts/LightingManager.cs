using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField] private List<Light> lamppostLights;
    [SerializeField] private Material lamppostMat;
    // Variables
    [SerializeField, Range(0, 1)] private float rateOfChange;
    [SerializeField, Range(0, 24)] private float lightsOffTime = 6;
    [SerializeField, Range(0, 24)] private float lightsOnTime = 20;
    [SerializeField, Range(0, 24)] private float timeOfDay;
    

    private float tempTime = 0;

    private void Update()
    {
        if (preset == null)
            return;

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * rateOfChange;
            tempTime += Time.deltaTime * rateOfChange;
            timeOfDay %= 24;
        }
        UpdateLighting(timeOfDay / 24f);
        //if (tempTime > 1)
        //{
            //DynamicGI.UpdateEnvironment();
           // tempTime = 0;
        //}
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            //DynamicGI.UpdateEnvironment();
        }

        if (timePercent > lightsOnTime/24f || timePercent < lightsOffTime/24f)
        {
            lamppostMat.EnableKeyword("_EMISSION");
            foreach (Light light in lamppostLights)
            {
                light.intensity = 12;
            }
        }
        else
        {
            lamppostMat.DisableKeyword("_EMISSION");
            foreach (Light light in lamppostLights)
            {
                light.intensity = 0;
            }
        }
    }

    private void LateUpdate()
    {
        DynamicGI.UpdateEnvironment();
    }

    private void OnValidate()
    {
        if (directionalLight != null)
            return;

        if (RenderSettings.sun != null)
            directionalLight = RenderSettings.sun;
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
