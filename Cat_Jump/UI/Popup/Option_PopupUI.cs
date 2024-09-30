using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Modules.UIModules.UI_Elements_Controller;
using Scripts.Framework.Modules.UIModules.UI_Panel;
using Scripts.Framework.Modules.UIModules.UIComponent;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Option_PopupUI : Popup_UI
{

    [Header("Button")]
    [SerializeField] private Toggle SFX_Toggle;
    [SerializeField] private Toggle Haptic_Toggle;
    
    [SerializeField] private Button Message_Btn;
    [SerializeField] private Button Terms_Btn;
    [SerializeField] private Button Personal_Btn;
    [SerializeField] private Button Exit_Btn;


    [Header("Button Callback")]
    [SerializeField] private BtnCallBack ExitBtn_Callback;


    [SerializeField] private Mail_Btn_SubUI mail;

    private void OnEnable()
    {
        GetComponent<Canvas>().sortingLayerName = Define.SortingLayerName.MainMenuUI.ToString();

        SFX_Toggle.onValueChanged.AddListener(OnSFXToggleChanged);
        Haptic_Toggle.onValueChanged.AddListener(OnHapticToggleChanged);

        Message_Btn.onClick.AddListener(OnMessageButtonClicked);
        Terms_Btn.onClick.AddListener(OnTermsBtnClicked);
        Personal_Btn.onClick.AddListener(OnPersonalBtnClicked);
        Exit_Btn.onClick.AddListener(OnExitBtnClicked);


        OnSFXToggleChanged(Data_Manager.Instance.IsMute_SFX);
        OnHapticToggleChanged(Data_Manager.Instance.ISMute_Haptic);
    }

    private void OnDisable()
    {
        SFX_Toggle.onValueChanged.RemoveAllListeners();
        Haptic_Toggle.onValueChanged.RemoveAllListeners();

        Message_Btn.onClick.RemoveAllListeners();
        Terms_Btn.onClick.RemoveAllListeners();
        Personal_Btn.onClick.RemoveAllListeners();
        Exit_Btn.onClick.RemoveAllListeners();

    }



    #region ToggleEvent

    // SFX 토글 On/Off
    private void OnSFXToggleChanged(bool isOn)
    {
        SFX_Toggle.isOn = isOn;


        //Audio_Manager.Instance.SetMute_SFX(isOn);
        Data_Manager.Instance.SetMuteSFX(isOn);
        if(isOn != false) Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
    }

    // Haptic 토글 On/Off
    private void OnHapticToggleChanged(bool isOn)
    {
        Haptic_Toggle.isOn = isOn;

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
        Data_Manager.Instance.SetMuteHaptic(isOn);
        if (isOn == true) Device_Manager.Instance.OnVibe(100);
    }


    #endregion




    #region Button Event
    // Messege 토글 On/Off
    private void OnMessageButtonClicked()
    {
        string feedback = "feedback";
        //mail.Contact(feedback);

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
    }

    private void OnTermsBtnClicked()
    {
        Application.OpenURL(Define.TERMS_SERVICE_URL);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
    }

    private void OnPersonalBtnClicked()
    {
        Application.OpenURL(Define.PRIVATE_POLICY_URL);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
    }

    private void OnExitBtnClicked()
    {
        ExitBtn_Callback.eventSO.RaiseEvent(ExitBtn_Callback);
    }
    #endregion



}
