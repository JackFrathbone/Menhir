using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActionSceneController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] StateActionSceneData _stateActionSceneData;

    [Header("References")]
    private PlayerCharacterManager _playerCharacterManager;

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();

        if(_playerCharacterManager == null)
        {
            Debug.Log("Missing player in scene with a StateActionSceneController");
            return;
        }

        foreach (StateActionSceneContainer stateActionCheck in _stateActionSceneData.stateActionSceneContainers)
        {
            CheckStates(stateActionCheck);
        }
    }

    private void CheckStates(StateActionSceneContainer stateActionCheck)
    {
        bool runActions = true;

        //If there checks
        foreach (StateCheck stateCheck in stateActionCheck.stateChecks)
        {
            if (!_playerCharacterManager.stateChecks.Contains(stateCheck))
            {
                runActions = false;
            }
        }

        //If there are inactive checks, check if the player doesn't have
        foreach (StateCheck stateCheck in stateActionCheck.stateChecksInactive)
        {
            if (_playerCharacterManager.stateChecks.Contains(stateCheck))
            {
                runActions = false;
            }
        }

        //If all conditions are met then run the action
        if(runActions == true)
        {
            foreach(Action action in stateActionCheck.actions)
            {
                action.StartAction();
            }
        }
        else
        {
            return;
        }
    }
}
