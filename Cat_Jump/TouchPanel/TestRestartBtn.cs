using Scripts.Framework.Scenes.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRestartBtn : MonoBehaviour
{
    #region Fields

    [Header("Broadcasting on")]
    [SerializeField] private RequestSequenceEventChannelSO _sequenceEventChannel;

    [Header("You need Scenes")]
    [SerializeField] private GameSceneSO _gameScene;

    #endregion



    #region Unity Behavior

    public async void ReStartScene()
    {
        // ���ϴ� �κ� ���Ͻô� ������ ���� �־ �����Ͻø� �˴ϴ�.
        // ex) Ư�� �������ʹ� _gameScene�ƴ� ��� _lobbyScene
        await _sequenceEventChannel.RaiseEvent(_gameScene, false);
        //Spawn_Manager.Instance.ResetChance();
    }

    #endregion
}
