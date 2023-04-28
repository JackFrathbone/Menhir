using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateAction System/New Scene Data")]
//Holds all the data for the checks and actions for the StateActionSceneController to use
public class StateActionSceneData : ScriptableObject
{
    //All the things to check and run on scene start
    public List<StateActionSceneContainer> stateActionSceneContainers = new();
}
