using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/StateCheck")]
public class StateCheck : ScriptableObject
{
    //Just used to keep track of what this state check is for
    [TextArea(3,15)]
    public string checkDescription;
}
