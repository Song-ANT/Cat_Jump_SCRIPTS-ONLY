using DG.Tweening;
using Scripts.Framework.Events.Interfaces;
using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Events.SO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraLookObject : MonoBehaviour
{
    [Header("Listener")]
    [SerializeField] private GameEventListener<PuddingData> EnterTopEvent;
    //[SerializeField] private GameEventSO<PuddingData> EnterTopSO;

    private GameObject _cat;
    private bool _isNormal = true;

    private void Start()
    {
        _cat = GameObject.FindWithTag("Cat");
        _isNormal = true;
        //_cat = GameObject.FindWithTag("Cat");
        EnterTopEvent.Subscribe();
        EnterTopEvent.SubstitutionEvent(OnEnterTopEventStarted);
    }

    private void OnDestroy()
    {
        EnterTopEvent.Unsubscribe();
    }

    private void Update()
    {
        if(!_isNormal)
        transform.position = _cat.transform.position;
    }

    public void OnEnterTopEventStarted(PuddingData data)
    {

        switch (data.type)
        {
            case PuddingType.Feather:
                _isNormal = false;
                break;
            default:
                _isNormal = true;
                transform.position = _cat.transform.position;
                break;
        }
    }
}
