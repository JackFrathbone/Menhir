using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueTopicsNode : DialogueBaseNode
{
	[Input] public int entry;
	[Output(dynamicPortList = true)] public List<Topic> topics = new();


#if UNITY_EDITOR
	private void OnValidate()
    {
		foreach (Topic topic in topics)
		{
			string sCheck = topic.topicTitle.Replace(" ", "");

			if (!topic.uniqueID.Contains(sCheck))
            {
				topic.uniqueID = sCheck + (UnityEngine.Random.Range(0, 9999)).ToString();
			}
		}
	}
#endif

	[System.Serializable]
	public class Topic
	{
		[TextArea(0,1)]
		public string topicTitle;

		//Setting
		public bool topicRunOnce;
		//Check the global list to see if a check is active
		public List<StateCheck> topicStateChecks = new();
		//Check the global list to see if a check is specifically inactive
		public List<StateCheck> topicStateChecksInactive = new();
		//Check players inventory for specific item
		public List<Item> topicRequiredItems = new();
		//Checks ability
		public Abilities topicAbilityChecks;

		//ID for saving
		[ReadOnly] public string uniqueID;
	}

	public override string GetNodeType()
	{
		return "topics";
	}
}