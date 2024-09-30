using DG.Tweening;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Managers.Asset.Core;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Utility;
using Spine.Unity;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum PuddingType
{
    Normal,
    Feather,
    Shield,
    Skin
}

public class PuddingData
{
    public PuddingType type = PuddingType.Normal;
    public int stairs = 1;
    public bool isRightMove = false;
    public float moveSpeed = 11f;
    public bool isZeroStair = false;
    public float waitTime = 1f;
}

public class PuddingController : MonoBehaviour
{
    [Header("AssetReference")]
    [SerializeField] private AssetReference defaultPudding;
    [SerializeField] private AssetReference specialPudding;

    [Header("Pudding")]
    public PuddingType _type;

    [Header("Broadcaster")]
    [SerializeField] private GameEventSO<PuddingData> EnterTopEvent;
    [SerializeField] private GameEventSO<bool> EnterSideEvent;
    [SerializeField] private GameEventSO<bool> EnterTopCliffEvent;
    [SerializeField] private GameEventSO<bool> ComboEvent;
    [SerializeField] private GameEventSO PerfectComboEvent;
    [SerializeField] private GameEventSO PuddingArriveEvent;

    [SerializeField] private MeshRenderer _puddingSpineMesh;

    private PuddingData _data;
    private SpineHandler _spine;

    private SkeletonDataAsset _defaultPuddingAsset;
    private SkeletonDataAsset _specialPuddingAsset;

    private Rigidbody2D _rbody;
    private Vector2 _velocity;
    private bool _isZeroStairs;
    private float _moveInput;
    private bool _initialized;

    private bool _isTrigger;
    private bool _isSideTrigger;
    private bool _isGroundTrigger;
    private bool _isArrive;

    private readonly object _lock = new();
    public PuddingData Data => _data;
    public bool IsSideTriggerHandler { get; set; }
    public bool IsArrive => _isArrive;


    #region Init
    private void Awake()
    {
        _rbody = GetComponent<Rigidbody2D>();
        _spine = GetComponentInChildren<SpineHandler>();

        _defaultPuddingAsset = ASM.GetAsset<SkeletonDataAsset>(defaultPudding);
        _specialPuddingAsset = ASM.GetAsset<SkeletonDataAsset>(specialPudding);
    }

    private void FixedUpdate()
    {
        if (!_initialized) return;
        if (_isZeroStairs || _isArrive) return;

        if (transform.position.x * _moveInput > 0)
        {
            Arrive();
        }
        Move();
    }



    public void Initialized(PuddingData data)
    {
        _data = data;
        if (_data.isZeroStair) _isZeroStairs = true;
        _type = _data.type;
        SetSkin(_type);
        _moveInput = _data.isRightMove ? 1 : -1;
        _velocity.x = _moveInput * _data.moveSpeed;

        _puddingSpineMesh.sortingOrder = data.stairs;

        _initialized = true;
    }
    #endregion


    #region Function
    private void SetSkin(PuddingType type)
    {
        switch (type)
        {
            case PuddingType.Normal:
                _spine.SetSpineData(_defaultPuddingAsset);
                break;
            case PuddingType.Feather:
                _spine.SetSpineData(_specialPuddingAsset, Define.SpecialPuddingName.E1.ToString());
                break;
            case PuddingType.Shield:
                _spine.SetSpineData(_specialPuddingAsset, Define.SpecialPuddingName.E2.ToString());
                break;
            case PuddingType.Skin:
                _spine.SetSpineData(_specialPuddingAsset, Util.GetRandomEnumValue<Define.SpecialPuddingName_Skin>().ToString());
                break;
        }

    }


    private void Move()
    {
        _rbody.velocity = _velocity;
    }

    // 푸딩 정지 메서드
    private void Arrive(bool isZeroPosition = true, bool isCliff = false)
    {
        _velocity.x = 0;
        _rbody.velocity = _velocity;
        if (isZeroPosition) transform.position = new Vector3(0, transform.position.y, transform.position.z);
        _isZeroStairs = true;

        ComboEventRaised(isZeroPosition);
        PuddingArriveEvent.RaiseEvent();

        if (isCliff && _data.type == PuddingType.Normal)
        {
            Define.PuddingAnimationName animationName = _moveInput == 1 ? Define.PuddingAnimationName.A03_edgeR : Define.PuddingAnimationName.A03_edgeL;
            _spine.ChangeAnimation(animationName.ToString(), 0, false);
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.Landing_Caution, 1, false);
            Device_Manager.Instance.OnVibe(250);
        }
        else
        {
            _spine.ChangeAnimation(Define.PuddingAnimationName.A02_in.ToString(), 0, false);
        }

        _isArrive = true;
    }

    private void ComboEventRaised(bool isZeroPosition)
    {
        if (_data.moveSpeed < 50)
        {
            if (isZeroPosition)
            {
                PerfectComboEvent.RaiseEvent();
            }
            else
            {
                ComboEvent.RaiseEvent(false);
            }
            return;
        }
        ComboEvent.RaiseEvent(true);
    }

    // 푸딩 쉴드에 의한 사라짐 기능
    public int Disappear()
    {
        Vector2 targetPosition = new Vector2(_moveInput * -15, transform.position.y);
        transform.DOMove(targetPosition, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Destroy(gameObject); // 푸딩 오브젝트 제거
        });
        return _data.stairs;
    }
    #endregion



    #region Event
    // 푸딩 이벤트 lock 메서드
    public void OnTriggerEvent(bool isGround, bool isPuddingSide_R, bool isCliff = false)
    {
        lock (_lock)
        {
            if (_isTrigger) return;
            _isTrigger = true;

            if (isGround) OnTriggerGroundEvent(isCliff);
            else OnTriggerSideEvent(isPuddingSide_R);

            StartCoroutine(ResetTriggerFlag());
        }
    }

    // 푸딩 윗면 이벤트
    public void OnTriggerGroundEvent(bool isCliff)
    {
        if (_isSideTrigger) return;
        _isGroundTrigger = true;
        if (!_isArrive) Arrive(false, isCliff);
        if (isCliff && _data.type == PuddingType.Normal) EnterTopCliffEvent.RaiseEvent(_moveInput == 1);
        EnterTopEvent.RaiseEvent(_data);
    }

    // 푸딩 옆면 이벤트
    public void OnTriggerSideEvent(bool isPuddingSide_R)
    {
        if (_isGroundTrigger) return;
        _isSideTrigger = true;
        EnterSideEvent.RaiseEvent(isPuddingSide_R);
    }

    // 푸딩 lock 메서드 플래그 초기화
    private IEnumerator ResetTriggerFlag()
    {
        yield return new WaitForSeconds(0.1f); // 0.2초 대기
        _isTrigger = false;
    }
    #endregion

}
