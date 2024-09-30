using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward_Btn_SubUI : MonoBehaviour
{
    [Header("RewardImage")]
    [SerializeField] private Image Reward_Image;
    [SerializeField] private Sprite Reward_On;
    [SerializeField] private Sprite Reward_Off;
    [SerializeField] private GameObject Ads;
    


    public void SetOn(Action SetTrue)
    {
        AdsCommand command = new AdsCommand(() => Reward(SetTrue), CommandOrderer.Reward);
        SDKIntegrationSystem.Instance.ShowReward(command).Forget();
    }

    public void Reward(Action SetTrue)
    {
        Reward_Image.sprite = Reward_On;
        Ads.SetActive(false);
        SetTrue();
    }

    public void SetOff(Action SetFalse)
    {
        Reward_Image.sprite = Reward_Off;
        Ads.SetActive(true);

        SetFalse();
    }
}
