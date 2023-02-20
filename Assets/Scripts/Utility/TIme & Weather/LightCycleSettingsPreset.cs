using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Settings/Lighting/DayNightCycleLightingPreset")]
public class LightCycleSettingsPreset : ScriptableObject
{
    [Header("Lighting Colour Settings")]
    [Tooltip("The scenes overall ambient colour")]
    public Gradient ambientColour;
    [Tooltip("The scenes overall ambient colour when it is raining")]
    public Gradient ambientColourRain;
    [Tooltip("The default ambient colour when weather is disabled")]
    public Color interiorAmbientColour;

    [Tooltip("The scenes skybox upper colour")]
    public Gradient skyboxUpperColour;
    [Tooltip("The scenes skybox lower colour")]
    public Gradient skyboxLowerColour;

    [Header("Fog Settings")]
    [Tooltip("Sets the colour of the fog and background color")]
    public Gradient fogColour;
    [Tooltip("Used to set the fog distance, time value should go from 0 - 1")]
    public AnimationCurve fogIntensity;
}
