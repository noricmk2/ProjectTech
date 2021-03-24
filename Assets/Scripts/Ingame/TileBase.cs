using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : ObjectBase
{
    public class TileInfo
    {
        public JPSNode nodeInfo;
    }

    public MeshRenderer renderer;

    #region Property
    private TileInfo _curInfo;

    public TileInfo Info => _curInfo;
    #endregion

    public void Init(TileInfo info)
    {
        _curInfo = info;
        //Test
        if (info.nodeInfo.state == 0)
        {
            renderer.material.color = Color.red;
        }
    }
}
