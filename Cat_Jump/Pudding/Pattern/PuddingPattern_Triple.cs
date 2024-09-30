using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingPattern_Triple : PuddingPattern_Base
{
    public override Queue<PuddingData> pattern(int stairs)
    {
        Queue<PuddingData> puddingQueue = new Queue<PuddingData>();

        float modifySpeed = Random.Range(-ModifySpeed, ModifySpeed);
        float totalSpeed = _defaultSpeed + modifySpeed;
        float speed = totalSpeed < 9.5f ? 9.5f : totalSpeed > 18f ? 18f : totalSpeed;

        puddingQueue.Enqueue(new PuddingData()
        {
            type = PuddingType.Normal,
            isRightMove = Random.Range(0, 2) == 0,
            stairs = ++stairs,
            moveSpeed = speed,
            waitTime = _defaultDelay - Random.Range(0.1f, 0.2f)
        });

        puddingQueue.Enqueue(new PuddingData()
        {
            type = PuddingType.Normal,
            isRightMove = Random.Range(0, 2) == 0,
            stairs = ++stairs,
            moveSpeed = speed,
            waitTime = _defaultDelay - Random.Range(0.1f, 0.2f)
        });

        puddingQueue.Enqueue(new PuddingData()
        {
            type = PuddingType.Normal,
            isRightMove = Random.Range(0, 2) == 0,
            stairs = ++stairs,
            moveSpeed = speed,
            waitTime = _defaultDelay
        });

        base.IncreaseChance(3);

        return puddingQueue;
    }
}
