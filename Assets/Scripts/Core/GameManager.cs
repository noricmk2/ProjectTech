using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        TCSceneManager.Instance.Init();
        TCSceneManager.Instance.EnterScene(SceneBase.GameSceneType.Intro);
    }
}
