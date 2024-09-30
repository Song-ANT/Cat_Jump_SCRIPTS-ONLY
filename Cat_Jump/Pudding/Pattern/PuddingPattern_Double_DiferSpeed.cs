using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingPattern_Double_DiferSpeed : PuddingPattern_Base
{
    public override Queue<PuddingData> pattern(int stairs)
    {
        Queue<PuddingData> puddingQueue = new Queue<PuddingData>();


        puddingQueue.Enqueue(new PuddingData()
        {
            type = PuddingType.Normal,
            isRightMove = Random.Range(0, 2) == 0,
            stairs = ++stairs,
            moveSpeed = _defaultSpeed,
            waitTime = _defaultDelay - Random.Range(0.1f, 0.2f)
        });

        float speed = _defaultSpeed + Random.Range(0, 3) + Random.Range(-ModifySpeed, ModifySpeed);
        puddingQueue.Enqueue(new PuddingData()
        {
            type = PuddingType.Normal,
            isRightMove = Random.Range(0, 2) == 0,
            stairs = ++stairs,

            moveSpeed = speed < 9.5f ? 9.5f : speed > 18f ? 18f : speed,
            waitTime = _defaultDelay
        });

        base.IncreaseChance(2);

        return puddingQueue;
    }
}
