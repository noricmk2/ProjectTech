using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
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
        public enum ToolModes
        {
            Transform,
            Building,
            Painting
        }

        private ToolModes toolMode = ToolModes.Building;

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
        public GameObject _prefabTest;
        public  Vector2 _scroll;

        private Vector2 _pos = new Vector2(20, 20);
        private Vector2 _size = new Vector2(1200, 900);
        private Vector2 _hideSize = new Vector2(10, 5);
        private Vector2 _tileSize = new Vector2(50, 50);
        private int _anchor = 0;
        private int _bottomBaseOffset = 50;
        private float _bottomOffset = -1;
        private List<GameObject> _tileList = new List<GameObject>();
        private List<GameObject> _prefabList = new List<GameObject>();
        private Texture2D _selectTexture;
        private GameObject _selectPrefab;
        private int _gridHei = 0;
        private string _spritesPath = ""; 
        private readonly string _spritePathPlayerPrefsKey = "SpritePath";
        private int _curIndex = 0;
        bool _isHide = false;
        #region BuildVariable
        private int _xBuildSize;
        private int _yBuildSize;
        #endregion
        
   
        public void Init(float x, float y, float width, float height, int anchor = 0)
        {
            _spritesPath = PlayerPrefs.GetString(_spritePathPlayerPrefsKey, Application.dataPath + "/Resources/TileSample");
            _pos = new Vector2(x, y);
            _size = new Vector2(width, height);
            _anchor = anchor;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            _bottomOffset = ((_prefabList.Count * _tileSize.x) / _size.x) * (_tileSize.x) + _bottomBaseOffset;

            SetSpriteList();
            SelectTile(_prefabList[0]);
        }

        private void SetSpriteList()
        {
            string resourceSubPath = "/Resources/";
            int startIdx = _spritesPath.IndexOf(resourceSubPath) + resourceSubPath.Length;
            string subPath = _spritesPath.Substring(startIdx);
            var allPrefabs = Resources.LoadAll(subPath);
            _prefabList.Clear();
            for (int i=0; i< allPrefabs.Length; i++)
            {
                GameObject gameObject = allPrefabs[i] as GameObject;
                if(gameObject != null && gameObject.layer == 8) 
                {
                    _prefabList.Add(gameObject);
                }
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

                        if (Event.current.button == 0)
                        {
                            if (toolMode == ToolModes.Painting)
                            {
                                CreateTileOnClick();
                            }
                        }
                        break;

                }
            }
        }
        
        void SetEditorViewCamera()
        {
            var scene_view = UnityEditor.SceneView.lastActiveSceneView;

            // SceneView.lastActiveSceneView.cameraDistance is private, compute it.
            var offset = scene_view.pivot - scene_view.camera.transform.position;
            var cameraDistance = offset.magnitude;

            scene_view.pivot = new Vector3(0,0) + new Vector3(0,0) * cameraDistance * -1.0f;
            scene_view.rotation = Quaternion.LookRotation(new Vector3(0,0));
        }

        void CreateTileOnClick()
        {
            var p = new Plane(_tileMap.transform.TransformDirection(Vector3.forward), Vector3.zero);
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;
            Vector3 hitPos = Vector3.zero;
            var dist = 0f;
            p.Raycast(worldRay, out dist);
            RaycastHit2D hit2d = Physics2D.Raycast(worldRay.origin, worldRay.direction);
            
            GameObject selectObject = _prefabList[_curIndex];
            if (selectObject != null)
            {
                Texture texture = selectObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
                Debug.Log("TextureNAme  :" + texture.name);
                float tileWid = (int)texture.width;
                float tileHei = (int)texture.height;

                var tileSize = 1;

                hitPos = worldRay.origin + worldRay.direction.normalized * dist;

                var invHitPos = _tileMap.transform.InverseTransformPoint(hitPos);

                Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
          
                var x = Mathf.Floor(invHitPos.x / tileSize) * tileSize;
                var y = Mathf.Floor(invHitPos.y / tileSize) * tileSize;
                var spawnPos = new Vector3(x, y, _tileMap.transform.position.z);
                if (_tileMap.GetTileCount() == 0)
                {
                    GameObject obj = GameObject.Instantiate(selectObject);
                    obj.transform.SetParent(_tileMap.transform);

                    var row = x / tileSize;
                    var column = Mathf.Abs(y / tileSize) - 1;

                    x += _tileMap.transform.position.x + tileSize / 2;
                    y += _tileMap.transform.position.y + tileSize / 2;

                    obj.transform.position = spawnPos;

                    _tileMap.RegisterTileItem(Vector2Int.zero, obj);
                    CreateChildNodes(obj);
                    _tileList.Add(obj);
                }
                else
                {
                    var selectObj = Selection.activeGameObject;
                    if (selectObj != null)
                    {
                        if (selectObj.GetComponent<ChildTile>() != null)
                            CreateChildNodes(selectObj);
                        else
                        {

                        }
                    }
                }
            }
        }

        void CreateMapWithSize(int x, int y)
        {
            
        }
        void CreateChildNodes(GameObject baseObj)
        {
            var meshFil = baseObj.GetComponent<MeshFilter>().sharedMesh.bounds;
            var bound = meshFil.size;
            float[] dx = { - bound.x, 0, bound.x, 0 };
            float[] dz = { 0, bound.y, 0, - bound.y };

            int[] dxTile = { -1, 0, 1, 0 };
            int[] dzTile = { 0, 1, 0, -1 };
            //bot,right,up,left
            Vector3 basePos = baseObj.transform.position;
            int curX = 0;
            int curY = 0;
            ChildTile child = baseObj.GetComponent<ChildTile>();
            if(child != null)
            {
                curX = child.x;
                curY = child.y;
                DestroyImmediate(child);
                _tileMap.ChildTiles.Remove(child);
            }
  
            EditorUtility.SetDirty(baseObj);

            _tileMap.RegisterTileItem(new Vector2Int(curX, curY), baseObj);
            for (int i=0; i < dx.Length; i++)
            {
   
                Vector3 newPos = new Vector3(basePos.x + dx[i],basePos.y, basePos.z + dz[i]);
                int xPos = curX + dxTile[i];
                int yPos = curY + dzTile[i];
                if(_tileMap.IsRegisteredTile(new Vector2Int(xPos,yPos),null) == false)
                    CreateChildNode(newPos, xPos, yPos,baseObj);
            }
        }

        void CreateChildNode(Vector3 pos,int x, int y,GameObject baseObj)
        {
            GameObject copy = GameObject.Instantiate(_selectPrefab);
            Mesh mesh = copy.GetComponent<MeshFilter>().sharedMesh ;
            ChildTile childTile = copy.AddComponent<ChildTile>();

            childTile.size = mesh.bounds.size;

            copy.transform.parent = baseObj.transform.parent;
            copy.transform.position = pos;
            copy.transform.rotation = baseObj.transform.rotation;

            childTile.mesh = mesh;
            childTile.rot = copy.transform.rotation;
            childTile.pos = copy.transform.position;
            childTile.scale = copy.transform.localScale;

            childTile.x = x;
            childTile.y = y;
            _tileMap.ChildTiles.Add(childTile);
            _tileList.Add(childTile.gameObject);
            _tileMap.RegisterTileItem(new Vector2Int(x, y), copy);
        }

        #region Display
        void GeneralDisPlay()
        {
            _isHide= GUILayout.Toggle(_isHide,"hide");
            toolMode= (ToolModes) GUILayout.Toolbar((int)toolMode, new[] {"Move", "Build", "Paint"});
        }

        void BuildDisplay()
        {
            _xBuildSize = EditorGUILayout.IntField("Build X Size",_xBuildSize);
            _yBuildSize = EditorGUILayout.IntField("Build Y Size",_yBuildSize);
        }
        void PaintModeDisplay()
        {
            EditorGUILayout.TextArea("CurSpritePath : " + _spritesPath);
        
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            if (GUILayout.Button("Set Sprite Folder Directory(스프라이트 폴더 경로 설정)"))
            {
                string  setPath = EditorUtility.OpenFolderPanel("Set Sprite Folder", _spritesPath, "");
                if (string.IsNullOrEmpty(setPath) == false)
                {
                    PlayerPrefs.SetString(_spritePathPlayerPrefsKey, setPath);
                    _spritesPath = setPath;
                }
            }

            if (GUILayout.Button("Clear Tiles"))
            {
                _tileMap.DestroyAllTiles();
                _tileList.Clear();
            }

            if (GUILayout.Button("Open Sprite Folder (스프라이트 폴더 오픈)"))
            {
                Application.OpenURL(_spritesPath);
            }
            DisplayAllPrefabs();
            DisplayCurrentSelect();
            GUILayout.EndScrollView();
        }
         void OnDisplay(int id)
         {
             GeneralDisPlay();
             if(_isHide)
                 return;

             switch (toolMode)
             {
                 case ToolModes.Transform:
                     break;
                 case ToolModes.Building:
                     BuildDisplay();
                     break;
                 case ToolModes.Painting:
                     PaintModeDisplay();
                     break;
                 default:
                     throw new ArgumentOutOfRangeException();
             }
           
        }

        void DisplayAllPrefabs()
        {
            var prefabList = _prefabList.Select(x => x.name).ToArray();
            _curIndex = GUILayout.SelectionGrid(_curIndex, _prefabList.Select(x => x.name).ToArray(),2);
            _selectPrefab = _prefabList[_curIndex];
        }
        private void DisplayCurrentSelect()
        {
            if (_selectTexture == null)
                return;
           GUILayout.TextArea("Selected Texture");
            if (_selectTexture != null)
            {
                GUILayout.Box(_selectTexture);
            }
        }
        #endregion
        private void SelectTile(GameObject selectObj)
        {
            if (_selectPrefab != selectObj)
            {
                _selectPrefab = selectObj;
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
    private void OnDisable()
    {
        SceneViewEditorWindow.Instance.Exit();
    }
    private void OnEnable()
    {
        SceneViewEditorWindow.Instance.TileMap = (CustomTileMap)target;
        SceneViewEditorWindow.Instance.Init(20, 20, 500, 250,0);
    }
}
