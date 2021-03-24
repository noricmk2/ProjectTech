using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : SceneBase
{
    public IntroScene(GameSceneType scene) : base(scene)
    {
    }

    public override IEnumerator Enter_C()
    {
        TCSceneManager.Instance.EnterScene(GameSceneType.Ingame);
        yield return null;
    }

    public override IEnumerator Exit_C()
    {
        yield return null;
    }
}
