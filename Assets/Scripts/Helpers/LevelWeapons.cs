using System;
using System.Collections.Generic;

[Serializable]
public class LevelWeapons
{
    public int Level;
    public List<WeaponItem> AvailableWeapons = new List<WeaponItem>();
}
