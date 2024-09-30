using Firebase.Analytics;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Managers.Asset.Core;
using Scripts.Framework.Modules.DeviceModules;
using Scripts.Framework.Modules.SecurityPlayerPrefs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class Data_Manager : IndividualSingletonPersist<Data_Manager>
{
    #region Field

    private bool _isInitialized = false;

    private GameEventSO RefreshGoldEvent;

    private int _gold = 0;

    private int _upgrade_Combo_Level = 0;
    private int _upgrade_Feather_Level = 0;
    private int _upgrade_Shield_Level = 0;

    private int _skin_Cat = 0;

    private bool _isMute_BGM;
    private bool _isMute_SFX;
    private bool _isMute_Haptic;

    private bool _isFirstStart;

    private int _bestScore;

    private int _startGameNum;

    private bool _firstUpgrade;


    private Dictionary<Define.CatSkinName, int> _skin_Cat_Dic = new Dictionary<Define.CatSkinName, int>()
    {
        {Define.CatSkinName.Cat00, 1 },
        {Define.CatSkinName.Cat01, 1 },
        {Define.CatSkinName.Cat02, 1 }
    };

    // InGame
    private List<int> _session = new List<int>() 
        {10, 20, 30, 50, 100, 150, 200, 300, 400, 500, 600, 800, 1000, 1200};

    private int _ingame_Score;
    private bool _rewardFeather;
    private bool _rewardShield;
    private bool _prevFirstUpgrade;
    #endregion





    protected override void Awake()
    {
        if (_isInitialized) return;

        _isInitialized = true;
        base.Awake();
        RefreshGoldEvent = ASM.GetAsset<GameEventSO>("@Event_RefreshGoldEvent");
        Load();
        Device_Manager.Instance.Initialize();
    }






    #region Getter

    public int Gold => _gold;

    public int Upgrade_Combo_Level => _upgrade_Combo_Level;
    public int Upgrade_Feather_Level => _upgrade_Feather_Level;
    public int Upgrade_Shield_Level => _upgrade_Shield_Level;


    public bool IsMute_BGM => _isMute_BGM;
    public bool IsMute_SFX => _isMute_SFX;
    public bool ISMute_Haptic => _isMute_Haptic;

    public int Skin_Cat => _skin_Cat;


    public bool IsFirstStart => _isFirstStart;

    public int BestScore => _bestScore;

    public int StartGameNum => _startGameNum;

    public bool FirstUpgrade { get
        {
            if (_firstUpgrade != _prevFirstUpgrade) 
            {
                _prevFirstUpgrade = _firstUpgrade;
                return true;
            }
            return false;
        } }


    // InGame
    public int InGame_Score => _ingame_Score;
    public bool RewardFeather => _rewardFeather;
    public bool RewardShield => _rewardShield;

    #endregion





    #region Setter

    public bool AddGold(int gold)
    {
        if (_gold + gold < 0) return false;
        _gold += gold;
        RefreshGoldEvent.RaiseEvent();
        return true;
    }

    public bool Upgrade_Combo()
    {
        if (Upgrade_Combo_Level + 1 > Define.Max_Upgrade_Combo_Level) return false;
        _upgrade_Combo_Level++;
        if(!_firstUpgrade) SetFirstUpgrade();
        Save();
        return true;
    }

    public bool Upgrade_Feather()
    {
        if (Upgrade_Feather_Level + 1 > Define.Max_Upgrade_Feather_Level) return false;
        _upgrade_Feather_Level++;
        if (!_firstUpgrade) SetFirstUpgrade();
        Save();
        return true;
    }

    public bool Upgrade_Shield()
    {
        if (Upgrade_Shield_Level + 1 > Define.Max_Upgrade_Shield_Level) return false;
        _upgrade_Shield_Level++;
        if (!_firstUpgrade) SetFirstUpgrade();
        Save();
        return true;
    }


    public void SetSkinCat(int num)
    {
        _skin_Cat = num;
        Save();
    }

    public void SetMuteBGM(bool isMute_BGM)
    {
        _isMute_BGM = isMute_BGM;
        Save();
    }

    public void SetMuteSFX(bool isMute_SFX)
    {
        _isMute_SFX = isMute_SFX;
        Save();
    }
    public void SetMuteHaptic(bool isMute_Haptic)
    {
        _isMute_Haptic = isMute_Haptic;
        Save();
    }


    public void SetIsFirstStart(bool isFirstStart) { _isFirstStart = isFirstStart; }

    public void SetBestScore(int score)
    {
        _bestScore = score > _bestScore ? score : _bestScore;
    }


    //InGame
    public void SetInGameScore(int score) 
    {
        _ingame_Score = score;
        Scoring_Event(score);
        SetBestScore(_ingame_Score);
    }

    public void SetRewardFeather(bool isRewardFeather) { _rewardFeather = isRewardFeather; }
    public void SetRewardShield(bool isRewardShield) { _rewardShield = isRewardShield; }

    public void SetStartGameNum(int initNum = -1)
    {
        _startGameNum++;
        if (initNum == 0)_startGameNum = initNum;
        Save();
    }

    public void SetFirstUpgrade() {  _firstUpgrade = true;}


    #endregion




    #region Save & Load
    public void Save()
    {
        SecurityModule.SetInt(Define.PLAYER_GOLD, _gold);

        SecurityModule.SetInt(Define.PLAYER_UPGRADE_COMBO, _upgrade_Combo_Level);
        SecurityModule.SetInt(Define.PLAYER_UPGRADE_FEATHER, _upgrade_Feather_Level);
        SecurityModule.SetInt(Define.PLAYER_UPGRADE_SHIELD, _upgrade_Shield_Level);

        DeviceManager.IsSfxMuted = _isMute_SFX;

        SecurityModule.SetInt(Define.PLAYER_SKIN_CAT, _skin_Cat);

        SecurityModule.SetBool(Define.PLAYER_ISFIRSTSTART, _isFirstStart);

        SecurityModule.SetInt(Define.PLAYER_BEST_SCORE, _bestScore);

        SecurityModule.SetInt(Define.PLAYER_START_GAME_NUM, _startGameNum);

        SecurityModule.SetBool(Define.PLAYER_FIRST_UPGRADE, _firstUpgrade);
    }

    private void SaveDicData(IDictionary dic, string saveName)
    {
        string saveString = "";

        foreach(DictionaryEntry item in dic)
        {
            saveString += ((int)item.Key).ToString();
            saveString += item.Value.ToString();
        }

        SecurityModule.SetString(saveName, saveString);
    }


    private void Load()
    {
        _gold = SecurityModule.GetInt(Define.PLAYER_GOLD, 200000);

        _upgrade_Combo_Level = SecurityModule.GetInt(Define.PLAYER_UPGRADE_COMBO, 0);
        _upgrade_Feather_Level = SecurityModule.GetInt(Define.PLAYER_UPGRADE_FEATHER, 0);
        _upgrade_Shield_Level = SecurityModule.GetInt(Define.PLAYER_UPGRADE_SHIELD, 0);

        _isMute_SFX = DeviceManager.IsSfxMuted;

        _skin_Cat = SecurityModule.GetInt(Define.PLAYER_SKIN_CAT, 0);

        _isFirstStart = SecurityModule.GetBool(Define.PLAYER_ISFIRSTSTART, false);

        _bestScore = SecurityModule.GetInt(Define.PLAYER_BEST_SCORE, 0);

        _startGameNum = SecurityModule.GetInt(Define.PLAYER_START_GAME_NUM, 0);

        _firstUpgrade = SecurityModule.GetBool(Define.PLAYER_FIRST_UPGRADE, false);
        _prevFirstUpgrade = _firstUpgrade;
    }

    #endregion



    #region Application Quit


    private void Scoring_Event(int score)
    {
        for(int i = _session.Count - 1; i >= 0; i--)
        {
            if(score >= _session[i] && _session[i] > _bestScore)
            {
                SingularSDK.Event(new Dictionary<string, object>() { { "scoring", _session[i].ToString() } }, "normal_score");
                FirebaseAnalytics.LogEvent("normal_score", "scoring", "_session[i].ToString()");
            }
        }
    }

    

    #endregion

}
