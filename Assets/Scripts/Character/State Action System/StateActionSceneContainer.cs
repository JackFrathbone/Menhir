using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just holds the requirements and actions for a scene action controller
[System.Serializable]
public class StateActionSceneContainer
{
    [Header("Checks")]
    [Tooltip("Run if these statechecks are all active")]
    public List<StateCheck> stateChecks;
    [Tooltip("and if these are inactive")]
    public List<StateCheck> stateChecksInactive;

    [Header("Outcome")]
    [Tooltip("All the actions to be run if the stateCheck requirements are met")]
    public List<Action> actions;
}
