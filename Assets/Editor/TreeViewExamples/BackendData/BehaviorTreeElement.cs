using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace UnityEditor.TreeViewExamples
{

	[Serializable]
	internal class BehaviorTreeElement : TreeElement
	{
		public BehaviorNodeType NodeType;
		public string NodeName;
		public bool IsRoot;
		public string ParentName;
		public int Level;
		public int Order;
		public string ActionName;
		
		public BehaviorTreeElement (BehaviorNodeType type, string name, bool isRoot, string parentName, int depth, int order, string actionName, int id) : base (name, depth, id)
		{
			NodeType = type;
			NodeName = name;
			IsRoot = isRoot;
			ParentName = parentName;
			Level = depth + 1;
			Order = order;
			ActionName = actionName;
		}
	}
}
