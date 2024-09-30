using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameEndCamera : MonoBehaviour
{
    [SerializeField] private AssetReference _gameEndCameraGroupAsset;

    //CinemachineTargetGroup


    //public void Initialized()
    //{
    //    CinemachineTargetGroup targetGroup = Instantiate(_gameEndCameraGroupAsset, transform).GetComponent<CinemachineTargetGroup>();
    //    GameObject obj = new GameObject();
    //    obj.name = "InitPosition";
    //    obj.transform.parent = transform;
    //    targetGroup.AddMember(obj.transform, 1, 0);

    //    GameObject obj2 = new GameObject();
    //    obj2.name = "EndPosition";
    //    obj2.transform.parent = transform;
    //    obj2.transform.position = new Vector3(0, 60, 0);
    //    targetGroup.AddMember(obj2.transform, 1, 0);

    //    GameObject _gameEndCameraObject = Instantiate(_gameEndCameraAsset, transform);
    //    _gameEndCamera = _gameEndCameraObject.GetComponent<CinemachineVirtualCamera>();
    //    _gameEndCamera.Priority = 9;
    //    _gameEndCamera.Follow = targetGroup.transform;
    //    _gameEndCamera.LookAt = targetGroup.transform;

    //    targetGroup.AddMember(cameraLookObject.transform, 1, 0);
    //}
}
