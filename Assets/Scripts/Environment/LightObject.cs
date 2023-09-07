public class LightObject : SwitchableObject
{
    public override void ElectricityAvailable(bool value)
    {
        HasElectricity = value;
    }

    public override void TurnOn()
    {
        if (!HasElectricity)
            return;

        IsOn = true;
    }

    public override void TurnOff()
    {
        IsOn = false;
    }

    public override bool Toggle()
    {
        if (!HasElectricity)
            return false;

        IsOn = !IsOn;

        return IsOn;
    }
}
