using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueBaseNode : Node {

	public virtual string GetNodeType()
    {
		return "base";
    }

	public override object GetValue(NodePort port) {
		return null;
	}
}