using Scripts.Framework.Events.Interfaces;
using Scripts.Framework.Events.SO;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;




public class SpineHandler : MonoBehaviour
{
    private SkeletonAnimation _spine;

    private bool _isFlip_X;

    void Awake()
    {
        _spine = GetComponent<SkeletonAnimation>();
        _spine.Initialize(true);
    }

    public string GetCurAnimation(int index = 0)
    {
        return _spine.AnimationState.GetCurrent(index).Animation.Name;
    }

    public void ChangeAnimation(string state, int trackIndex = 0, bool loop = true, bool flipX = false)
    {
        if(_isFlip_X) ResetFlipX();
        _spine.AnimationState.SetAnimation(trackIndex, state, loop);
        if(flipX) FlipX();
    }

    public void ChangeAnimation(string state, float animationSpeed, int trackIndex = 0, bool loop = true, bool flipX = false)
    {
        if (_isFlip_X) ResetFlipX();
        SetAnimationSpeed(animationSpeed);
        _spine.AnimationState.Complete += OnAnimationSpeedSetDefault;
        _spine.AnimationState.SetAnimation(trackIndex, state, loop);
        if (flipX) FlipX();
    }
    private void OnAnimationSpeedSetDefault(TrackEntry trackEntry)
    {
        SetAnimationSpeed(1f);
        _spine.AnimationState.Complete -= OnAnimationSpeedSetDefault;
    }

    public void FlipX()
    {
        _spine.skeleton.ScaleX *= -1;
        _isFlip_X = true;
    }

    public void ResetFlipX()
    {
        if (_isFlip_X)
        {
            _spine.skeleton.ScaleX *= -1;
            _isFlip_X = false;
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        _spine.timeScale = speed;
    }

    public void ChangeSkin(string skinName)
    {
        _spine.Initialize(true);
        _spine.skeleton.SetSkin(skinName);
    }

    public void SetSpineData(SkeletonDataAsset newSkin)
    {
        _spine.skeletonDataAsset = newSkin;
        _spine.Initialize(true);
    }

    public void SetSpineData(SkeletonDataAsset newSkin, string skinName)
    {
        _spine.skeletonDataAsset = newSkin;
        _spine.Initialize(true);
        _spine.skeleton.SetSkin(skinName);
    }


    public void AddAnimation(string state, int trackIndex = 0, bool loop = true, bool flipX = false)
    {
        if (_isFlip_X) ResetFlipX();
        _spine.AnimationState.AddAnimation(trackIndex, state, loop, 0);
        if (flipX) FlipX();
    }
}
