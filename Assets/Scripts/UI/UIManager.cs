using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    #region Define
    public enum UIType
    {
        Title,
        Ingame,
    }

    public enum CanvasType
    {
        Overlay,
        Camera,
    }
    #endregion
    
    #region Inspector
    [SerializeField] private Canvas _overlayCanvas;
    [SerializeField] private Canvas _cameraCanvas;
    [SerializeField] private Camera _uiCamera;
    #endregion

    #region Property
    private Stack<UIController> _currentUIStack = new Stack<UIController>();
    private Dictionary<UIType, UIController> _uiControllerDict = new Dictionary<UIType, UIController>();

    public Canvas OverlayCanvas => _overlayCanvas;
    public Canvas CameraCanvas => _cameraCanvas;
    public Camera UICamera => _uiCamera;
    #endregion

    public void Init()
    {
        _uiControllerDict = new Dictionary<UIType, UIController>();
        int count = Enum.GetValues(typeof(UIType)).Length;
        for (int i = 0; i < count; ++i)
        {
            UIType type = (UIType) i;
            _uiControllerDict[type] = CreateController(type);
        }
    }

    private UIController CreateController(UIType type)
    {
        UIController controller = null;
        switch (type)
        {
            case UIType.Title:
                break;
            case UIType.Ingame:
                controller = new IngameUIController();
                break;
        }

        return controller;
    }

    public UIType GetUIType<T>() where T : UIController
    {
        switch (typeof(T).Name)
        {
            case "IngameUIController":
                return UIType.Ingame;
            case "TitleUIController":
                return UIType.Title;
        }

        return UIType.Ingame;
    }

    public T GetController<T>() where T : UIController
    {
        var type = GetUIType<T>();
        if (_uiControllerDict.ContainsKey(type))
            return _uiControllerDict[type] as T;
        else
            return CreateController(type) as T;
    }

    private void Update()
    {
        var iter = _currentUIStack.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.OnUpdate();
        }
    }

    public UIController GetCurrentController()
    {
        if(_currentUIStack.Count == 0)
            DebugEx.LogError($"[Failed] ui stack is null");
        
        return _currentUIStack.Peek();
    }

    public UIController OpenUI(UIType type, bool overlap = false)
    {
        var targetController = _uiControllerDict[type];
        if (_currentUIStack.Contains(targetController))
        {
            if (overlap)
                _currentUIStack.Push(targetController);
            else
            {
                var tempStack = new Stack<UIController>();
                UIController temp = null;
                while (_currentUIStack.Count > 0)
                {
                    temp = _currentUIStack.Pop();
                    if(temp != targetController) 
                        tempStack.Push(temp);
                    else
                        break;
                }
                while (tempStack.Count > 0)
                    _currentUIStack.Push(tempStack.Pop());
                
                _currentUIStack.Push(targetController);
            }
        }
        else
        {
            _currentUIStack.Push(targetController);
            targetController.Init();
        }

        return targetController;
    }

    public T OpenUI<T>() where T : UIController
    {
        var type = GetUIType<T>();
        return OpenUI(type) as T;
    }
    
    public void CloseAll()
    {
        while (_currentUIStack.Count > 0)
        {
            var target = _currentUIStack.Pop();
            target.CloseUI();
        }
    }

    public void CloseCurrentUI()
    {
        if (_currentUIStack.Count > 0)
        {
            var target = _currentUIStack.Pop();
            target.CloseUI();
        }
        else
        {
            DebugEx.LogError($"[Failed] ui stack is empty");
        }
    }

    public static T CreateView<T>(string name, CanvasType canvas = CanvasType.Overlay) where T : UIView
    {
        Canvas targetCanvas = null;
        switch (canvas)
        {
            case CanvasType.Overlay:
                targetCanvas = UIManager.Instance.OverlayCanvas;
                break;
            case CanvasType.Camera:
                targetCanvas = UIManager.Instance.CameraCanvas;
                break;
        }
        var obj = ObjectFactory.Instance.CreateObject<T>(name, targetCanvas.transform, LayerSettings.UI);
        return obj as T;
    }
}
