using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueLoadLevelNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	//Loads the level by name
	public int sceneIndex;

	public override string GetNodeType()
	{
		return "loadLevel";
	}
}