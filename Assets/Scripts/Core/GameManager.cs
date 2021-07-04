using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private TableLoader _tableLoader;

    public bool TableLoadFinished => _tableLoader.IsFinished;
    
    private void Awake()
    {
        TCSceneManager.Instance.Init();
        TCSceneManager.Instance.EnterScene(SceneBase.GameSceneType.Intro);
    }

    public void LoadTable()
    {
        _tableLoader.LoadTable();
    }
}
