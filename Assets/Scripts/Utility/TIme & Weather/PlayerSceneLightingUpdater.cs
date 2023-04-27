using UnityEngine;

[ExecuteInEditMode]
public class PlayerSceneLightingUpdater : MonoBehaviour
{
#if UNITY_EDITOR
    [InspectorButton("RefreshPlayerLighting")]
    public bool refresh;

    [SerializeField] LightCycleSettingsPreset _settingsPreset;

    private void RefreshPlayerLighting()
    {
        RenderSettings.ambientLight = _settingsPreset.interiorAmbientColour;

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("Reloaded Player Scene Lighting!");
    }
#endif
}
