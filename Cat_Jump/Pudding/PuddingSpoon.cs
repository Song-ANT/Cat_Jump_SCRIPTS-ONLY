using DG.Tweening;
using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Managers.ObjectPool.Core;
using Scripts.Framework.Managers.ObjectPool.Modular;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingSpoon : PoolBehavior
{
    [SerializeField] private GameObject PuddingSpoon_L;
    [SerializeField] private GameObject PuddingSpoon_R;

    

    private Transform _pudding;
    private PuddingData _data;
    private PuddingController _controller;

    private bool _isArrive;
    private bool _isRightMove;

    public void Initialize(Transform pudding, PuddingData data, Vector3 dir)
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = data.stairs-1;
        _pudding = pudding;
        _data = data;
        _controller = pudding.GetComponent<PuddingController>();
        transform.position = dir;

        if (data.isRightMove) PuddingSpoon_L.SetActive(true);
        else PuddingSpoon_R.SetActive(true);

    }


    void Update()
    {
        if (_pudding == null) Clear();
        if (_isArrive) return;

        if (_pudding != null) transform.position = _pudding.position;

        if (_controller.IsArrive) ArriveMovement();
    }

    private void ArriveMovement()
    {
        _isArrive = true;

        Vector2 targetPosition = _data.isRightMove ? new Vector2(-15, transform.position.y) : new Vector2(15, transform.position.y);
        transform.DOMove(targetPosition, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Clear();
        });
    }

    private void Clear()
    {
        _isArrive = false;
        PuddingSpoon_L?.SetActive(false);
        PuddingSpoon_R?.SetActive(false);
        PoolManager.OnRelease(gameObject);
    }
}
