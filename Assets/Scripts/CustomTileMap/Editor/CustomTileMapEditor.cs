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

        public  Vector2 _scroll;

        private  Vector2 _pos = new Vector2(20, 20);
        private  Vector2 _size = new Vector2(1200, 900);
        private Vector2 _hideSize = new Vector2(10, 10);
        private Vector2 _tileSize = new Vector2(50, 50);
        private int _anchor = 0;
        private string _spriteBasePath = "";
        private List<Texture2D> _spriteLists = new List<Texture2D>();
        
        bool _isHide = false;
        public void Init(float x, float y, float width, float height, int anchor = 0)
        {
            _spriteBasePath = Application.dataPath + "/Resources/TileImages/Test";
            _pos = new Vector2(x, y);
            _size = new Vector2(width, height);
            _anchor = anchor;

            SceneView.duringSceneGui += OnSceneGUI;
            string resourceSubPath = "/Resources/";
            int startIdx = _spriteBasePath.IndexOf(resourceSubPath) + resourceSubPath.Length;
            string subPath = _spriteBasePath.Substring(startIdx);
            var allSprites = Resources.LoadAll(subPath);

            _spriteLists.Clear();
            foreach(Texture2D item  in allSprites)
            {
                _spriteLists.Add(item);
            }

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
                
             
                GUILayout.Window(controlId, CreateRect(scn, _pos, _size, _anchor, _tileMap), OnDisplay, "TileMap");
                int id = GUIUtility.GetControlID("CustomTileMapEditor".GetHashCode(), FocusType.Passive);

                switch (Event.current.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        CreateTileOnClick();
                        break;
                    case EventType.MouseUp:
                        
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

         void OnDisplay(int id)
        {
            _isHide = GUILayout.Toggle(_isHide,"Hide");
            _scroll = GUILayout.BeginScrollView(_scroll);



            if (GUILayout.Button("Set Directory(경로 설정)"))
            {
                string path = EditorUtility.OpenFolderPanel("Overwrite with png", _spriteBasePath, "");

            }

            DisplayAllSprites();
            GUILayout.EndScrollView();
        }

        void DisplayAllSprites()
        {
            var offset = new Vector2(10, 25);

            int wid = 0;
            int hei = 0;
            int preHei = 0;
            for (int i = 0; i < _spriteLists.Count; i++)
            {
               
                hei =(int) ((i * _tileSize.x)  /_size.x );
                if (preHei != hei)
                    wid = 0;

                float xPos = offset.x + (_tileSize.x * wid);

                preHei = hei;
                GUI.DrawTexture(new Rect(xPos, offset.y + (hei * _tileSize.y), _tileSize.x, _tileSize.y), _spriteLists[i]);
                wid++;
            }
        }

        private Rect CreateRect(SceneView scn, Vector2 pos, Vector2 size, int anchor, CustomTileMap customTileMap)
        {
            if (_isHide == true)
                size = _hideSize;
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
        SceneViewEditorWindow.Instance.Init(20, 20, 500, 500,0);
    }
}
