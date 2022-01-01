using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCSceneManager : MonoSingleton<TCSceneManager>
{
    private Dictionary<SceneBase.GameSceneType, SceneBase> _sceneDic = new Dictionary<SceneBase.GameSceneType, SceneBase>();
    private SceneBase _currentScene;

    public void Init()
    {
        var count = (int)SceneBase.GameSceneType.Length;
        for (int i = 0; i < count; ++i)
            _sceneDic[(SceneBase.GameSceneType)i] = SceneBase.CreateScene((SceneBase.GameSceneType)i);
    }

    public void EnterScene(SceneBase.GameSceneType scene, bool loadAsync = false, bool unloadResource = true, bool forcedEnter = false)
    {
        if (forcedEnter || _currentScene == null || scene != _currentScene.SceneType)
            StartCoroutine(LoadScene_C(scene, loadAsync, unloadResource));
    }

    IEnumerator LoadScene_C(SceneBase.GameSceneType scene, bool loadAsync, bool unloadResource)
    {
        bool isFirstScene = false;

        if (_currentScene != null)
            yield return StartCoroutine(_currentScene.Exit_C());
        else
            isFirstScene = true;

        if (unloadResource)
        {
            ObjectFactory.Instance.Release();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        var nextScene = _sceneDic[scene];
        if (!isFirstScene)
        {
            if (loadAsync)
            {
                var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene.SceneName);
                while (!operation.isDone)
                {
                    yield return null;
                    _currentScene = nextScene;
                    yield return StartCoroutine(_currentScene.Enter_C());
                }
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene.SceneName);
                yield return null;
                _currentScene = nextScene;
                yield return StartCoroutine(_currentScene.Enter_C());
            }
        }
        else
        {
            _currentScene = nextScene;
            yield return StartCoroutine(_currentScene.Enter_C());
        }
    }
}
