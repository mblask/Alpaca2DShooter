using System;
using UnityEngine;

[Serializable]
public class TimerObject
{
    private float _timer = 0.0f;
    private float _duration = 0.0f;

    public TimerObject()
    {
        _timer = 0.0f;
    }

    public TimerObject(float duration)
    {
        _timer = 0.0f;
        _duration = duration;
    }

    public float GetTimer()
    {
        return _timer;
    }

    public float GetDuration()
    {
        return _duration;
    }

    public float GetTimerDurationRatio()
    {
        return _timer / _duration;
    }

    public bool IsOver()
    {
        return _timer > _duration;
    }

    /// <summary>
    /// Increments the timer by <paramref name="timeIncrement"/>
    /// <br>Returns true if timer has surpassed the expected <see cref="_duration"/></br>
    /// <br>Returns true if set <see cref="_duration"/> is 0.0</br>
    /// </summary>
    /// <param name="timeIncrement"></param>
    /// <returns><see cref="bool"/></returns>
    public bool Update(float timeIncrement)
    {
        if (IsOver())
            return true;

        if (_duration == 0.0f)
            return true;

        _timer += timeIncrement;
        return _timer >= _duration;
    }

    /// <summary>
    /// Increments the timer by <see cref="Time.deltaTime"/>
    /// <br>Returns true if timer has surpassed the expected <see cref="_duration"/></br>
    /// <br>Returns true if set <see cref="_duration"/> is 0.0</br>
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
        _timer = 0.0f;
    }
}
