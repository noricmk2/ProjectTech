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

        private Vector2 _pos = new Vector2(20, 20);
        private Vector2 _size = new Vector2(1200, 900);
        private Vector2 _hideSize = new Vector2(10, 10);
        private Vector2 _tileSize = new Vector2(50, 50);
        private int _anchor = 0;
        private int _bottomBaseOffset = 50;
        private float _bottomOffset = -1;
        private List<Texture2D> _spriteLists = new List<Texture2D>();
        private List<GameObject> _tileList = new List<GameObject>();
        private Texture2D _selectTexture;
        private int _gridHei = 0;
        private string _spritesPath = ""; 
        private readonly string _spritePathPlayerPrefsKey = "SpritePath";
        bool _isHide = false;
        public void Init(float x, float y, float width, float height, int anchor = 0)
        {
            _spritesPath = PlayerPrefs.GetString(_spritePathPlayerPrefsKey, Application.dataPath + "/Resources/TileImages/Test");
            _pos = new Vector2(x, y);
            _size = new Vector2(width, height);
            _anchor = anchor;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            _bottomOffset = ((_spriteLists.Count * _tileSize.x) / _size.x) * (_tileSize.x*2) + _bottomBaseOffset;

            SetSpriteList();
            SelectTile(_spriteLists[0]);
        }

        private void SetSpriteList()
        {
            string resourceSubPath = "/Resources/";
            int startIdx = _spritesPath.IndexOf(resourceSubPath) + resourceSubPath.Length;
            string subPath = _spritesPath.Substring(startIdx);
            var allSprites = Resources.LoadAll(subPath);

            _spriteLists.Clear();
            foreach (Texture2D item in allSprites)
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
            if (p.Raycast(worldRay, out dist) && _selectTexture != null)
            {
                float tileWid = _selectTexture.width;
                float tileHei = _selectTexture.height;

                var tileSize = tileWid / 100;

                hitPos = worldRay.origin + worldRay.direction.normalized * dist;

                var invHitPos = _tileMap.transform.InverseTransformPoint(hitPos);

                Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                GameObject obj = new GameObject("tile");
                obj.transform.SetParent(_tileMap.transform);

                var x = Mathf.Floor(invHitPos.x / tileSize) * tileSize;
                var y = Mathf.Floor(invHitPos.y / tileSize) * tileSize;

                var row = x / tileSize;
                var column = Mathf.Abs(y / tileSize) - 1;

                x += _tileMap.transform.position.x + tileSize / 2;
                y += _tileMap.transform.position.y + tileSize / 2;
               
                obj.transform.position = new Vector3(x, y, _tileMap.transform.position.z);

                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
                Rect rect = new Rect(0, 0, tileWid, tileHei);
                sr.sprite = Sprite.Create(_selectTexture, rect, new Vector2(0.5f, 0.5f));

                _tileList.Add(obj);
            }
        }

         void OnDisplay(int id)
        {
            _isHide = GUILayout.Toggle(_isHide,"Hide");
            GUILayout.Label("CurSpritePath : " + _spritesPath);
         
            _scroll = GUILayout.BeginScrollView(_scroll);

            if (GUILayout.Button("Set Sprite Folder Directory(스프라이트 폴더 경로 설정)"))
            {
                string  setPath = EditorUtility.OpenFolderPanel("Set Sprite Folder", _spritesPath, "");
                if (string.IsNullOrEmpty(setPath) == false)
                {
                    PlayerPrefs.SetString(_spritePathPlayerPrefsKey, setPath);
                    _spritesPath = setPath;
                }
            }

            if (GUI.Button(new Rect(100, _bottomOffset,100,50),"Clear Tiles"))
            { 
                foreach(GameObject child in _tileList)
                {
                    DestroyImmediate(child.gameObject);
                }

                _tileList.Clear();
            }

            if (GUILayout.Button("Open Sprite Folder (스프라이트 폴더 오픈)"))
            {
                Application.OpenURL(_spritesPath);
            }


            DisplayAllSprites();
            DisplayCurrentSelect();
            GUILayout.EndScrollView();
        }

        void DisplayAllSprites()
        {
            var offset = new Vector2(10, 50);

            int wid = 0;
            int hei = 0;
            int preHei = 0;

            int i = 0;
           
            foreach (Texture2D texture2D in _spriteLists)
            {
                hei = (int)((i * _tileSize.x) / _size.x);
                if (preHei != hei)
                    wid = 0;

                float xPos = offset.x + (_tileSize.x * wid);

                preHei = hei;

                var textureRect = new Rect(xPos, offset.y + (hei * _tileSize.y), _tileSize.x, _tileSize.y);

                GUI.color = new Color(1, 1, 1, 1f);
                GUI.DrawTexture(textureRect, texture2D);
                GUI.color = new Color(1, 1, 1, 0f);
                if (GUI.Button(textureRect, ""))
                {
                    SelectTile(texture2D);
                }
                wid++;
                i++;
            }
        }
        private void DisplayCurrentSelect()
        {
            if (_selectTexture == null)
                return;
            GUI.color = new Color(1, 1, 1, 1f);
            GUI.TextArea(new Rect(0, (float)_bottomOffset, 60,100),"Selected Texture");
            if (_selectTexture != null)
            {
               GUI.DrawTexture(new Rect(0, _bottomOffset + _bottomBaseOffset, _tileSize.x, _tileSize.y), _selectTexture);
            }
        }

        private void SelectTile(Texture2D texture)
        {
            _selectTexture = texture;
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
    private void OnDisable()
    {
        SceneViewEditorWindow.Instance.Exit();
    }
    private void OnEnable()
    {
        SceneViewEditorWindow.Instance.TileMap = (CustomTileMap)target;
        SceneViewEditorWindow.Instance.Init(20, 20, 500, 500,0);
    }
}
