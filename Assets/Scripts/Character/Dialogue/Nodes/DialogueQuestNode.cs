using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueQuestNode : DialogueBaseNode
{
	//Add a quest to the player
	public Quest questToAdd;
	//Enable a specific quest entry by id number
	public int questEntry;

	public override string GetNodeType()
	{
		return "quest";
	}
}
