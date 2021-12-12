using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace UnityEditor.TreeViewExamples
{

	class MultiColumnWindow : EditorWindow
	{
		[NonSerialized] bool m_Initialized;
		[SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
		[SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
		SearchField m_SearchField;
		MultiColumnTreeView m_TreeView;
		BehaviorTreeAsset _behaviorTreeAsset;
		List<BehaviorTreeElement> _editorTreeList = new List<BehaviorTreeElement>();
		int _allowDepth = 0;
		string _assetName = String.Empty;

		[MenuItem("ProjectTech/BehaviorTreeEditor")]
		public static MultiColumnWindow GetWindow ()
		{
			var window = GetWindow<MultiColumnWindow>();
			window.titleContent = new GUIContent("Tree Editor");
			window.Focus();
			window.Repaint();
			return window;
		}

		[OnOpenAsset]
		public static bool OnOpenAsset (int instanceID, int line)
		{
			var myTreeAsset = EditorUtility.InstanceIDToObject (instanceID) as BehaviorTreeAsset;
			if (myTreeAsset != null)
			{
				var window = GetWindow ();
				window.SetTreeAsset(myTreeAsset);
				return true;
			}
			return false; // we did not handle the open
		}

		void SetTreeAsset (BehaviorTreeAsset behaviorTreeAsset)
		{
			_behaviorTreeAsset = behaviorTreeAsset;
			m_Initialized = false;
		}

		Rect multiColumnTreeViewRect
		{
			get { return new Rect(20, 30, position.width-40, position.height-60); }
		}

		Rect toolbarRect
		{
			get { return new Rect (20f, 10f, position.width-40f, 20f); }
		}

		Rect bottomToolbarRect
		{
			get { return new Rect(20f, position.height - 18f, position.width - 40f, 16f); }
		}

		public MultiColumnTreeView treeView
		{
			get { return m_TreeView; }
		}

		void InitIfNeeded ()
		{
			if (!m_Initialized)
			{
				// Check if it already exists (deserialized from window layout file or scriptable object)
				if (m_TreeViewState == null)
					m_TreeViewState = new TreeViewState();

				bool firstInit = m_MultiColumnHeaderState == null;
				var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
				if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
					MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
				m_MultiColumnHeaderState = headerState;
				
				var multiColumnHeader = new MyMultiColumnHeader(headerState);
				if (firstInit)
					multiColumnHeader.ResizeToFit ();

				var treeModel = new TreeModel<BehaviorTreeElement>(GetData());
				
				m_TreeView = new MultiColumnTreeView(m_TreeViewState, multiColumnHeader, treeModel);

				m_SearchField = new SearchField();
				m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

				m_Initialized = true;
			}
		}
		
		IList<BehaviorTreeElement> GetData ()
		{
			if (_behaviorTreeAsset != null && _behaviorTreeAsset.treeElements != null && _behaviorTreeAsset.treeElements.Count > 0)
				return _behaviorTreeAsset.treeElements;
			else
			{
				if (_editorTreeList.Count == 0)
				{
					var root = new BehaviorTreeElement(BehaviorNodeType.SelectorNode, "Root", true, String.Empty, -1, 0, String.Empty, 0);
					_editorTreeList.Add(root);
					MyTreeElementGenerator.AddChildrenRecursive(root, 1, true, _editorTreeList.Count + 1, ref _allowDepth, _editorTreeList);
				}

				return _editorTreeList;
			}
		}

		public void AddNewElement(BehaviorTreeElement parent)
		{
			MyTreeElementGenerator.AddChildrenRecursive(parent, 1, true, _editorTreeList.Count + 1, ref _allowDepth, _editorTreeList);
		}

		void OnSelectionChange ()
		{
			if (!m_Initialized)
				return;

			var myTreeAsset = Selection.activeObject as BehaviorTreeAsset;
			if (myTreeAsset != null && myTreeAsset != _behaviorTreeAsset)
			{
				_behaviorTreeAsset = myTreeAsset;
				m_TreeView.treeModel.SetData (GetData ());
				m_TreeView.Reload ();
			}
		}

		void OnGUI ()
		{
			InitIfNeeded();

			SearchBar (toolbarRect);
			DoTreeView (multiColumnTreeViewRect);
			BottomToolBar (bottomToolbarRect);
		}

		void SearchBar (Rect rect)
		{
			treeView.searchString = m_SearchField.OnGUI (rect, treeView.searchString);
		}

		void DoTreeView (Rect rect)
		{
			m_TreeView.OnGUI(rect);
		}

		void BottomToolBar (Rect rect)
		{
			GUILayout.BeginArea (rect);

			using (new EditorGUILayout.HorizontalScope ())
			{
				var style = "miniButton";
				if (GUILayout.Button("Expand All", style))
				{
					treeView.ExpandAll ();
				}

				if (GUILayout.Button("Collapse All", style))
				{
					treeView.CollapseAll ();
				}
				
				if(GUILayout.Button("Save", style))
				{
					SaveCurrent();
				}

				_assetName = GUILayout.TextField(_assetName);
			}

			GUILayout.EndArea();
		}

		private readonly string savePath = "Assets/LocalResource/DataAsset/";
		private readonly string extension = ".asset";
		
		private void SaveCurrent()
		{
			var saveData = new AIRawData();
			saveData.treeDataList = new List<BehaviorTreeData>();
			var treeModel = treeView.treeModel;
			
			TreeElementUtility.TreeToList(treeModel.root, _editorTreeList);
			
			for (int i = 0; i < _editorTreeList.Count; ++i)
			{
				var data = new BehaviorTreeData();
				data.Level = _editorTreeList[i].Level;
				data.Order = _editorTreeList[i].Order;
				data.ActionName = _editorTreeList[i].ActionName;
				data.IsRoot = _editorTreeList[i].IsRoot;
				data.NodeName = _editorTreeList[i].NodeName;
				data.ParentName = _editorTreeList[i].ParentName;
				data.NodeType = _editorTreeList[i].NodeType;
				saveData.treeDataList.Add(data);
			}
			AssetDatabase.CreateAsset(saveData, string.Concat(savePath, _assetName, extension));
			AssetDatabase.SaveAssets();
		}
	}


	internal class MyMultiColumnHeader : MultiColumnHeader
	{
		Mode m_Mode;

		public enum Mode
		{
			LargeHeader,
			DefaultHeader,
			MinimumHeaderWithoutSorting
		}

		public MyMultiColumnHeader(MultiColumnHeaderState state)
			: base(state)
		{
			mode = Mode.DefaultHeader;
		}

		public Mode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
				switch (m_Mode)
				{
					case Mode.LargeHeader:
						canSort = true;
						height = 37f;
						break;
					case Mode.DefaultHeader:
						canSort = true;
						height = DefaultGUI.defaultHeight;
						break;
					case Mode.MinimumHeaderWithoutSorting:
						canSort = false;
						height = DefaultGUI.minimumHeight;
						break;
				}
			}
		}

		protected override void ColumnHeaderGUI (MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
		{
			// Default column header gui
			base.ColumnHeaderGUI(column, headerRect, columnIndex);

			// Add additional info for large header
			if (mode == Mode.LargeHeader)
			{
				// Show example overlay stuff on some of the columns
				if (columnIndex > 2)
				{
					headerRect.xMax -= 3f;
					var oldAlignment = EditorStyles.largeLabel.alignment;
					EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
					GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
					EditorStyles.largeLabel.alignment = oldAlignment;
				}
			}
		}
	}

}
