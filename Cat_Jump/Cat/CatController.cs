using Cysharp.Threading.Tasks;
using Scripts.Framework.Events.Listeners;
using Scripts.Framework.Events.SO;
using Scripts.Framework.Modules.DeviceModules.SoundModule;
using Scripts.Framework.Utility;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CatController : MonoBehaviour
{
    #region Field

    [Header("Broadcaster")]
    [SerializeField] private GameEventSO<bool> EnterShieldSideEvent;
    [SerializeField] private GameEventSO<bool> GameOverEvent;


    [Header("Listening")]
    [SerializeField] private GameEventListener<PuddingData> EnterTopEvent;
    [SerializeField] private GameEventListener<bool> EnterTopCliffEvent;
    [SerializeField] private GameEventListener<bool> EnterSideEvent;
    [SerializeField] private GameEventListener TouchCatJumpEvent;
    [SerializeField] private GameEventListener StartBtnEvent;
    [SerializeField] private GameEventListener<bool> ReviveEvent;
    [SerializeField] private GameEventListener<int> CatSetSkinEvent;


    [Header("Shield Image")]
    [SerializeField] private Image _shield;

    private float GravityScale = 11f;
    private const float MaxJumpHeight = 6f;

    private SpineHandler _spine;
    private Rigidbody2D _rbody;
    private BoxCollider2D _collider;

    private bool _isGamePlaying;

    private bool _isGrounded = true;
    private Vector2 _velocity;
    private bool _jumpInput;
    private bool _isJumping;
    private float dt;

    private bool _isMovingSide;
    private float SideMoveDistance = 7f; // ��� �� ���ư��� �Ÿ�
    private float SideMoveSpeed = 60f;   // ��� �� ���ư��� �ӵ�
    private Coroutine _sideMoveCoroutine;

    private float _gravity;

    private bool _isCliff;
    private bool _isCliff_R;

    private int _shieldStairs = -1;
    private int _shieldCount;

    private bool _isRevive;
    private bool _isDead;

    #endregion


    #region Init

    private void Start()
    {
        _spine = GetComponentInChildren<SpineHandler>();
        _rbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _gravity = Physics2D.gravity.y * GravityScale;
        _rbody.gravityScale = 0;
        _velocity.y = 0;
        _isDead = false;

        OnCatSetSkinEventStarted(Data_Manager.Instance.Skin_Cat);

        // Ǫ�� ���� �̺�Ʈ
        EnterTopEvent.Subscribe();
        EnterTopEvent.SubstitutionEvent(OnEnterTopEventStarted);

        // Ǫ�� �𼭸� �̺�Ʈ
        EnterTopCliffEvent.Subscribe();
        EnterTopCliffEvent.SubstitutionEvent(OnEnterTopCliffEvenStarted);

        // Ǫ�� ���̵� �̺�Ʈ
        EnterSideEvent.Subscribe();
        EnterSideEvent.SubstitutionEvent(OnEnterSideEventStarted);

        // �г� ��ġ �̺�Ʈ
        TouchCatJumpEvent.Subscribe();
        TouchCatJumpEvent.SubstitutionEvent(OnTouchCatJumpEventStarted);

        // ���� ���� �̺�Ʈ
        StartBtnEvent.Subscribe();
        StartBtnEvent.SubstitutionEvent(OnStartBtnEventClicked);

        // ��Ȱ �̺�Ʈ
        ReviveEvent.Subscribe();
        ReviveEvent.SubstitutionEvent(OnReviveEventStarted);

        // ����̽�Ų���� �̺�Ʈ
        CatSetSkinEvent.Subscribe();
        CatSetSkinEvent.SubstitutionEvent(OnCatSetSkinEventStarted);
    }

    private void OnDestroy()
    {
        EnterTopEvent.Unsubscribe();
        EnterTopCliffEvent.Unsubscribe();
        EnterSideEvent.Unsubscribe();
        TouchCatJumpEvent.Unsubscribe();
        StartBtnEvent.Unsubscribe();
        ReviveEvent.Unsubscribe();
        CatSetSkinEvent.Unsubscribe();
    }

    #endregion


    #region Updata & Jump
    private void FixedUpdate()
    {
        dt = Time.deltaTime;
        if (!_isMovingSide)
        {
            if (_jumpInput && _isGrounded)
            {
                Jump();
            }

        }

        ApplyGravity();
        Move();
    }


    // �߷� ����
    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _velocity.y += _gravity * dt;
            if (_velocity.y < 0 && !_isDead) _spine.ChangeAnimation(Define.CatAnimationName.A5_game_jump01.ToString());
            _velocity.y = _velocity.y < -35 ? -35 : _velocity.y;
        }
        else // ���� ������ �ʱ�ȭ
        {
            _velocity.y = 0;
        }
    }

    // ���� ������ �����
    private void Move()
    {
        _rbody.velocity = _velocity;
    }

    // ���� ����
    private void Jump(float jumpHeight = MaxJumpHeight, bool jumpAnimation = true)
    {
        if (!_isGamePlaying) return;
        // 1/2 mv^2 = mgh; => v = (2gh)^(1/2)
        float jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * jumpHeight);
        _velocity.y = jumpVelocity;
        _isGrounded = false;
        _jumpInput = false;
        _isJumping = true;

        Device_Manager.Instance.Sound.PlayClip(SoundClipName.Lending_Jump, 1, false);

        if (jumpAnimation) _spine.ChangeAnimation(Define.CatAnimationName.A5_game_jump01.ToString());
    }

    #endregion


    #region Function
    // �߷� ������ ����
    public void SetGravity(float gravityScaleModify)
    {
        _gravity = Physics2D.gravity.y * gravityScaleModify;
    }

    // ��� �̺�Ʈ
    public void EnterPuddingSide(bool isPuddingSide_R)
    {
        if (_isMovingSide) return;
        if (_sideMoveCoroutine != null)
        {
            StopCoroutine(_sideMoveCoroutine);
        }

        _collider.enabled = false;
        _velocity.y = 0;
        _spine.ChangeAnimation(isPuddingSide_R ? Define.CatAnimationName.A7_game_overR.ToString() : Define.CatAnimationName.A7_game_overL.ToString(), 0, false);
        _sideMoveCoroutine = StartCoroutine(MoveSide(isPuddingSide_R));
    }

    // Ǫ�� ���� ���
    public void EnterPuddingTop(PuddingType type = PuddingType.Normal)
    {
        _isGrounded = true;
        _isJumping = false;

        if (_isCliff && type == PuddingType.Normal)
        {
            _spine.ChangeAnimation(Define.CatAnimationName.A3_game2_edgeL.ToString(), 0, false, _isCliff_R);

        }
        else if (!_isGamePlaying) _spine.ChangeAnimation(Define.CatAnimationName.A0_Main_loop.ToString(), 0);
        else
        {
            _spine.ChangeAnimation(Define.CatAnimationName.A4_game_landing.ToString(), 2f);
            _spine.AddAnimation(Define.CatAnimationName.A3_game1_loop.ToString());
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.Landing_Landing, 0.5f, false);
            if (_isGamePlaying) Device_Manager.Instance.OnVibe(10);
        }
        _isCliff = false;
    }

    // Ǫ�� - ����Ÿ�� ���
    private async UniTaskVoid EnterFeather()
    {
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.Item_Toy, 1, false);
        Device_Manager.Instance.OnVibe(150);
        _spine.ChangeAnimation(Define.CatAnimationName.A3_game1_loop.ToString());
        _isGrounded = true;
        _isJumping = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.Item_Toy_Wind_Up, 1, false);

        float jumpHeight = (3 * Data_Manager.Instance.Upgrade_Feather_Level) + 30;

        Jump(jumpHeight, false);
        _spine.ChangeAnimation(Define.CatAnimationName.A6_item_rocket.ToString());
    }

    // ���� ȹ��
    private void GetShield(int stairs)
    {
        _isCliff = false;
        Device_Manager.Instance.OnVibe(150);
        EnterPuddingTop();
        if (_shieldStairs == stairs) return;
        _shieldStairs = stairs;

        _shieldCount = Data_Manager.Instance.Upgrade_Shield_Level + 1;
        _shield.gameObject.SetActive(true);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.Item_Bubble, 1, false);
    }

    // ���� ���� & ����ŸƮ �ִϸ��̼�
    private void RestartAnimation()
    {
        _isCliff = false;
        _spine.ChangeAnimation(Define.CatAnimationName.A2_Game_start.ToString(), 0, false);
        _spine.AddAnimation(Define.CatAnimationName.A3_game1_loop.ToString());

        _isGrounded = true;
        _isGamePlaying = true;
        _jumpInput = false;
        _isJumping = false;
    }
    #endregion


    #region Coroutine
    // ��� �� ���ư��� �ڷ�ƾ
    private IEnumerator MoveSide(bool isPuddingSide_R)
    {
        _isDead = true;
        Vector2 direction = isPuddingSide_R ? Vector2.right : Vector2.left;
        _isMovingSide = true;
        _isGrounded = false;
        float initialGravityScale = GravityScale;
        SetGravity(0);

        Vector2 startPosition = _rbody.position;
        Vector2 targetPosition = startPosition + (direction * SideMoveDistance) + (Vector2.up * 2);
        float distance = (targetPosition - startPosition).magnitude;
        float moveDuration = distance / SideMoveSpeed; // �̵��ϴµ� �ɸ��� �ð�

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            _rbody.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _rbody.MovePosition(targetPosition);

        _isMovingSide = false;
        if (!_isRevive)
        {
            GameOverEvent.RaiseEvent(_isRevive);
            yield break;
        }

        GameOverEvent.RaiseEvent(_isRevive);

        StartCoroutine(DeathGravityCoroutine());
    }

    // ���� ���� �������� �ڷ�ƾ
    private IEnumerator DeathGravityCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        bool isPuddingSide_R = _spine.GetCurAnimation() == Define.CatAnimationName.A7_game_overR.ToString() ? true : false;
        _spine.ChangeAnimation(isPuddingSide_R ? Define.CatAnimationName.A8_game_downR.ToString() : Define.CatAnimationName.A8_game_downL.ToString(), 0, false);


        SetGravity(GravityScale * 0.01f);

        yield return new WaitForSeconds(3f);
        //SetGravity(initialGravityScale);
    }
    #endregion




    #region Event

    // PuddingData Event - TopEnterEvent
    public void OnEnterTopEventStarted(PuddingData data)
    {
        switch (data.type)
        {
            case PuddingType.Normal:
                EnterPuddingTop(data.type);
                break;
            case PuddingType.Feather:
                EnterFeather().Forget();
                break;
            case PuddingType.Shield:
                GetShield(data.stairs);
                break;
            case PuddingType.Skin:
                EnterPuddingTop(data.type);
                break;
        }

    }

    // Ǫ�� �𼭸� �̺�Ʈ 
    private void OnEnterTopCliffEvenStarted(bool isCliff_R)
    {
        _isCliff = true;
        _isCliff_R = isCliff_R;
    }


    // Event - TouchCatJumpEvent
    public void OnTouchCatJumpEventStarted()
    {
        if (_isJumping || !_isGamePlaying) return;
        _jumpInput = true;
    }

    // Ǫ�� ���� �̺�Ʈ
    public void OnEnterSideEventStarted(bool isPuddingSide_R)
    {
        if (_shieldCount != 0)
        {
            _shieldCount--;
            EnterShieldSideEvent.RaiseEvent(true);
            if (_shieldCount == 0) _shield.gameObject.SetActive(false);
            Device_Manager.Instance.Sound.PlayClip(SoundClipName.Crash_Bubble, 1, false);
            return;
        }
        else EnterShieldSideEvent.RaiseEvent(false);
        Device_Manager.Instance.Sound.PlayClip(SoundClipName.Crash_Gameover, 1, false);
        EnterPuddingSide(isPuddingSide_R);
    }

    // ���� ���� �̺�Ʈ
    private void OnStartBtnEventClicked()
    {
        RestartAnimation();
        if (Data_Manager.Instance.RewardShield)
        {
            GetShield(1);
            Data_Manager.Instance.SetRewardShield(false);
        }
    }

    // ��Ȱ �̺�Ʈ
    private void OnReviveEventStarted(bool isReviveBtnClicked)
    {
        if (!isReviveBtnClicked)
        {
            GameOverEvent.RaiseEvent(true);
            StartCoroutine(DeathGravityCoroutine());
            return;
        }

        _collider.enabled = true;
        _isRevive = true;
        _isDead = false;
        _isJumping = true;

        SetGravity(GravityScale);

        // ��Ȱ �� 3�� �ð� �� �ƽ��ƽ� �ִϸ��̼� ���
        Util.Delay(0.1f, () => { _spine.ChangeAnimation(Define.CatAnimationName.A3_game1_loop.ToString(), 0, false); });
        
        // ��Ȱ 3�� ���� ����ŸƮ
        Util.Delay(3f, () => RestartAnimation());
    }

    // ����� ��Ų ����
    private void OnCatSetSkinEventStarted(int num)
    {
        switch (num)
        {
            case (int)Define.CatSkinName.Cat00:
                _spine.ChangeSkin(Define.CatSkinName.Cat00.ToString());
                break;
            case (int)Define.CatSkinName.Cat01:
                _spine.ChangeSkin(Define.CatSkinName.Cat01.ToString());
                break;
            case (int)Define.CatSkinName.Cat02:
                _spine.ChangeSkin(Define.CatSkinName.Cat02.ToString());
                break;

        }

        Data_Manager.Instance.SetSkinCat(num);
        Data_Manager.Instance.Save();
    }
    #endregion
}
