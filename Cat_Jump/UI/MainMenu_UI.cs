using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Modules.UIModules.UI_Elements_Controller;
using Scripts.Framework.Modules.UIModules.UIComponent;
using Scripts.Framework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class MainMenu_UI : Scene_UI
{
    [Header("Broadcater")]
    [SerializeField] private GameEventSO StartBtnEvent;
    [SerializeField] private GameEventSO<int> CatSetSkinEventTest;

    [Header("Listener")]
    [SerializeField] private BtnSO UiOnOffBtn;
    [SerializeField] private GameEventListener RefreshGoldEvent;

    [Header("Button")]
    [SerializeField] private Button Start_Btn;
    [SerializeField] private Button Upgrade_Btn;
    [SerializeField] private Button Collect_Btn;
    [SerializeField] private Button Reward_Feather_Btn;
    [SerializeField] private Button Reward_Shield_Btn;
    [SerializeField] private Button Option_Btn;

    [SerializeField] private Button TestUISetFalse;


    [Header("ButtonCallback")]
    [SerializeField] private BtnCallBack Upgrade_Callback;
    [SerializeField] private BtnCallBack Collect_Callback;
    [SerializeField] private BtnCallBack Option_Callback;


    [Header("Reward")]
    [SerializeField] private Reward_Btn_SubUI Reward_Feather;
    [SerializeField] private Reward_Btn_SubUI Reward_Shield;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI Gold;
    [SerializeField] private GameObject GoldObject;


    private bool _isUIOpend;
    private int _uiClicked;

    private void OnEnable()
    {
        GetComponentInParent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerName = Define.SortingLayerName.MainMenuUI.ToString();

        Start_Btn.onClick.AddListener(OnStartBtnClicked);
        Upgrade_Btn.onClick.AddListener(OnShopBtnClicked);
        Collect_Btn.onClick.AddListener(CollectBtnClicked);
        Reward_Feather_Btn.onClick.AddListener(OnRewardFeatherBtnClicked);
        Reward_Shield_Btn.onClick.AddListener(OnRewardShieldBtnClicked);
        Option_Btn.onClick.AddListener(OnOptionBtnClicked);


        UiOnOffBtn.OnBtnEventTriggered.AddListener(UIOnOffClicked);

        Reward_Feather.SetOff(() => { Data_Manager.Instance.SetRewardFeather(false); });
        Reward_Shield.SetOff(() => { Data_Manager.Instance.SetRewardShield(false); });

        RefreshGoldEvent.Subscribe();
        RefreshGoldEvent.SubstitutionEvent(OnRefreshGoldEventStarted);

        OnRefreshGoldEventStarted();

        TestUISetFalse.onClick.AddListener(TestUISetFalseStarted);
    }

    private void OnDisable()
    {
        Start_Btn.onClick.RemoveAllListeners();
        Upgrade_Btn.onClick.RemoveAllListeners();
        Reward_Feather_Btn.onClick.RemoveAllListeners();
        Reward_Shield_Btn.onClick.RemoveAllListeners();
        Option_Btn.onClick.RemoveAllListeners();


        UiOnOffBtn.OnBtnEventTriggered.RemoveListener(UIOnOffClicked);

        RefreshGoldEvent.Unsubscribe();

        TestUISetFalse.onClick.RemoveAllListeners();
    }


    private void TestUISetFalseStarted()
    {
        if (_uiClicked > 10)
        {
            gameObject.SetActive(false);
        }
        _uiClicked += 1;
    }

    private void SetActiveTrue()
    {
        Start_Btn.gameObject.SetActive(true);
        Upgrade_Btn.gameObject.SetActive(true);
        Collect_Btn.gameObject.SetActive(true);
        Reward_Feather_Btn.gameObject.SetActive(true);
        Reward_Shield_Btn.gameObject.SetActive(true);
        Option_Btn.gameObject.SetActive(true);
        GoldObject.gameObject.SetActive(true);
    }

    private void SetActiveFalse()
    {
        Start_Btn.gameObject.SetActive(false);
        Upgrade_Btn.gameObject.SetActive(false);
        Collect_Btn.gameObject.SetActive(false);
        Reward_Feather_Btn.gameObject.SetActive(false);
        Reward_Shield_Btn.gameObject.SetActive(false);
        Option_Btn.gameObject.SetActive(false);
        GoldObject.gameObject.SetActive(false);
    }


    #region Button Event
    private void OnStartBtnClicked()
    {
        if (_isUIOpend) return;
        StartBtnEvent.RaiseEvent();
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_Button_Start, 1, false);
    }

    private void OnShopBtnClicked()
    {
        if (_isUIOpend) return;
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
        Upgrade_Callback.eventSO.RaiseEvent(Upgrade_Callback);
    }

    private void CollectBtnClicked()
    {
        if (_isUIOpend) return;
        //SetActiveFalse();
        //Collect_Callback.eventSO.RaiseEvent(Collect_Callback);

        CatSetSkinEventTest.RaiseEvent((int)Util.GetRandomEnumValue<CatSkinName>());
    }

    private void OnRewardFeatherBtnClicked()
    {
        if (_isUIOpend || Data_Manager.Instance.RewardFeather) return;

        Reward_Feather.SetOn(() => { Data_Manager.Instance.SetRewardFeather(true); });

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
        Debugger.Log("¸®¿öµå±¤°í ³ª¿À¼À - ±êÅÐ");
    }

    private void OnRewardShieldBtnClicked()
    {
        if (_isUIOpend) return;

        Reward_Shield.SetOn(() => { Data_Manager.Instance.SetRewardShield(true); });

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
        Debugger.Log("¸®¿öµå±¤°í ³ª¿À¼À - ½¯µå");
    }

    private void OnOptionBtnClicked()
    {
        if (_isUIOpend) return;
        Option_Callback.eventSO.RaiseEvent(Option_Callback);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_Button_Option, 1, false);
    }

    #endregion






    #region Event
    private void UIOnOffClicked(BtnCallBack callback)
    {
        switch (callback.btnCallbackType)
        {
            case BtnCallBack.BtnCallbackType.Open:
                _isUIOpend = true; break;
            case BtnCallBack.BtnCallbackType.Close:
                SetActiveTrue();
                _isUIOpend = false; break;
        }

    }


    private void OnRefreshGoldEventStarted()
    {
        Gold.text = Util.FormatMoney(Data_Manager.Instance.Gold);
    }
    #endregion
}
