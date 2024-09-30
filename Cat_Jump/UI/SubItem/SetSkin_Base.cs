using Scripts.Framework.Events.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum SkinTypeEnum
{
    None,
    Cat,
    Pudding,
    BG
}

[Serializable]
public class SetSkin_Base
{
    
    [HideInInspector] public SkinTypeEnum skinType = SkinTypeEnum.None;
    [HideInInspector] public bool isRandom;

    [HideInInspector] public Define.CatSkinName catSkinName;
    [HideInInspector] public Define.PuddingSkinName puddingSkinName;
    [HideInInspector] public Define.BGSkinName bgSkinName;


}
