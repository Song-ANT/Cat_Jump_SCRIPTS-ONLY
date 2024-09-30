using Cinemachine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Managers.Asset.Core;
using Scripts.Framework.Managers.ObjectPool.Core;
using Scripts.Framework.Managers.ObjectPool.Modular;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;



public class Game_Manager : MonoBehaviour
{
    [Header ("AssetReference")]
    [SerializeField] private AssetReference CatRference;
    [SerializeField] private AssetReference PuddingReference;
    [SerializeField] private AssetReference CameraLookReference;
    [SerializeField] private AssetReference StateCameraReference;
    [SerializeField] private AssetReference GameEndCameraReference;
    [SerializeField] private AssetReference GameEndCameraGroupReference;
    [SerializeField] private AssetReference PuddingSpoonReference;
    [SerializeField] private AssetReference FeatherReference;


    [Header("Broadcaster")]
    [SerializeField] private GameEventSO<int> SetComboTextEvent;
    [SerializeField] private GameEventSO<int> SetPerfectComboTextEvent;


    [Header("Listening")]
    [SerializeField] private GameEventListener<PuddingData> EnterTopEvent;
    [SerializeField] private GameEventListener<bool> EnterSideEvent;
    [SerializeField] private GameEventListener<bool> EnterShieldSideEvent;
    [SerializeField] private GameEventListener StartBtnEvent;
    [SerializeField] private GameEventListener<bool> ComboEvent;
    [SerializeField] private GameEventListener PerfectComboEvent;
    [SerializeField] private GameEventListener<bool> ReviveEvent;
    [SerializeField] private GameEventListener<bool> GameOverEvent;


    private GameObject _catAsset;
    private GameObject _puddingAsset;
    private GameObject _cameraLookAsset;
    private GameObject _stateCameraAsset;
    private GameObject _gameEndCameraAsset;
    private GameObject _gameEndCameraGroupAsset;
    private GameObject _puddingSpoonAsset;
    private GameObject _featherAsset;

    private GameObject _catObject;
    private CinemachineVirtualCamera _gameEndCamera;
    private PoolData _spoonData;


    private PuddingPatternFactory _pattern;
    private Queue<PuddingData> _puddingQueue = new Queue<PuddingData>();
    private Dictionary<int, PuddingController> _puddingControllers = new Dictionary<int, PuddingController>();

    private bool _isGamePlaying;

    private int _stairs = 0;
    private int _curStairs = 0;
    private int puddingCount;
    private bool _isSpawning;
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


    private int _combo;
    private int _score;
    private int _difficulty;

    private Coroutine _spawnMonitorCoroutine;

    #region Init
    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1.0f / 60.0f;
        _catAsset = ASM.GetAsset<GameObject>(CatRference);
        _puddingAsset = ASM.GetAsset<GameObject>(PuddingReference);
        _cameraLookAsset = ASM.GetAsset<GameObject>(CameraLookReference);
        _stateCameraAsset = ASM.GetAsset<GameObject>(StateCameraReference);
        _gameEndCameraAsset = ASM.GetAsset<GameObject>(GameEndCameraReference);
        _gameEndCameraGroupAsset = ASM.GetAsset<GameObject>(GameEndCameraGroupReference);
        _puddingSpoonAsset = ASM.GetAsset<GameObject>(PuddingSpoonReference);
        _featherAsset = ASM.GetAsset<GameObject>(FeatherReference);
        _pattern = new PuddingPatternFactory();

        _score = -1;
        _combo = 0;

        Data_Manager.Instance.SetInGameScore(0);
        

        InitSpawn();
        Data_Manager.Instance.Save();

        _spoonData = new PoolData("Spoon", _puddingSpoonAsset, 5);
        PoolManager.Register(_spoonData);

        EnterTopEvent.Subscribe();
        EnterTopEvent.SubstitutionEvent(OnEnterTopEventStarted);

        EnterSideEvent.Subscribe();
        EnterSideEvent.SubstitutionEvent((bool a) => { _combo = 0; });

        EnterShieldSideEvent.Subscribe();
        EnterShieldSideEvent.SubstitutionEvent(OnEnterShieldSideEventStarted);

        StartBtnEvent.Subscribe();
        StartBtnEvent.SubstitutionEvent(OnStartBtnClicked);

        ComboEvent.Subscribe();
        ComboEvent.SubstitutionEvent(OnComboEventStarted);

        PerfectComboEvent.Subscribe();
        PerfectComboEvent.SubstitutionEvent(OnPerfectComboEventStarted);

        ReviveEvent.Subscribe();
        ReviveEvent.SubstitutionEvent(OnReviveEventStarted);

        GameOverEvent.Subscribe();
        GameOverEvent.SubstitutionEvent(OnGameOverEventStarted);
    }

    private void OnDisable()
    {
        EnterTopEvent.Unsubscribe();
        EnterShieldSideEvent.Unsubscribe();
        StartBtnEvent.Unsubscribe();
        ComboEvent.Unsubscribe();
        PerfectComboEvent.Unsubscribe();
        ReviveEvent.Unsubscribe();
        GameOverEvent.Unsubscribe();
    }
    #endregion


    #region Function
    private void InitSpawn()
    {
        _catObject = Instantiate(_catAsset, new Vector3(0, 2.5f, -1), Quaternion.identity, transform);

        CameraSet();
        _difficulty = 0;

        SpawnPudding(new PuddingData { isZeroStair = true }, 0).Forget() ;
        _curStairs = 1;
    }

    private void CameraSet()
    {
        // 기본 카메라 및 고양이 타겟 오브젝트 소환
        GameObject cameraLookObject = Instantiate(_cameraLookAsset, new Vector3(0, 2.5f, -1), Quaternion.identity, transform);

        GameObject stateCameraObject = Instantiate(_stateCameraAsset, transform);
        stateCameraObject.GetComponent<StateDrivenCamera>().Initialized(cameraLookObject.transform);
       
        

        // 게임 종료 타겟그룹 및 카메라 소환
        CinemachineTargetGroup targetGroup = Instantiate(_gameEndCameraGroupAsset, transform).GetComponent<CinemachineTargetGroup>();
        GameObject obj = new GameObject();
        obj.name = "InitPosition";
        obj.transform.parent = transform;
        targetGroup.AddMember(obj.transform, 1, 0);

        GameObject obj2 = new GameObject();
        obj2.name = "EndPosition";
        obj2.transform.parent = transform;
        obj2.transform.position = new Vector3(0, 60, 0);
        targetGroup.AddMember(obj2.transform, 1, 0);

        GameObject _gameEndCameraObject = Instantiate(_gameEndCameraAsset, transform);
        _gameEndCamera = _gameEndCameraObject.GetComponent<CinemachineVirtualCamera>();
        _gameEndCamera.Priority = 9;
        _gameEndCamera.Follow = targetGroup.transform;
        _gameEndCamera.LookAt = targetGroup.transform;

        targetGroup.AddMember(cameraLookObject.transform, 1, 0);
    }

    private async UniTask SpawnPudding(PuddingData data, int prevStairs)
    {
        if (prevStairs < _stairs)
        {
            return;
        }
        puddingCount++;
        int puddingX = data.isZeroStair ? 0 : data.isRightMove ? -15 : 15;

        Vector3 dir = new Vector3(puddingX, _stairs++ * 2.65f, 1);
        GameObject pudding = Instantiate(_puddingAsset, dir, Quaternion.identity, transform);
        if (!data.isZeroStair)
        {
            //GameObject spoon = Instantiate(_puddingSpoonAsset, dir, Quaternion.identity, transform);
            GameObject spoon = PoolManager.OnGet("Spoon", transform);
            spoon.GetComponent<PuddingSpoon>().Initialize(pudding.transform, data, dir);
        }
        PuddingController controller = pudding.GetComponent<PuddingController>();
        controller.Initialized(data);
        if(data.stairs != 1) _puddingControllers.Add(data.stairs, controller);
        
        pudding.name = $"{_stairs}";

        ResetSpawnMonitorCoroutine();

        await UniTask.Yield();
    }

    private void ResetSpawnMonitorCoroutine()
    {
        if (_spawnMonitorCoroutine != null)
        {
            StopCoroutine(_spawnMonitorCoroutine);
        }
        _spawnMonitorCoroutine = StartCoroutine(MonitorSpawnPuddingTime());
    }

    private int GetContinuousCombo(int combo)
    {
        int currentNumber = 1;
        int count = 0;

        while (count < combo)
        {
            for (int i = 0; i < currentNumber; i++)
            {
                count++;
                if (count == combo)
                {
                    return currentNumber;
                }
            }
            currentNumber++;
            if (Data_Manager.Instance.Upgrade_Combo_Level+3 < currentNumber) return currentNumber - 1;
        }

        return 0;
    }

    private void IncreaseDifficulty()
    {
        if (_difficulty > 8) return;
        _difficulty++;
        int patternProb = PuddingPatternFactory.PatternProb - _difficulty;
        float modifySpeed = PuddingPattern_Base.ModifySpeed + _difficulty;
        float modifyDelay = PuddingPattern_Base.ModifyDelay + _difficulty;
        SetDifficulty(patternProb, modifySpeed, modifyDelay);
    }

    private void SetDifficulty(int patternProb, float modifySpeed, float modifyDelay)
    {
        PuddingPatternFactory.PatternProb = patternProb;
        PuddingPattern_Base.ModifySpeed = modifySpeed;
        PuddingPattern_Base.ModifyDelay = modifyDelay;
    }

    #endregion


    #region Event
    // 푸딩 패턴 유도 이벤트 - 푸딩 윗면 밟을 시 발동
    public async void OnEnterTopEventStarted(PuddingData data)
    {
        if (!_isGamePlaying) return;
        if (data.stairs == _curStairs) return;

        _curStairs = data.stairs;
        if (_curStairs != 0 && _curStairs % 20 == 0) IncreaseDifficulty();

        if (_isSpawning) return;

        await _semaphore.WaitAsync();
        _isSpawning = true;
        try
        {

            switch (data.type)
            {
                case PuddingType.Feather:
                    _puddingQueue = _pattern.SelectPattern(data.stairs, PuddingPatternType.Feather);
                    Instantiate(_featherAsset, new Vector2(0, (_curStairs * 2.65f) + 20), Quaternion.identity, transform);
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    break;
                default:
                    _puddingQueue = _pattern.SelectPattern(data.stairs);
                    break;
            }

            await SpawnPuddingFromQueue();

        }

        finally
        {
            _isSpawning = false;
            if (_semaphore.CurrentCount == 0)
            {
                _semaphore.Release();
            }
        }


    }

    // 푸딩 패턴 유도 이벤트 - 쉴드로 푸딩 막을 시 발동
    public async void OnEnterTopEventStarted(PuddingData data, bool isShield)
    {
        if (!isShield && data.stairs == _curStairs) return;
        _curStairs = data.stairs;


        _puddingQueue.Clear();
        if (_semaphore.CurrentCount == 0)
        {
            _semaphore.Release();
        }

        await _semaphore.WaitAsync();
        _isSpawning = true;
        try
        {

            switch (data.type)
            {
                case PuddingType.Feather:
                    _puddingQueue = _pattern.SelectPattern(data.stairs, PuddingPatternType.Feather);
                    Instantiate(_featherAsset, new Vector2(0, (_curStairs * 2.65f) + 20), Quaternion.identity, transform);
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                    break;
                default: 
                    _puddingQueue = _pattern.SelectPattern(data.stairs);
                    break;
            }

            await SpawnPuddingFromQueue();

        }

        finally
        {
            _isSpawning = false;
            _semaphore.Release();
        }


    }


    // 쉴드 이벤트
    private void OnEnterShieldSideEventStarted(bool isShield)
    {
        if (!isShield) _isGamePlaying = false;
        _puddingQueue.Clear();

        var controllersToDisappear = _puddingControllers
            .Where(kvp => kvp.Key > _curStairs)
            .Select(kvp => kvp.Value)
            .ToList();

        List<int> stairsKeysToRemove = new List<int>();
        foreach (var controller in controllersToDisappear)
        {
            _puddingControllers.Remove(controller.Data.stairs);
            controller.Disappear();
        }

        // 전부 사라지는 것 기다리기
        UniTask.Delay(TimeSpan.FromSeconds(0.8f)).ContinueWith(() =>
        {
            _stairs = _curStairs;
            if(isShield) OnEnterTopEventStarted(new PuddingData { stairs = _curStairs, type = PuddingType.Normal }, true);
        });
        
    }


    // 게임 시작
    private void OnStartBtnClicked()
    {
        if (!Data_Manager.Instance.IsFirstStart)
        {
            GameObject tutorial_UI = ASM.GetAsset<GameObject>("Tutorial_UI");
            Instantiate(tutorial_UI);
            return;
        }
        _isGamePlaying = true;
        Data_Manager.Instance.SetStartGameNum();

        PuddingData data = new PuddingData() { stairs = 2 };
        if (Data_Manager.Instance.RewardFeather)
        {
            data.type = PuddingType.Feather;
            Data_Manager.Instance.SetRewardFeather(false);
        }

        SpawnPudding(data, 1).Forget();

    }



    // 콤보 발동
    private void OnComboEventStarted(bool isFeather)
    {
        if (_curStairs == 0) return;
        if(!isFeather) _combo = 0;
        _score += 1;
        Data_Manager.Instance.SetInGameScore(_score);
        Data_Manager.Instance.AddGold(1);
        SetComboTextEvent.RaiseEvent(_score);
    }

    // 퍼펙트 콤보 발동
    private void OnPerfectComboEventStarted()
    {
        _combo++;
        int continuousCombo = GetContinuousCombo(_combo);
        _score += continuousCombo + 1;
        Data_Manager.Instance.SetInGameScore(_score);
        Data_Manager.Instance.AddGold(1);

        SetPerfectComboTextEvent.RaiseEvent(continuousCombo);
        SetComboTextEvent.RaiseEvent(_score);
        SetComboSound();
    }

    private void SetComboSound()
    {
        switch (_combo)
        {
            case 1: Device_Manager.Instance.Sound.PlayClip(SoundClipName.Combo_1, 1, false); break;
            case 2: Device_Manager.Instance.Sound.PlayClip(SoundClipName.Combo_2, 1, false); break;
            case 3: Device_Manager.Instance.Sound.PlayClip(SoundClipName.Combo_3, 1, false); break;
            case 4: Device_Manager.Instance.Sound.PlayClip(SoundClipName.Combo_4, 1, false); break;
            case 5: Device_Manager.Instance.Sound.PlayClip(SoundClipName.Combo_5, 1, false); break;
            default: Device_Manager.Instance.Sound.PlayClip(SoundClipName.Combo_more, 1, false); break;
        }
    }

    // 부활 위치 지정 & 다음 패턴 시작
    private void OnReviveEventStarted(bool isReviveBtnClicked)
    {
        if (!isReviveBtnClicked)
        {
            OnGameOverEventStarted(true);
            return;
        }
        _catObject.transform.position = new Vector3(0, _curStairs * 2.65f, 0);
        
        _isGamePlaying = false;

        UniTask.Delay(TimeSpan.FromSeconds(3f)).ContinueWith(() =>
        {
            _isGamePlaying = true;
           OnEnterTopEventStarted(new PuddingData { stairs = _curStairs, type = PuddingType.Normal }, true);
        });
    }

    // 부활 후 죽음 
    private void OnGameOverEventStarted(bool isRevive)
    {
        if (isRevive)
        {
            PoolManager.Unregister("Spoon");
            _isGamePlaying = false;
            _gameEndCamera.Priority = 11;
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.Crash_Parachute, 1, false);
        }
    }
    #endregion


    #region Coroutine
    // 푸딩 패턴 생성 코루틴
    private async UniTask SpawnPuddingFromQueue()
    {
        while (_puddingQueue.Count > 0)
        {
            var data = _puddingQueue.Dequeue();
            await SpawnPudding(data, data.stairs);
            if (data.waitTime > 1) _isSpawning = false;
            await UniTask.Delay(TimeSpan.FromSeconds(data.waitTime));
        }
        
        _isSpawning = false;

    }

    // 푸딩이 일정 이상 소환되지 않을 경우 새로운 패턴 시작
    private IEnumerator MonitorSpawnPuddingTime()
    {
        float deltaTime = 0f;

        while (deltaTime < 3f)
        {
            deltaTime += Time.deltaTime;
            if (!_isGamePlaying) yield break;
            yield return null;
        }


        OnEnterTopEventStarted(new PuddingData { stairs = _curStairs, type = PuddingType.Normal }, true);
    }



    #endregion

}
