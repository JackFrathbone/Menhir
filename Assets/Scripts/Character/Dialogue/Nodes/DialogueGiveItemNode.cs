using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueGiveItemNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	//For giving items to the player
	public Item itemToGive;

	public override string GetNodeType()
	{
		return "giveObject";
	}
}