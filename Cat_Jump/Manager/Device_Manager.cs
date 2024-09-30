using Scripts.Framework.Modules.DeviceModules;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Modules.DeviceModules.VibeModule;
using Scripts.Framework.Modules.SingletonModule.CentralSingleton;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class Device_Manager : IndividualSingletonPersist<Device_Manager>
{
    private bool _isInitialized = false;

    private DeviceManager _dm;


    [HideInInspector] public SoundController Sound;


    public void Initialize()
    {
        if(_isInitialized) return;
        _isInitialized = true;

        _dm = CentralProcessor.I.GetSingleton<DeviceManager>();
        _dm.Initialize();

        Sound = _dm.AttachSoundController(gameObject);

        Vibration.Init();

        DeviceManager.IsSfxMuted = true;
    }

    public void OnVibe(int m_second)
    {
        if (Data_Manager.Instance.ISMute_Haptic) return;
        Vibration.VibrateAndroid(m_second);
    }


}
