using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueQuestNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	//Add a quest to the player
	public Quest questToAdd;
	//Enable a specific quest entry by id number
	public List<int> questEntries;

	public override string GetNodeType()
	{
		return "quest";
	}
}
