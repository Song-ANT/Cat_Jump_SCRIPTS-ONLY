using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuddingPattern_Base : IPuddingPattern
{
    protected float _defaultSpeed = 11f;
    protected float _defaultDelay = 1f;
    
    public static float ModifySpeed { get; set; } = 0;
    public static float ModifyDelay { get; set; } = 2f;

    protected static int _featherChance = 0;
    protected static int _shieldChance = 0;
    protected static int _skinChance = 0;

   
    public PuddingPattern_Base()
    {
        ModifySpeed = 0;
        ModifyDelay = 2f;
        _featherChance = 0;
        _shieldChance = 0;
        _skinChance = 0;
    }

    public virtual Queue<PuddingData> pattern(int stairs)
    {
        throw new System.NotImplementedException();
    }

    public void IncreaseChance(int increase)
    {
        _featherChance+=increase;
        _shieldChance+=increase;
    }


}
