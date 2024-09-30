using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Managers.Asset.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UI_Manager : MonoBehaviour
{
    [Header("Asset Reference")]
    [SerializeField] private AssetReference MainMenu_Reference;
    [SerializeField] private AssetReference InGame_Reference;

    [Header("Listener")]
    [SerializeField] private GameEventListener StartBtnEvent;

    private GameObject MainMenu_Object;
    private GameObject InGame_Object;


    private void Start()
    {
        MainMenu_Object = Instantiate(ASM.GetAsset<GameObject>(MainMenu_Reference), transform);
        InGame_Object = Instantiate(ASM.GetAsset<GameObject>(InGame_Reference), transform);
        InGame_Object.SetActive(false);

        StartBtnEvent.Subscribe();
        StartBtnEvent.SubstitutionEvent(OnStartBtnEventStarted);
    }

    private void OnDisable()
    {
        StartBtnEvent.Unsubscribe();
    }

    private void OnStartBtnEventStarted()
    {
        MainMenu_Object.SetActive(false);
        InGame_Object.SetActive(true);
    }

}
