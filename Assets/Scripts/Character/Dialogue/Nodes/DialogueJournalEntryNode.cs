using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueJournalEntryNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	public JournalEntry entryToAdd = new();

	public override string GetNodeType()
	{
		return "journalEntry";
	}
}
