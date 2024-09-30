using System.Collections.Generic;
using UnityEngine;

public enum PuddingPatternType
{
    Normal,
    Feather,
    Delay,
    Double_EqualSpeed,
    Double_DifferSpeed,
    Triple
}


public class PuddingPatternFactory
{
    private Dictionary<PuddingPatternType, PuddingPattern_Base> _patterns;

    public static int PatternProb { get; set; } = 30;

    public PuddingPatternFactory()
    {
        _patterns = new Dictionary<PuddingPatternType, PuddingPattern_Base>
        {
            {PuddingPatternType.Normal, new PuddingPattern_Normal()},
            {PuddingPatternType.Feather, new PuddingPattern_Feather()},
            {PuddingPatternType.Delay, new PuddingPattern_Delay()},
            {PuddingPatternType.Double_EqualSpeed, new PuddingPattern_Double_EqualSpeed()},
            {PuddingPatternType.Double_DifferSpeed, new PuddingPattern_Double_DiferSpeed()},
            {PuddingPatternType.Triple, new PuddingPattern_Triple()}
        };
    }

    public Queue<PuddingData> SelectPattern(int stairs)
    {
        PuddingPatternType type = GetPatternType();

        if (_patterns.TryGetValue(type, out PuddingPattern_Base pattern))
        {
            return pattern.pattern(stairs);
        }
        else return null;
    }

    public Queue<PuddingData> SelectPattern(int stairs, PuddingPatternType type)
    {
        
        if (_patterns.TryGetValue(type, out PuddingPattern_Base pattern))
        {
            return pattern.pattern(stairs);
        }
        else return null;
    }

    private PuddingPatternType GetPatternType()
    {
        int rand = Random.Range(0, PatternProb);


        if (rand == 1)
        {
            return PuddingPatternType.Delay;
        }
        if (rand == 2)
        {
            return PuddingPatternType.Double_EqualSpeed;
        }
        if (rand == 3)
        {
            return PuddingPatternType.Double_DifferSpeed;
        }
        if (rand == 4)
        {
            return PuddingPatternType.Triple;
        }

        return PuddingPatternType.Normal;
    }
}
