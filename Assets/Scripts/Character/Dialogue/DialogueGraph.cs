using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Dialogue/New Graph", order = 0)]
public class DialogueGraph : NodeGraph {
    [HideInInspector] public DialogueBaseNode current;
}