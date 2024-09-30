using Scripts.Framework.Events.SO;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Modules.UIModules.UI_Elements_Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Collect_Popup_UI : MonoBehaviour
{


    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private List<Toggle> _toggle;
    [SerializeField] private List<GameObject> _toggleInven;

    [SerializeField] private BtnCallBack _btnCallback;
    [SerializeField] private Button _homeBtn;

    [SerializeField] private GameEventSO<int> CameraAnimationEvent;


    private void OnEnable()
    {
        GetComponent<Canvas>().sortingLayerName = Define.SortingLayerName.MainMenuUI.ToString();
        ToggleChanged_AddListener();
        _homeBtn.onClick.AddListener(OnHomeBtnClicked);
        CameraAnimationEvent.RaiseEvent((int)Define.CameraAnimation.CollectCamera);

        ToggleChanged(true, 0);
    }

    private void OnDisable()
    {
        ToggleChanged_RemoveListener();
        _homeBtn.onClick.RemoveAllListeners();
    }


    private void ToggleChanged_AddListener()
    {
        for (int i = 0; i < _toggle.Count; i++)
        {
            int index = i;
            _toggle[i].onValueChanged.AddListener((isOn) => ToggleChanged(isOn, index));
        }
    }

    private void ToggleChanged_RemoveListener()
    {
        for (int i=0; i< _toggle.Count; i++)
        {
            _toggle[i].onValueChanged.RemoveAllListeners();
        }
    }

    private void ToggleChanged(bool isOn, int index)
    {
        Toggle toggle = _toggle[index];
        RectTransform rt = toggle.GetComponent<RectTransform>();
        Vector2 size = rt.sizeDelta;
        

        if (isOn)
        {
            size.y = 250;
            _toggleInven[index].gameObject.SetActive(true);
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
        }
        else
        {
            size.y = 200;
            _toggleInven[index].gameObject.SetActive(false);
        }

        rt.sizeDelta = size;
    }




    private void OnHomeBtnClicked()
    {
        CameraAnimationEvent.RaiseEvent((int)Define.CameraAnimation.CatJumpSceneCamera);
        _btnCallback.eventSO.RaiseEvent(_btnCallback);
    }
}
