using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueEntryNode : DialogueBaseNode
{
    [Header("Ports")]
	[Output] public int exitAlive;
	[Output] public int exitWounded;

    public override string GetNodeType()
    {
        return "entry";
    }
}