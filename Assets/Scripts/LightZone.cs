using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

using Random = UnityEngine.Random;
public class LightZone : MonoBehaviour
{
    [Header("Primary Lights")]
    public Light[] PrimaryLights;
    public Color[] PrimaryLightColors;
    private int PrimaryColorIndex;
    public float[] PrimaryLightIntensities;
    private int PrimaryIntensityIndex;
    [Space(10)]
    
    [Header("Primary Lights")]
    public Light[] SecondaryLights;
    public Color[] SecondaryLightColors;
    private int SecondaryColorIndex;
    public float[] SecondaryLightIntensities;
    private int SecondaryIntensityIndex;
    

    public void UpdateLightColor(bool secondary){
        if(secondary && SecondaryLightColors.Length > 0){
            int newIndex = SecondaryColorIndex + 1;
            SecondaryColorIndex = newIndex >= SecondaryLightColors.Length ? 0 : newIndex >= 0 ? newIndex : SecondaryLightColors.Length-1;
            print(SecondaryLightColors[SecondaryColorIndex]);
            for(int i = 0; i < SecondaryLights.Length; i++){
                SecondaryLights[i].color = SecondaryLightColors[SecondaryIntensityIndex];
            }
        }else if(!secondary){
            int newIndex = PrimaryColorIndex + 1;
            PrimaryColorIndex = newIndex >= PrimaryLightColors.Length ? 0 : newIndex >= 0 ? newIndex : PrimaryLightColors.Length-1;
            print(PrimaryLightColors[PrimaryColorIndex]);
            for(int i = 0; i < PrimaryLights.Length; i++){
                PrimaryLights[i].color = PrimaryLightColors[PrimaryColorIndex];
            }
        }
    }
    public void UpdateLightIntensity(bool secondary){
        if(secondary && SecondaryLightIntensities.Length > 0){
            int newIndex = SecondaryIntensityIndex + 1;
            SecondaryIntensityIndex = newIndex >= SecondaryLightIntensities.Length ? 0 : newIndex >= 0 ? newIndex : SecondaryLightIntensities.Length-1;
            for(int i = 0; i < SecondaryLights.Length; i++){
                SecondaryLights[i].intensity = SecondaryLightIntensities[SecondaryIntensityIndex];
            }
        }else if(!secondary){
            int newIndex = PrimaryIntensityIndex + 1;
            PrimaryIntensityIndex = newIndex >= PrimaryLightIntensities.Length ? 0 : newIndex >= 0 ? newIndex : PrimaryLightIntensities.Length-1;
            for(int i = 0; i < PrimaryLights.Length; i++){
                PrimaryLights[i].intensity = PrimaryLightIntensities[PrimaryIntensityIndex];
            }
        }
    }
    public void ToggleLights(bool secondary){
        if(secondary && SecondaryLights.Length > 0){
            for(int i = 0; i < SecondaryLights.Length; i++){
                SecondaryLights[i].gameObject.SetActive(!SecondaryLights[i].gameObject.activeSelf);
            }
        }else if(!secondary){
            for(int i = 0; i < PrimaryLights.Length; i++){
                PrimaryLights[i].gameObject.SetActive(!PrimaryLights[i].gameObject.activeSelf);
            }
        }
    }
}
