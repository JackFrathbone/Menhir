using System.Collections;
using System.Collections.Generic;
using Udar.SceneField;
using UnityEngine;
using XNode;

public class DialogueLoadLevelNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output] public int exit;

	//Loads the level by name
	public SceneField targetScene;
	public string spawnPointName = "default";

	public override string GetNodeType()
	{
		return "loadLevel";
	}
}