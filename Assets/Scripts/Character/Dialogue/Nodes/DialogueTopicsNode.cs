using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueTopicsNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output(dynamicPortList = true)] public List<Topic> topics = new();

	[System.Serializable]
	public class Topic
	{
		[TextArea(0,1)]
		public string topicTitle;

		//Setting
		public bool topicRunOnce;
		//Check the global list to see if a check is active
		public List<StateCheck> topicStateChecks = new();
		//Check players inventory for specific item
		public List<Item> topicRequiredItems = new();
		//Checks ability
		public Abilities topicAbilityChecks;
	}

	public override string GetNodeType()
	{
		return "topics";
	}
}