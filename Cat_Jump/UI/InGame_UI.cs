using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Modules.UIModules.UI_Elements_Controller;
using Scripts.Framework.Modules.UIModules.UIComponent;
using Scripts.Framework.Scenes.SO;
using Scripts.Framework.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGame_UI : Scene_UI
{

    #region Field

    [Header("Listener")]
    [SerializeField] private BtnSO UiOnOffBtn;
    [SerializeField] private RequestSequenceEventChannelSO SequenceEventChannel;
    [SerializeField] private GameEventListener<bool> GameoverEvent;
    [SerializeField] private GameEventListener<int> SetComboTextEvent;
    [SerializeField] private GameEventListener<int> SetPerfectComboTextEvent;
    [SerializeField] private GameEventListener<bool> ReviveEvent;


    [Header("ButtonCallback")]
    [SerializeField] private BtnCallBack GameOver_Callback;

    [Header("You need Scenes")]
    [SerializeField] private GameSceneSO GameScene;

    [SerializeField] private GameObject TouchCanvas;
    [SerializeField] private TextMeshProUGUI Combo_Text;
    [SerializeField] private TextMeshProUGUI BestScore_Text;
    [SerializeField] private GameObject PerfectCombo_Object;
    [SerializeField] private TextMeshProUGUI PerfectCombo_Text;
    [SerializeField] private GameObject ReviveCount;
    [SerializeField] private TextMeshProUGUI ReviveCount_Text;


    public Button testUI;
    private bool _isUIOpend;

    private int _bestScore;

    #endregion



    #region Enable & Disable

    private void OnEnable()
    {
        GetComponentInParent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().sortingLayerName = Define.SortingLayerName.InGame.ToString();

        TouchCanvas.SetActive(true);

        UiOnOffBtn.OnBtnEventTriggered.AddListener(UIOnOffClicked);

        GameoverEvent.Subscribe();
        GameoverEvent.SubstitutionEvent(OnGameOverEventStarted);

        SetComboTextEvent.Subscribe();
        SetComboTextEvent.SubstitutionEvent(OnSetComboEventStarted);

        SetPerfectComboTextEvent.Subscribe();
        SetPerfectComboTextEvent.SubstitutionEvent(OnSetPerfectComboEventStarted);

        ReviveEvent.Subscribe();
        ReviveEvent.SubstitutionEvent(OnReviveEventStarted);

        testUI.onClick.AddListener(OnClickTestUIStarted);

        _bestScore = Data_Manager.Instance.BestScore;
        BestScore_Text.text = _bestScore.ToString();
    }


    private void OnDisable()
    {
        UiOnOffBtn.OnBtnEventTriggered.RemoveListener(UIOnOffClicked);

        GameoverEvent.Unsubscribe();
        SetComboTextEvent.Unsubscribe();
        SetPerfectComboTextEvent.Unsubscribe();
        ReviveEvent.Unsubscribe();

        testUI.onClick.RemoveAllListeners();
    }

    #endregion

    private void OnClickTestUIStarted()
    {
        gameObject.SetActive(false);
    }

    #region Function

    public async void ReStartScene()
    {
        await SequenceEventChannel.RaiseEvent(GameScene, false);
    }

    #endregion



    #region Event

    private void OnGameOverEventStarted(bool isRevive)
    {
        TouchCanvas.SetActive(false);
        if (!isRevive) GameOver_Callback.eventSO.RaiseEvent(GameOver_Callback);
        else
        {
            int rewardGole = Data_Manager.Instance.InGame_Score / 10 > 250 ? 250 : Data_Manager.Instance.InGame_Score / 10;
            Data_Manager.Instance.AddGold(rewardGole);

            UniTask.Delay(TimeSpan.FromSeconds(3f)).ContinueWith(() =>
            {
                if (Data_Manager.Instance.FirstUpgrade || Data_Manager.Instance.StartGameNum >= 5)
                {
                    SDKIntegrationSystem.Instance.ShowInterstitial(InterstitialKey.Resume, new AdsCommand(() => { ReStartScene(); }, CommandOrderer.Interstitial)).Forget();
                    if(Data_Manager.Instance.StartGameNum >= 5) Data_Manager.Instance.SetStartGameNum(0);
                }
                else ReStartScene();
            });
        }
    }

    private void OnSetComboEventStarted(int combo)
    {
        Combo_Text.text = combo.ToString();
        
        if(combo > _bestScore)
        {
            _bestScore = combo;
            BestScore_Text.text = _bestScore.ToString();
        }
    }

    private void OnSetPerfectComboEventStarted(int perfectCombo)
    {
        PerfectCombo_Object.SetActive(true);
        PerfectCombo_Text.text = "+" + perfectCombo.ToString();

        PerfectCombo_Object.transform.localScale = Vector3.one;


        PerfectCombo_Object.transform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            PerfectCombo_Object.SetActive(false);
        });
    }

    private void OnReviveEventStarted(bool isReviveBtnClicked)
    {
        if (!isReviveBtnClicked) return;

        ReviveCount.SetActive(true);
        StartCoroutine(ReviveCountDown(3f));
    }


    private void UIOnOffClicked(BtnCallBack callback)
    {
        switch (callback.btnCallbackType)
        {
            case BtnCallBack.BtnCallbackType.Open:
                _isUIOpend = true; break;
            case BtnCallBack.BtnCallbackType.Close:
                _isUIOpend = false; break;
        }

    }

    #endregion





    #region Coroutine
    private IEnumerator ReviveCountDown(float duration)
    {
        float deltaTime = duration + 1;

        while (deltaTime > 1)
        {
            ReviveCount_Text.text = ((int)deltaTime).ToString();

            deltaTime -= Time.deltaTime;

            yield return null;
        }

        ReviveCount.SetActive(false);
        TouchCanvas.SetActive(true);

    }
    #endregion
}
