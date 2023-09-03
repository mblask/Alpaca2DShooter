using System;

public interface IButton : IInteractable
{
    void SetOnPressed(Action onButtonPressed);

    void SetOnReleased(Action onButtonReleased);
}
