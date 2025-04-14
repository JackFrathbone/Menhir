using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many ingame seconds pass for every single real second")]
    [SerializeField] float _timeScale;
    [Tooltip("Current ingame hour, game time starts at selected hour, readonly during play")]
    [SerializeField, Range(0, 24)] int _currentHour;

    [Header("Data")]
    private float _seconds;
    private float _totalSeconds;
    //The in-game tracked time
    private static TimeTracker trackedTime = new();

    //Updates the lighting every time the lighting timer is met
    public delegate void OnLightingUpdateDelegate(float timerPercent);
    public static OnLightingUpdateDelegate onLightingUpdate;

    //Updated the weather every hour
    public delegate void onWeatherUpdateDelegate();
    public static onWeatherUpdateDelegate onWeatherUpdate;

    private void Awake()
    {
        SetTrackedTime(0, _currentHour, 0);
        SetSecondsFromHour();
    }

    private void Update()
    {
        _seconds += _timeScale * Time.deltaTime;
        _totalSeconds += _timeScale * Time.deltaTime;

        if (_seconds >= 60)
        {
            _seconds = 0;
            trackedTime.TimeStep();

            //Update the lighting
            //onLightingUpdate?.Invoke((trackedTime.hours * 60 + trackedTime.minutes) / 1440f);
        }

        if (_totalSeconds >= 86400f)
        {
            _totalSeconds = 0;
        }


        //Update the tracked hour
        if (trackedTime.hours != _currentHour)
        {
            onWeatherUpdate?.Invoke();
            _currentHour = trackedTime.hours;
        }
    }

    private void FixedUpdate()
    {
        onLightingUpdate?.Invoke(_totalSeconds / 86400f);
    }

    public void AddHours(int hours)
    {
        trackedTime.hours += hours;
        trackedTime.TimeCheck();
        onWeatherUpdate?.Invoke();

        _currentHour = trackedTime.hours;
        SetSecondsFromHour();
    }

    public static void SetTrackedTime(int days, int hours, int minutes)
    {
        trackedTime.days = days;
        trackedTime.hours = hours;
        trackedTime.minutes = minutes;
    }

    public static int GetDays()
    {
        return trackedTime.days;
    }

    public static int GetHours()
    {
        return trackedTime.hours;
    }

    public static int GetMinutes()
    {
        return trackedTime.minutes;
    }

    private void SetSecondsFromHour()
    {
        _totalSeconds = (_currentHour * 60) * 60;
    }
}
