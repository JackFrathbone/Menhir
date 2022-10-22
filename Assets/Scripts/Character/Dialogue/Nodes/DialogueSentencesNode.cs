using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueSentencesNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	[TextArea(0, 3)] public List<string> sentences = new();

	public override string GetNodeType()
	{
		return "sentences";
	}
}