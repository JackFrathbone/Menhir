using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueQuitNode : DialogueBaseNode
{
    [Input] public int entry;

    public override string GetNodeType()
    {
        return "quit";
    }
}