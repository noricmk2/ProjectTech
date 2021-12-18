using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.TreeViewExamples
{
	internal class MultiColumnTreeView : TreeViewWithTreeModel<BehaviorTreeElement>
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;
		public bool showControls = true;

		static Texture2D[] s_TestIcons =
		{
			EditorGUIUtility.FindTexture ("Folder Icon"),
			EditorGUIUtility.FindTexture ("AudioSource Icon"),
			EditorGUIUtility.FindTexture ("Camera Icon"),
			EditorGUIUtility.FindTexture ("Windzone Icon"),
			EditorGUIUtility.FindTexture ("GameObject Icon")

		};

		// All columns
		enum BehaviorTreeColumns
		{
			NodeType,
			NodeName,
			ParentName,
			Level,
			Order,
			ActionName,
			AddBtn,
			RemoveBtn,
		}

		public enum SortOption
		{
			Level,
			Order,
		}

		// Sort options per column
		SortOption[] m_SortOptions = 
		{
			SortOption.Level, 
			SortOption.Order,
		};

		public static void TreeToList (TreeViewItem root, IList<TreeViewItem> result)
		{
			if (root == null)
				throw new NullReferenceException("root");
			if (result == null)
				throw new NullReferenceException("result");

			result.Clear();
	
			if (root.children == null)
				return;

			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			for (int i = root.children.Count - 1; i >= 0; i--)
				stack.Push(root.children[i]);

			while (stack.Count > 0)
			{
				TreeViewItem current = stack.Pop();
				result.Add(current);

				if (current.hasChildren && current.children[0] != null)
				{
					for (int i = current.children.Count - 1; i >= 0; i--)
					{
						stack.Push(current.children[i]);
					}
				}
			}
		}

		public MultiColumnTreeView (TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<BehaviorTreeElement> model) : base (state, multicolumnHeader, model)
		{
			//Assert.AreEqual(m_SortOptions.Length , Enum.GetValues(typeof(BehaviorTreeColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

			// Custom setup
			rowHeight = kRowHeights;
			columnIndexForTreeFoldouts = 0;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;
			//multicolumnHeader.sortingChanged += OnSortingChanged;
			
			Reload();
		}

		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows (root);
			return rows;
		}
		
		IOrderedEnumerable<TreeViewItem<BehaviorTreeElement>> InitialOrder(IEnumerable<TreeViewItem<BehaviorTreeElement>> myTypes, int[] history)
		{
			SortOption sortOption = m_SortOptions[history[0]];
			bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
			switch (sortOption)
			{
				case SortOption.Level:
					return myTypes.Order(l => l.data.Level, ascending);
				case SortOption.Order:
					return myTypes.Order(l => l.data.Order, ascending);
				default:
					Assert.IsTrue(false, "Unhandled enum");
					break;
			}

			// default
			return myTypes.Order(l => l.data.name, ascending);
		}

		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem<BehaviorTreeElement>) args.item;

			for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (BehaviorTreeColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI (Rect cellRect, TreeViewItem<BehaviorTreeElement> item, BehaviorTreeColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case BehaviorTreeColumns.NodeType:
				{
					Rect popupRect = cellRect;
					popupRect.x += GetContentIndent(item);
					popupRect.width = 180;
					item.data.NodeType = (BehaviorNodeType)EditorGUI.EnumPopup(popupRect, item.data.NodeType);
				}
					break;
				case BehaviorTreeColumns.Level:
				{
					cellRect.xMin += 5f;
					item.data.Level = EditorGUI.IntField(cellRect, item.data.Level);
				}
					break;
				case BehaviorTreeColumns.Order:
				{
					cellRect.xMin += 5f;
					item.data.Order = EditorGUI.IntField(cellRect, item.data.Order);
				}
					break;
				case BehaviorTreeColumns.NodeName:
				case BehaviorTreeColumns.ParentName:
				case BehaviorTreeColumns.ActionName:
					{
						cellRect.xMin += 5f; // When showing controls make some extra spacing
						if (column == BehaviorTreeColumns.ActionName)
							item.data.ActionName = GUI.TextField(cellRect, item.data.ActionName);
						if (column == BehaviorTreeColumns.NodeName)
							item.data.NodeName = GUI.TextField(cellRect, item.data.NodeName);
						if (column == BehaviorTreeColumns.ParentName)
							item.data.ParentName = GUI.TextField(cellRect, item.data.ParentName);
					}
					break;
				case BehaviorTreeColumns.AddBtn:
				{
					cellRect.xMin += 5f; // When showing controls make some extr
					if (GUI.Button(cellRect, "Add"))
					{
						TreeElement parent = item.data;
						int depth = parent.depth + 1;
						int id = treeModel.GenerateUniqueID();
						int order = 0;
						if (parent.hasChildren)
							order = parent.children.Count;
						var element = new BehaviorTreeElement (BehaviorNodeType.SelectorNode, String.Empty, false, parent.name, depth, order, String.Empty, id);
						treeModel.AddElement(element, parent, order);
					}
				}
					break;
				case BehaviorTreeColumns.RemoveBtn:
				{
					cellRect.xMin += 5f; // When showing controls make some extr
					if (GUI.Button(cellRect, "Remove"))
					{
						var list = new List<BehaviorTreeElement>();
						list.Add(item.data);
						treeModel.RemoveElements(list);
					}
				}
					break;
			}
		}

		// Rename
		//--------

		protected override bool CanRename(TreeViewItem item)
		{
			// Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
			Rect renameRect = GetRenameRect (treeViewRect, 0, item);
			return renameRect.width > 30;
		}

		protected override void RenameEnded(RenameEndedArgs args)
		{
			// Set the backend name and reload the tree to reflect the new model
			if (args.acceptedRename)
			{
				var element = treeModel.Find(args.itemID);
				element.name = args.newName;
				Reload();
			}
		}

		protected override Rect GetRenameRect (Rect rowRect, int row, TreeViewItem item)
		{
			Rect cellRect = GetCellRectForTreeFoldouts (rowRect);
			CenterRectUsingSingleLineHeight(ref cellRect);
			return base.GetRenameRect (cellRect, row, item);
		}

		// Misc
		//--------

		protected override bool CanMultiSelect (TreeViewItem item)
		{
			return true;
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
		{
			var columns = new[] 
			{
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("NodeType"),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Right,
					width = 200, 
					minWidth = 200,
					autoResize = false,
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("NodeName"),
					headerTextAlignment = TextAlignment.Center,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Right,
					width = 180, 
					minWidth = 180,
					autoResize = false,
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("RootName"),
					headerTextAlignment = TextAlignment.Right,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 180,
					minWidth = 180,
					autoResize = false
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Depth"),
					headerTextAlignment = TextAlignment.Right,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 180,
					minWidth = 180,
					autoResize = false
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("Order"),
					headerTextAlignment = TextAlignment.Right,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 180,
					minWidth = 180,
					autoResize = false
				},
				new MultiColumnHeaderState.Column 
				{
					headerContent = new GUIContent("ActionName"),
					headerTextAlignment = TextAlignment.Right,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 180,
					minWidth = 180,
					autoResize = false
				},
				new MultiColumnHeaderState.Column()
				{
					headerTextAlignment = TextAlignment.Right,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 50,
					minWidth = 50,
					autoResize = false
				},
				new MultiColumnHeaderState.Column()
				{
					headerTextAlignment = TextAlignment.Right,
					sortedAscending = true,
					sortingArrowAlignment = TextAlignment.Left,
					width = 100,
					minWidth = 50,
					autoResize = false
				}
			};

			Assert.AreEqual(columns.Length, Enum.GetValues(typeof(BehaviorTreeColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

			var state =  new MultiColumnHeaderState(columns);
			return state;
		}
	}

	static class MyExtensionMethods
	{
		public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.OrderBy(selector);
			}
			else
			{
				return source.OrderByDescending(selector);
			}
		}

		public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.ThenBy(selector);
			}
			else
			{
				return source.ThenByDescending(selector);
			}
		}
	}
}
