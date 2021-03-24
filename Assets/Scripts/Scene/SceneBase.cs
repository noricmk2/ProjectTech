using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public enum GameSceneType
    {
        Intro,
        Ingame,
        Length
    }

    public GameSceneType SceneType { get; private set; }
    public string SceneName { get; private set; }

    public SceneBase(GameSceneType scene)
    {
        SceneType = scene;
        SceneName = scene.ToString();
    }

    public static SceneBase CreateScene(GameSceneType scene)
    {
        SceneBase ret = null;
        switch (scene)
        {
            case GameSceneType.Intro:
                ret = new IntroScene(GameSceneType.Intro);
                break;
            case GameSceneType.Ingame:
                ret = new IngameScene(GameSceneType.Ingame);
                break;
        }
        return ret;
    }

    public abstract IEnumerator Enter_C();
    public abstract IEnumerator Exit_C();
}
