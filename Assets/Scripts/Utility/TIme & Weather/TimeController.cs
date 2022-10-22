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

    [Header("Data")]
    //The in-game tracked time
    public static TimeSpan trackedTime = TimeSpan.Zero;
    private TimeSpan _lightingUpdateTimer = TimeSpan.Zero;

    //Updates the lighting every time the lighting timer is met
    public delegate void OnLightingUpdateDelegate(float timerPercent);
    public static OnLightingUpdateDelegate onLightingUpdate;

    //Updated the weather every hour
    public delegate void onWeatherUpdateDelegate();
    public static onWeatherUpdateDelegate onWeatherUpdate;

    private void Start()
    {
        AddHours(_currentHour);
    }

    private void Update()
    {
        trackedTime += TimeSpan.FromSeconds(1 * _timeScale) * Time.deltaTime;

        //Update the tracked hour
        if(trackedTime.Hours != _currentHour)
        {
            onWeatherUpdate?.Invoke();
            _currentHour = trackedTime.Hours;
        }

        if (_lightingUpdateTimer <= trackedTime)
        {
            onLightingUpdate?.Invoke((trackedTime.Hours*60 + trackedTime.Minutes) / 1440f);
            _lightingUpdateTimer = trackedTime + TimeSpan.FromMinutes(_lightingUpdateTime);
        }
    }


    public void AddHours(float hours)
    {
        TimeSpan t = TimeSpan.FromHours(hours);

        trackedTime += t;
    }

    public float GetHours()
    {
        return trackedTime.Hours;
    }
}
