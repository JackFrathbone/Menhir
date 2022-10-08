using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many ingame seconds pass for every single real second")]
    [SerializeField] float _timeScale;
    [Tooltip("How many ingame minutes pass before day/night lighting is updated")]
    [SerializeField, Range(0, 60)] float _lightingUpdateTime;
    [Tooltip("Current ingame hour, game time starts at selected hour, readonly during play")]
    [SerializeField, Range(0, 24)] int _currentHour;

    [Header("References")]
    [SerializeField] Light _directionalLight;
    [SerializeField] LightCycleSettingsPreset _settingsPreset;

    [Header("Data")]
    //The in-game tracked time
    private TimeSpan _trackedTime = TimeSpan.Zero;
    private TimeSpan _lightingUpdateTimer = TimeSpan.Zero;

    private void Start()
    {
        AddHours(_currentHour);
        _lightingUpdateTimer = _trackedTime + TimeSpan.FromMinutes(_lightingUpdateTime);
        UpdateLighting(_trackedTime.Hours / 24f);
    }

    private void Update()
    {
        if (_settingsPreset == null)
        {
            return;
        }

        _trackedTime += TimeSpan.FromSeconds(1 * _timeScale) * Time.deltaTime;
        _currentHour = _trackedTime.Hours;

        if (_lightingUpdateTimer <= _trackedTime)
        {
            UpdateLighting(_trackedTime.Hours / 24f);
            _lightingUpdateTimer = _trackedTime + TimeSpan.FromHours(1);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = _settingsPreset.ambientColour.Evaluate(timePercent);

        Camera.main.backgroundColor = _settingsPreset.fogColour.Evaluate(timePercent);
        RenderSettings.fogColor = _settingsPreset.fogColour.Evaluate(timePercent);
        RenderSettings.fogEndDistance = _settingsPreset.fogIntensity.Evaluate(timePercent);

        if (_directionalLight != null)
        {
            _directionalLight.color = _settingsPreset.directionalColour.Evaluate(timePercent);
            _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
        }
    }

    public void AddHours(float hours)
    {
        TimeSpan t = TimeSpan.FromHours(hours);

        _trackedTime += t;
    }

    public float GetHours()
    {
        return _trackedTime.Hours;
    }
}
