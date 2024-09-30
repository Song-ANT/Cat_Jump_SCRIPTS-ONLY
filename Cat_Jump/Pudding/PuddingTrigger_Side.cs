using Scripts.Framework.Events.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingTrigger_Side : MonoBehaviour
{
    private PuddingController _controller;

    private void Start()
    {
        _controller = transform.GetComponentInParent<PuddingController>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)Define.LayerName.Cat)
        {
            if(!_controller.IsSideTriggerHandler)
            {
                _controller.OnTriggerEvent(false, gameObject.CompareTag("Side_R"));
                _controller.IsSideTriggerHandler = true;
            } 
        }
        
    }
}
