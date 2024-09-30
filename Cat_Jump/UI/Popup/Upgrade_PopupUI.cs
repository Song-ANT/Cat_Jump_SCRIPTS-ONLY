using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Modules.UIModules.UI_Elements_Controller;
using Scripts.Framework.Modules.UIModules.UI_Panel;
using Scripts.Framework.Modules.UIModules.UIComponent;
using Scripts.Framework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade_PopupUI : Popup_UI
{

    [Header("Button")]
    [SerializeField] private Button ExitBtn;
    [SerializeField] private Button UpgradeComboBtn;
    [SerializeField] private Button UpgradeFeatherBtn;
    [SerializeField] private Button UpgradeShieldBtn;


    [Header ("Combo")]
    [SerializeField] private TextMeshProUGUI Upgrade_Combo_Level_Text;
    [SerializeField] private TextMeshProUGUI Upgrade_Combo_Descript_Text;
    [SerializeField] private TextMeshProUGUI Upgrade_Combo_Gold_Text;

    [Header("Feather")]
    [SerializeField] private TextMeshProUGUI Upgrade_Feather_Level_Text;
    [SerializeField] private TextMeshProUGUI Upgrade_Feather_Descript_Text;
    [SerializeField] private TextMeshProUGUI Upgrade_Feather_Gold_Text;

    [Header("Shield")]
    [SerializeField] private TextMeshProUGUI Upgrade_Shield_Level_Text;
    [SerializeField] private TextMeshProUGUI Upgrade_Shield_Descript_Text;
    [SerializeField] private TextMeshProUGUI Upgrade_Shield_Gold_Text;

    [Header("ButtonCallback")]
    [SerializeField] private BtnCallBack Exit_Callback;


    private int _comboPrice = 0;
    private int _featherPrice = 0;
    private int _shieldPrice = 0;


    private void OnEnable()
    {
        GetComponent<Canvas>().sortingLayerID = 6;

        ExitBtn.onClick.AddListener(OnExitBtnClicked);
        UpgradeComboBtn.onClick.AddListener(OnUpgradeComboBtnClicked);
        UpgradeFeatherBtn.onClick.AddListener(OnUpgradeFeatherBtnClicked);
        UpgradeShieldBtn.onClick.AddListener(OnUpgradeShieldBtnClicked);

        Set_Upgrade_Combo();
        Set_Upgrade_Feather();
        Set_Upgrade_Shield();
    }

    private void OnDisable()
    {
        ExitBtn.onClick.RemoveAllListeners();
        UpgradeComboBtn.onClick.RemoveAllListeners();
        UpgradeFeatherBtn.onClick.RemoveAllListeners();
        UpgradeShieldBtn.onClick.RemoveAllListeners();
    }

    private void OnExitBtnClicked()
    {
        Exit_Callback.eventSO.RaiseEvent(Exit_Callback);
    }


    #region Upgrade_Combo

    private void Set_Upgrade_Combo()
    {
        int level = Data_Manager.Instance.Upgrade_Combo_Level;
        Upgrade_Combo_Level_Text.text = level == Define.Max_Upgrade_Combo_Level ? "Max" : $"Lv. {level}";

        if (level == 0) Upgrade_Combo_Descript_Text.gameObject.SetActive(false);
        else
        {
            Upgrade_Combo_Descript_Text.gameObject.SetActive(true);
            Upgrade_Combo_Descript_Text.text = $"+{level}";
        }

        _comboPrice = Get_Upgrade_Combo_Price(level);
        Upgrade_Combo_Gold_Text.text = _comboPrice == -1 ? "Max" : Util.FormatMoney(_comboPrice);
    }

    private int Get_Upgrade_Combo_Price(int level)
    {
        switch (level)
        {
            case 0: return 1000;
            case 1: return 3000;
            case 2: return 5000;
            case 3: return 7500;
            case 4: return 10000;
            case 5: return 14000;
            case 6: return 20000;
            default: return -1;
        }
    }


    private void OnUpgradeComboBtnClicked()
    {
        if (_comboPrice == -1 || !Data_Manager.Instance.AddGold(-_comboPrice))
        {
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
            return;
        }

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_Button_Purchase, 1, false);
        Data_Manager.Instance.Upgrade_Combo();
        Set_Upgrade_Combo();
    }

    #endregion


    #region Upgrade_Feather;

    private void Set_Upgrade_Feather()
    {
        int level = Data_Manager.Instance.Upgrade_Feather_Level;
        Upgrade_Feather_Level_Text.text = level == Define.Max_Upgrade_Feather_Level ? "Max" : $"Lv. {level}";

        if (level == 0) Upgrade_Feather_Descript_Text.gameObject.SetActive(false);
        else
        {
            Upgrade_Feather_Descript_Text.gameObject.SetActive(true);
            Upgrade_Feather_Descript_Text.text = $"+{level}";
        }

        _featherPrice = Get_Upgrade_Feather_Price(level);
        Upgrade_Feather_Gold_Text.text = _featherPrice == -1 ? "Max" : Util.FormatMoney(_featherPrice);
    }

    private int Get_Upgrade_Feather_Price(int level)
    {
        switch (level)
        {
            case 0: return 150;
            case 1: return 300;
            case 2: return 700;
            case 3: return 1200;
            case 4: return 1800;
            case 5: return 2500;
            case 6: return 3300;
            case 7: return 4200;
            case 8: return 5200;
            case 9: return 6300;
            default: return -1;
        }
    }


    private void OnUpgradeFeatherBtnClicked()
    {
        if (_featherPrice == -1 || !Data_Manager.Instance.AddGold(-_featherPrice))
        {
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
            return;
        }

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_Button_Purchase, 1, false);
        Data_Manager.Instance.Upgrade_Feather();
        Set_Upgrade_Feather();
    }

    #endregion


    #region Upgrade_Shield;

    private void Set_Upgrade_Shield()
    {
        int level = Data_Manager.Instance.Upgrade_Shield_Level;
        Upgrade_Shield_Level_Text.text = level == Define.Max_Upgrade_Shield_Level ? "Max" : $"Lv. {level}";

        if (level == 0) Upgrade_Shield_Descript_Text.gameObject.SetActive(false);
        else
        {
            Upgrade_Shield_Descript_Text.gameObject.SetActive(true);
            Upgrade_Shield_Descript_Text.text = $"+{level}";
        }

        _shieldPrice = Get_Upgrade_Shield_Price(level);
        Upgrade_Shield_Gold_Text.text = _shieldPrice == -1 ? "Max" : Util.FormatMoney(_shieldPrice);
    }

    private int Get_Upgrade_Shield_Price(int level)
    {
        switch (level)
        {
            case 0: return 5000;
            case 1: return 25000;
            default: return -1;
        }
    }


    private void OnUpgradeShieldBtnClicked()
    {
        if (_shieldPrice == -1 || !Data_Manager.Instance.AddGold(-_shieldPrice))
        {
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_button_Others, 1, false);
            return;
        }

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.UI_Button_Purchase, 1, false);
        Data_Manager.Instance.Upgrade_Shield();
        Set_Upgrade_Shield();
    }

    #endregion
}
