using Scripts.Framework.Events.SO;
using Scripts.Framework.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tutorial_UI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameEventSO StartBtnEvent;
    [SerializeField] private GameEventSO TouchCatJumpEvent;
    [SerializeField] private Image Hand;

    private bool _init;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_init) return;
        _init = true;

        Data_Manager.Instance.SetIsFirstStart(true);
        
        TouchCatJumpEvent.RaiseEvent();
        Hand.enabled = false;
        
        Util.Delay(1.5f, () =>
        {
            StartBtnEvent.RaiseEvent();
            Destroy(gameObject);
        });
        
    }
}
