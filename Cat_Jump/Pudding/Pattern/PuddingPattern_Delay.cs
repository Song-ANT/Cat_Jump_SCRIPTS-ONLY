using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingPattern_Delay : PuddingPattern_Base
{
    public override Queue<PuddingData> pattern(int stairs)
    {
        Queue<PuddingData> puddingQueue = new Queue<PuddingData>();

        PuddingData data = new PuddingData();
        data.type = PuddingType.Normal;
        data.isRightMove = Random.Range(0, 2) == 0;
        data.stairs = stairs + 1;
        float speed = _defaultSpeed + Random.Range(-ModifySpeed, ModifySpeed);
        data.moveSpeed = (speed < 9.5f) ? 9.5f : (speed > 18) ? 18f : speed;
        data.waitTime = _defaultDelay + Random.Range(0, ModifyDelay - _defaultDelay);

        base.IncreaseChance(1);

        puddingQueue.Enqueue(data);

        return puddingQueue;
    }
}
