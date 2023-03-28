using UnityEngine;

public class TimeTracker
{
    public int minutes;
    public int hours;
    public int days;

    //How time increases every second
    public void TimeStep()
    {
        minutes++;
        TimeCheck();
    }

    //Use when updated any time
    public void TimeCheck()
    {
        if (minutes >= 60)
        {
            minutes = 0;
            hours++;
        }

        if (hours >= 24)
        {
            hours = 0;
            days++;
        }
    }
}
