using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skin_Inven_SubUI : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Scrollbar _scroll;
    [SerializeField] private Transform _content;

    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;

    private int _maxPage;
    private int _curPage = 0;
    private float _distance;

    private float[] pos;

    private void OnEnable()
    {
        _maxPage = _content.childCount - 1;
        _curPage = 0;
        _distance = 1f / _maxPage;

        pos = new float[_maxPage+1];
        for (int i = 0; i <= _maxPage; i++) pos[i] = _distance * i;

        _leftBtn.onClick.AddListener(OnLeftBtnClicked);
        _rightBtn.onClick.AddListener(OnRightBtnClicked);

        StartCoroutine(InitializeScrollPosition());
    }

    private IEnumerator InitializeScrollPosition()
    {
        yield return new WaitForEndOfFrame();
        _scrollRect.horizontalNormalizedPosition = 0f;
    }

    private void OnDisable()
    {
        _leftBtn.onClick.RemoveAllListeners();
        _rightBtn.onClick.RemoveAllListeners();
    }


    #region Event
    private void OnLeftBtnClicked()
    {
        _curPage = _curPage - 1 <= 0 ? 0 : _curPage - 1;
        _scrollRect.DOHorizontalNormalizedPos(pos[_curPage], 0.5f);

    }

    private void OnRightBtnClicked()
    {
        _curPage = _curPage + 1 >= _maxPage ? _maxPage : _curPage + 1;
        _scrollRect.DOHorizontalNormalizedPos(pos[_curPage], 0.5f);

    }



    public void OnEndDrag(PointerEventData eventData)
    {
        for (int i = 0; i <= _maxPage; i++)
        {
            if (_scroll.value < pos[i] + _distance * 0.5 && _scroll.value > pos[i] - _distance * 0.5f)
            {
                _scrollRect.DOHorizontalNormalizedPos(pos[i], 0.5f);
                _curPage = i;
                break;
            }
        }

        if(eventData.delta.x > 18 && _curPage -1 >= 0)
        {
            _curPage -= 1;
            _scrollRect.DOHorizontalNormalizedPos(pos[_curPage], 0.5f);
        }
        else if(eventData.delta.x < -18 && _curPage + 1 <= _maxPage)
        {
            _curPage += 1;
            _scrollRect.DOHorizontalNormalizedPos(pos[_curPage], 0.5f);

        }
    }
    #endregion
}
