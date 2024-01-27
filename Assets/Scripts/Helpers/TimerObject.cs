using UnityEngine;

public class TimerObject
{
    public float Timer { get; private set; } = 0.0f;
    public float Duration { get; set; } = 0.0f;
    public bool IsOver
    {
        get
        {
            return Timer >= Duration;
        }
        
        private set { }
    }


    public TimerObject() { }

    public TimerObject(float duration)
    {
        Duration = duration;
    }

    /// <summary>
    /// Increments the timer by <paramref name="timeIncrement"/>
    /// <br>Returns true if timer has surpassed the expected <see cref="Duration"/></br>
    /// <br>Returns true if set <see cref="Duration"/> is 0.0</br>
    /// </summary>
    /// <param name="timeIncrement"></param>
    /// <returns><see cref="bool"/></returns>
    public bool Update(float timeIncrement)
    {
        if (IsOver)
            return true;

        if (Duration == 0.0f)
            return true;

        Timer += timeIncrement;
        return Timer >= Duration;
    }

    /// <summary>
    /// Increments the timer by <see cref="Time.deltaTime"/>
    /// <br>Returns true if timer has surpassed the expected <see cref="Duration"/></br>
    /// <br>Returns true if set <see cref="Duration"/> is 0.0</br>
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool Update()
    {
        return Update(Time.deltaTime);
    }

    /// <summary>
    /// Resets the timer to initial values
    /// </summary>
    public void Reset()
    {
        Timer = 0.0f;
        IsOver = false;
    }
}
