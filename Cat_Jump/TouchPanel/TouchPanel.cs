using Cysharp.Threading.Tasks;
using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Modules.SingletonModule.CentralSingleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour, IPointerDownHandler
{
    [Header("BroadCasting")]
    [SerializeField] private GameEventSO TouchCatJumpEvent;


    public void OnPointerDown(PointerEventData eventData)
    {
        TouchCatJumpEvent.RaiseEvent();
    }
}
