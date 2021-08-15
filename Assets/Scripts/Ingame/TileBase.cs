using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : ObjectBase
{
    public class TileInfo
    {
        public JPSNode nodeInfo;
        public MapNodeType nodeType;
        public string spanwerName;
    }

    public MeshRenderer renderer;

    #region Property
    private Spawner _spawner;
    private TileInfo _curInfo;
    public TileInfo Info => _curInfo;
    public Spawner CharacterSpanwer => _spawner;
    #endregion

    public void Init(TileInfo info)
    {
        _curInfo = info;
        switch (info.nodeType)
        {
            //Test
            case MapNodeType.Block:
                renderer.material.color = Color.red;
                break;
            case MapNodeType.Road:
                break;
            case MapNodeType.Spanwer:
                _spawner = gameObject.AddComponent<Spawner>();
                _spawner.SetSpanwerName(info.spanwerName);
                break;
        }
    }
}
