using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameScene : SceneBase
{
    public IngameScene(GameSceneType scene) : base(scene)
    {
    }

    public override IEnumerator Enter_C()
    {
        IngameManager.Instance.Init();
        yield return null;
    }

    public override IEnumerator Exit_C()
    {
        yield return null;
    }
}
