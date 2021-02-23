using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CustomTileMap))]
public class CustomTileMapEditor : Editor
{
    int _lastHandleID = -1;

    public CustomTileMap _tileMap;
    public class SceneViewEditorWindow
    {
        private CustomTileMap _tileMap;
        public enum EAnchor
        {
            L_Top = 0,
            R_Top,
            L_Bottom,
            R_Bottom
        }

        public static SceneViewEditorWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SceneViewEditorWindow();
                }
                return _instance;
            }
        }

        public CustomTileMap TileMap { get => _tileMap; set => _tileMap = value; }

        public static SceneViewEditorWindow _instance;

        public static Vector2 _scroll;

        private static Vector2 _pos = new Vector2(20, 20);
        private static Vector2 _size = new Vector2(250, 200);
        private static int _anchor = 0;

        public void Init(float x, float y, float width, float height, int anchor = 0)
        {
            _pos = new Vector2(x, y);
            _size = new Vector2(width, height);
            _anchor = anchor;

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Exit()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        void OnSceneGUI(SceneView scn)
        {
            HandleEvent(scn);
        }

        void HandleEvent(SceneView scn)
        {
            if (Event.current.type != EventType.Repaint)
            {
                int controlId = GUIUtility.GetControlID(FocusType.Passive);
                GUILayout.Window(controlId, CreateRect(scn, _pos, _size, _anchor, _tileMap), OnDisplay, "Editor");

                int id = GUIUtility.GetControlID("CustomTileMapEditor".GetHashCode(), FocusType.Passive);

                switch (Event.current.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        break;
                    case EventType.MouseUp:
                        CreateTileOnClick();
                        break;
                    case EventType.MouseMove:
                        break;
                    case EventType.MouseDrag:
                        break;
                    case EventType.KeyDown:
                        break;
                    case EventType.KeyUp:
                        break;

                }

            }
        }

        void CreateTileOnClick()
        {
            var p = new Plane(_tileMap.transform.TransformDirection(Vector3.forward), Vector3.zero);
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;
            Vector3 hitPos = Vector3.zero;
            var dist = 0f;
            if (p.Raycast(worldRay, out dist))
            {
                hitPos = worldRay.origin + worldRay.direction.normalized * dist;
                Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                GameObject obj = new GameObject("tile");
                obj.transform.position = _tileMap.transform.InverseTransformPoint(pos);
                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            }
        }

        static void OnDisplay(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            EditorGUILayout.LabelField("Label");

            if (GUILayout.Button("Button"))
            {
            }

            GUILayout.EndScrollView();
        }

        private Rect CreateRect(SceneView scn, Vector2 pos, Vector2 size, int anchor, CustomTileMap customTileMap)
        {
            Vector2 rc = new Vector2(pos.x, pos.y);
            if (scn != null)
            {
                switch ((EAnchor)anchor)
                {
                    case EAnchor.L_Top: rc = new Vector2(pos.x, pos.y); break;
                    case EAnchor.R_Top: rc = new Vector2(scn.camera.pixelRect.width - pos.x, pos.y); break;
                    case EAnchor.L_Bottom: rc = new Vector2(pos.x, scn.camera.pixelRect.height - pos.y); break;
                    case EAnchor.R_Bottom: rc = new Vector2(scn.camera.pixelRect.width - pos.x, scn.camera.pixelRect.height - pos.y); break;
                }
            }
            _tileMap = customTileMap;
            return new Rect(rc.x, rc.y, size.x, size.y);
        }
    }

    private void OnEnable()
    {
        SceneViewEditorWindow.Instance.TileMap = (CustomTileMap)target;
        SceneViewEditorWindow.Instance.Init(20, 20, 250, 250,0);
    }
}
