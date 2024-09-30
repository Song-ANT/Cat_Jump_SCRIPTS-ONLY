using Cysharp.Threading.Tasks;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Modules.UIModules.UI_Elements_Controller;
using Scripts.Framework.Modules.UIModules.UIComponent;
using Scripts.Framework.Scenes.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver_PopupUI : Popup_UI
{
    #region Fields

    [Header("Broadcasting on")]
    [SerializeField] private GameEventSO RefreshGoldEvent;
    [SerializeField] private GameEventSO<bool> ReviveEvent;

    [Header("Button")]
    [SerializeField] private Button GameOverBtn;
    [SerializeField] private Button ReviveBtn;
    [SerializeField] private BtnCallBack GameOver_Callback;

    [Header("You need Scenes")]
    [SerializeField] private GameSceneSO GameScene;

    [SerializeField] private Image FrontHeart;
    [SerializeField] private Image ReviveBtnAdImage;


    #endregion



    private void OnEnable()
    {
        Time.timeScale = 1f;
        GetComponent<Canvas>().sortingLayerName = Define.SortingLayerName.InGame.ToString();
        StartCoroutine(RevieveHeartDecrease(5f));

        GameOverBtn.onClick.AddListener(OnGameOverBtnClicked);
        ReviveBtn.onClick.AddListener(OnReviveBtnClicked);
    }

    private void OnDisable()
    {
        GameOverBtn.onClick.RemoveAllListeners();
        ReviveBtn.onClick.RemoveAllListeners();
        Time.timeScale = 1f;
    }

    private void OnGameOverBtnClicked()
    {
        ReviveEvent.RaiseEvent(false);
        GameOver_Callback.eventSO.RaiseEvent(GameOver_Callback);
    }

    private void OnReviveBtnClicked()
    {
        SDKIntegrationSystem.Instance.ShowReward(new AdsCommand(() => { ReviveReward(); }, CommandOrderer.Reward), FailAciton).Forget();
        GameOver_Callback.eventSO.RaiseEvent(GameOver_Callback);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
    }

    private void ReviveReward()
    {
        ReviveEvent.RaiseEvent(true);
    }

    private void FailAciton()
    {
        Debugger.Log("fail");
        OnGameOverBtnClicked();
    }


    private IEnumerator RevieveHeartDecrease(float duration)
    {
        float startFillAmount = FrontHeart.fillAmount;
        float endFillAmount = 0f;
        float deltaTime = 0f;

        while (deltaTime < duration)
        {
            float t = deltaTime / duration;

            FrontHeart.fillAmount = Mathf.Lerp(startFillAmount, endFillAmount, t);

            deltaTime += Time.deltaTime;

            yield return null;
        }

        ReviveBtnAdImage.color = new Color(200/255f, 200/255f, 200/255f);
        ReviveBtn.interactable = false;

        OnGameOverBtnClicked();
    }
}
