using Scripts.Framework.Events.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartBtn : MonoBehaviour, IPointerClickHandler
{
    [Header("Broadcaster")]
    [SerializeField] private GameEventSO StartBtnEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        StartBtnEvent.RaiseEvent();
    }

}
