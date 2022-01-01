using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : SceneBase
{
    private UITitleController _titleController;
    
    public IntroScene(GameSceneType scene) : base(scene)
    {
    }

    public override IEnumerator Enter_C()
    {
        GameManager.Instance.LoadTable();
        while (!GameManager.Instance.TableLoadFinished)
            yield return null;

        _titleController = UIManager.Instance.OpenUI<UITitleController>();
        _titleController.ShowUI();
        
        yield return null;
    }

    public override IEnumerator Exit_C()
    {
        UIManager.Instance.CloseAll();
        yield return null;
    }
}
