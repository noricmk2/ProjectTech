using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Temp script
public class IngameUIController : MonoBehaviour
{
    public class InitData
    {
        public Canvas ingameCanvas;
    }

    private Canvas _ingameCanvas;
    
    public void Init(InitData data)
    {
        _ingameCanvas = data.ingameCanvas;
    }

    public Canvas GetIngameCanvas()
    {
        return _ingameCanvas;
    }
}
