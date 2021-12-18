using UnityEditor;
using UnityEngine;

/// <summary>
/// This window shows how you can listen for and consume user input events
/// from the Scene View. Super useful for making editor tools!
/// </summary>
public class SceneViewControlWindow : EditorWindow
{
	/// <summary>
	/// Open the window
	/// </summary>
	[MenuItem("Window/Scene View Control")]
	static void Open()
	{
		SceneViewControlWindow win = GetWindow<SceneViewControlWindow>();
		win.titleContent = new GUIContent("Scene View Control");
		win.Show();
	}

	/// <summary>
	/// When we open the window, subscribe to the SceneView's input event
	/// </summary>
	void OnEnable()
	{
		SceneView.duringSceneGui += OnSceneGUI;
	}

	/// <summary>
	/// When we close, unsubscribe from the SceneView
	/// </summary>
	void OnDisable()
	{
		SceneView.duringSceneGui -= OnSceneGUI;
	}

	void OnGUI()
	{
		EditorGUILayout.HelpBox("While this window is open, only scene objects with Colliders can be selected in the Scene View!", MessageType.Info);
	}
	

	/// <summary>
	/// Draw or do input/handle overriding in the scene view
	/// </summary>
	void OnSceneGUI(SceneView sceneView)
	{
		// Event.current houses information on scene view input this cycle
		Event current = Event.current;

		// If user has pressed the Left Mouse Button, do something and
		// swallow it so nothing else hears the event
		if (current.type == EventType.MouseDown && current.button == 0)
		{
			// While this tool is open, only allow the user to select scene
			// objects with a Collider component on them
			if (!Select<Collider>(current))
			{
				// If nothing with Collider found, unselect everything
				Selection.activeGameObject = null;
			}
		}
		
		// After you've done all your custom event interpreting and swallowing,
		// you have to call this code to make sure swallowed events don't bleed out.
		// Not sure why, but that's the rules.
		if (current.type == EventType.Layout)
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	}


	/// <summary>
	/// When user attempts to select an object, this sees if they selected an
	/// object with the given component. This will swallow the event and select
	/// the object if successful.
	/// </summary>
	/// <param name="e">Event from OnSceneGUI</param>
	/// <typeparam name="T">Component type</typeparam>
	/// <returns>Returns the object</returns>
	public static GameObject Select<T>(Event e) where T : UnityEngine.Component
	{
		Camera cam = Camera.current;

		if (cam != null)
		{
			RaycastHit hit;
			Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null)
				{
					GameObject gameObj = hit.collider.gameObject;
					if (gameObj.GetComponent<T>() != null)
					{
						e.Use();
						UnityEditor.Selection.activeGameObject = gameObj;
						return gameObj;
					}
				}
			}
		}

		return null;
	}
}