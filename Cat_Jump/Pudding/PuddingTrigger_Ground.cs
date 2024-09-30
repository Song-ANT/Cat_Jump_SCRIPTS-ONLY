using Scripts.Framework.Events.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingTrigger_Ground : MonoBehaviour
{
    private PuddingController _controller;

    private void Awake()
    {
        _controller = transform.GetComponentInParent<PuddingController>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isCliff = false;
        if (collision.gameObject.layer == (int)Define.LayerName.Cat)
        {
            if (Mathf.Abs(collision.transform.position.x - transform.position.x) > 1.5f) isCliff = true;
            _controller.OnTriggerEvent(true, false, isCliff);
        }
    }
}
