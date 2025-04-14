using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDoor : MonoBehaviour
{
    //The scene door teleports the player within a scene

    [Header("Settings")]
    [SerializeField, Tooltip("Locked doors can't be interacted with")] bool _isLocked;
    [SerializeField,Tooltip("The transform to teleport the player to")] Transform _exitLocation;

    public bool GetLockedStatus()
    {
        return _isLocked;
    }

    public void ActivateSceneDoor(GameObject player)
    {
        player.transform.position = _exitLocation.position;
    }
}
