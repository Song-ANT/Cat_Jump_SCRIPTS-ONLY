using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum LayerName
    {
        Ground = 6,
        Side = 7,
        Cat = 8
    }

    public enum SortingLayerName
    {
        BackGround,
        Default,
        Pudding,
        Cat,
        InGame,
        TouchPanel,
        MainMenuUI
    }

    public enum CatAnimationName
    {
        A0_Main_loop,
        A1_Main_idle01,
        A1_Main_idle02,
        A1_Main_idle03,
        A2_Game_start,
        A3_game1_loop,
        A3_game2_edgeL,
        A4_game_landing,
        A4_game_touch,
        A5_game_jump01,
        A5_game_jump02,
        A5_game_jump03,
        A6_item_rocket,
        A7_game_overL,
        A7_game_overR,
        A8_game_downL,
        A8_game_downR
    }

    public enum PuddingAnimationName
    {
        A01_Idle01,
        A02_in,
        A03_edgeL,
        A03_edgeR,
        A04_push
    }

    public enum SpecialPuddingName
    {
        E1,
        E2
    }

    public enum SpecialPuddingName_Skin
    {
        G1,
        G2,
        G3
    }


    #region SetSkinName
    public enum CatSkinName
    {
        Cat00 = 0,
        Cat01 = 1,
        Cat02 = 2
    }

    public enum PuddingSkinName
    {

    }

    public enum BGSkinName
    {

    }
    #endregion

    #region CameraAnimation
    public enum CameraAnimation
    {
        CatJumpSceneCamera,
        CollectCamera
    }
    #endregion

    public static int Max_Upgrade_Combo_Level = 7;
    public static int Max_Upgrade_Feather_Level = 10;
    public static int Max_Upgrade_Shield_Level = 2;



    public const string PLAYER_GOLD = "PLAYER_GOLD";
    public const string PLAYER_UPGRADE_COMBO = "PLAYER_UPGRADE_COMBO";
    public const string PLAYER_UPGRADE_FEATHER = "PLAYER_UPGRADE_FEATHER";
    public const string PLAYER_UPGRADE_SHIELD = "PLAYER_UPGRADE_SHIELD";
    public const string PLAYER_SKIN_CAT = "PLAYER_SKIN_CAT";
    public const string PLAYER_SKIN_CAT_DIC = "PLAYER_SKIN_CAT_DIC";
    public const string PLAYER_ISFIRSTSTART = "PLAYER_ISFIRSTSTART";
    public const string PLAYER_BEST_SCORE = "PLAYER_BEST_SCORE";
    public const string PLAYER_START_GAME_NUM = "PLAYER_START_GAME_NUM";
    public const string PLAYER_FIRST_UPGRADE = "PLAYER_FIRST_UPGRADE";


    public const string TERMS_SERVICE_URL = "https://sites.google.com/view/actionfit-termsofservice-en";
    public const string PRIVATE_POLICY_URL = "https://sites.google.com/view/actionfit-privacypolicy-en";
}
