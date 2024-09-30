using Cinemachine;
using Scripts.Framework.Events.Listeners;
using UnityEngine;

public class StateDrivenCamera : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera _catJumpSceneCamera;
    [SerializeField] private CinemachineVirtualCamera _collectCamera;

    //[SerializeField] private Animator _animator;

    [SerializeField] GameEventListener<int> _cameraAnimationEvent;

    private Transform _cameraLookObject;

    private Animator _animator;



    public void Initialized(Transform cameraLookObject)
    {
        _cameraLookObject = cameraLookObject;


        _catJumpSceneCamera.Follow = _cameraLookObject;
        _catJumpSceneCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 18.5f;

        _collectCamera.Follow = _cameraLookObject;
        _collectCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 15;


        _animator = GetComponent<CinemachineStateDrivenCamera>().m_AnimatedTarget;

        _cameraAnimationEvent.Subscribe();
        _cameraAnimationEvent.SubstitutionEvent(OnCameraAnimationEventStarted);
    }

    private void OnDisable()
    {
        _cameraAnimationEvent.Unsubscribe();
    }


    private void OnCameraAnimationEventStarted(int num)
    {
        switch (num)
        {
            case (int)Define.CameraAnimation.CatJumpSceneCamera:
                _animator.Play(Define.CameraAnimation.CatJumpSceneCamera.ToString());
                _catJumpSceneCamera.gameObject.SetActive(true);
                _collectCamera.gameObject.SetActive(false);
                break;
            case (int)Define.CameraAnimation.CollectCamera:
                _animator.Play(Define.CameraAnimation.CollectCamera.ToString());
                _catJumpSceneCamera.gameObject.SetActive(false);
                _collectCamera.gameObject.SetActive(true);
                break;
        }
    }

}
