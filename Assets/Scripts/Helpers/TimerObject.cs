public class TimerObject
{
    public float Timer { get; private set; } = 0.0f;
    public float Duration { get; set; } = 0.0f;
    public bool IsOver { get; private set; }

    public void Update(float timeIncrement)
    {
        if (IsOver)
            return;

        Timer += timeIncrement;

        if (Timer >= Duration)
            IsOver = true;
    }

    public void Reset()
    {
        Timer = 0.0f;
        IsOver = false;
    }
}
