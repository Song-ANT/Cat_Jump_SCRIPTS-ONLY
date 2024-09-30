using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuddingPattern 
{
    public Queue<PuddingData> pattern(int stairs);
}
