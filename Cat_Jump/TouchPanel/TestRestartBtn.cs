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
        // 원하는 부분 원하시는 씬으로 조건 넣어서 관리하시면 됩니다.
        // ex) 특정 레벨부터는 _gameScene아닐 경우 _lobbyScene
        await _sequenceEventChannel.RaiseEvent(_gameScene, false);
        //Spawn_Manager.Instance.ResetChance();
    }

    #endregion
}
