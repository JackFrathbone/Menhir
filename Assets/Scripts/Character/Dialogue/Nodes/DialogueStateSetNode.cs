using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueStateSetNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	//Add a check to the global list
	public List<StateCheck> dialogueAddStateCheck = new();

	public override string GetNodeType()
	{
		return "stateSet";
	}
}