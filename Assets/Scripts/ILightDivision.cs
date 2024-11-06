using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ILightDivision : MonoBehaviour
{
    public GameObject[] lights;
    public Color[] lightColors;
    public int colorIndex;
    public float[] lightIntensities;
    public int intensityIndex;

    
    public void UpdateLightColor(int changeBy){
        int newIndex = colorIndex + changeBy;
        colorIndex = newIndex >= lightColors.Length ? 0 : newIndex >= 0 ? newIndex : lightColors.Length-1;
        foreach(GameObject light in lights){
            Light temp = light.GetComponent<Light>();
            temp.color = lightColors[colorIndex];
        }
    }
    public void UpdateLightIntensity(int changeBy){
        int newIndex = intensityIndex + changeBy;
        intensityIndex = newIndex >= lightIntensities.Length ? 0 : newIndex >= 0 ? newIndex : lightIntensities.Length-1;
        foreach(GameObject light in lights){
            Light temp = light.GetComponent<Light>();
            temp.intensity = lightIntensities[intensityIndex];
        }
    }
    public void ToggleLights(){
        foreach(GameObject light in lights){
            light.SetActive(!light.activeSelf);
        }
    }
}
