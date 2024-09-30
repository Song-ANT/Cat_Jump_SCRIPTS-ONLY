using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingPattern_Normal : PuddingPattern_Base
{

    public override Queue<PuddingData> pattern(int stairs)
    {
        Queue<PuddingData> puddingQueue = new Queue<PuddingData>();

        PuddingData data = new PuddingData();
        data.type = GetType();
        data.isRightMove = Random.Range(0, 2) == 0;
        data.stairs = stairs + 1;

        float speed = _defaultSpeed + Random.Range(-ModifySpeed, ModifySpeed);
        data.moveSpeed = (speed < 9.5f) ? 9.5f : (speed > 18) ?  18f : speed; 


        puddingQueue.Enqueue(data);

        return puddingQueue;
    }

    private new PuddingType GetType()
    {
        int rand = Random.Range(0, 1000);



        if (_skinChance == 0 && ( _skinChance > 100 && rand == 0))
        {
            _skinChance = 1;
            return PuddingType.Skin;
        }
        if (_shieldChance > 200 || (_shieldChance > 15 && rand > 0 && rand <= 5))
        {
            _shieldChance = 0;

            return PuddingType.Shield;
        }
        if (_featherChance > 50 || ( _featherChance > 5 && rand > 5 && rand <= 75))
        {
            _featherChance = 0;
            return PuddingType.Feather;
        }


        base.IncreaseChance(1);
        return PuddingType.Normal;
    }
}
