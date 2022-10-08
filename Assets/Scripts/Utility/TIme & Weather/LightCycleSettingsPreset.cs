using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Settings/Lighting/DayNightCycleLightingPreset")]
public class LightCycleSettingsPreset : ScriptableObject
{
    [Header("Lighting Colour Settings")]
    [Tooltip("The scenes overall ambient colour")]
    public Gradient ambientColour;
    [Tooltip("The specific colour of the directional light")]
    public Gradient directionalColour;

    [Header("Fog Settings")]
    [Tooltip("Sets the colour of the fog and background color")]
    public Gradient fogColour;
    [Tooltip("Used to set the fog distance, time value should go from 0 - 1")]
    public AnimationCurve fogIntensity;
}
