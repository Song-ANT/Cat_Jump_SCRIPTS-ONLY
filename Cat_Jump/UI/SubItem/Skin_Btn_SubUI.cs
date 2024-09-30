using JetBrains.Annotations;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Define;






public class Skin_Btn_SubUI : MonoBehaviour
{

    #region Field

    [SerializeField] private Button _btn;
    [SerializeField] private SetSkin_Base _skin;

    [SerializeField] private GameEventSO<int> _eventSO;



    public SetSkin_Base Skin { get { return _skin; } set { _skin = value; } }

    #endregion


    #region Enable & Disable

    private void OnEnable()
    {
        _btn.onClick.AddListener(OnBtnClicked);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveAllListeners();
    }

    #endregion



    #region Event

    private void OnBtnClicked()
    {
        SetSkinEvent(_skin);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_Button_Skin, 1, false);
    }

    #endregion



    private void SetSkinEvent(SetSkin_Base skin)
    {
        switch (skin.skinType)
        {
            case SkinTypeEnum.Cat:
                int skinNum = skin.isRandom ? (int)Util.GetRandomEnumValue<CatSkinName>() : (int)skin.catSkinName;
                _eventSO?.RaiseEvent(skinNum);
                break;
            case SkinTypeEnum.Pudding:
                break;
            case SkinTypeEnum.BG:
                break;

            default:
                break;
        }
    }
}

