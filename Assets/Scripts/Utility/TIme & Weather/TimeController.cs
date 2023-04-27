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
    private float seconds;
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
    }

    private void Update()
    {
        seconds += _timeScale * Time.deltaTime;

        if(seconds >= 60)
        {
            seconds = 0;
            trackedTime.TimeStep();

            //Update the lighting
            onLightingUpdate?.Invoke((trackedTime.hours * 60 + trackedTime.minutes) / 1440f);
        }

        
        //Update the tracked hour
        if (trackedTime.hours != _currentHour)
        {
            onWeatherUpdate?.Invoke();
            _currentHour = trackedTime.hours;
        }
    }


    public void AddHours(int hours)
    {
        trackedTime.hours += hours;
        trackedTime.TimeCheck();
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
}
