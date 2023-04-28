using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueStateActionNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	public List<Action> actionToRun = new();

	public override string GetNodeType()
	{
		return "action";
	}
}