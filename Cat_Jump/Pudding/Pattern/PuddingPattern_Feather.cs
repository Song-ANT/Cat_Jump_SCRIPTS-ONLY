using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingPattern_Feather : PuddingPattern_Base
{
    public override Queue<PuddingData> pattern(int stairs)
    {
        Queue<PuddingData> puddingQueue = new Queue<PuddingData>();

        int level = Data_Manager.Instance.Upgrade_Feather_Level;

        for (int i = 0; i < level + 8; i++)
        {
            float speed = _defaultSpeed + 80 + level * 20;
            float waitTime = 0.05f - (level * 0.003f);
            puddingQueue.Enqueue(new PuddingData()
            {
                type = PuddingType.Normal,
                isRightMove = Random.Range(0, 2) == 0,
                stairs = ++stairs,
                moveSpeed = speed,
                waitTime = waitTime
            });

        }
        base.IncreaseChance(1);


        return puddingQueue;
    }
}
