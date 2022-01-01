using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectUtility : MonoBehaviour
{
    public static int targetWidth = 720;
    public static int targetHeight = 1280;

    public static int maxWidth = 720;
    public static int maxHeight = 1280;

    static float targetAspectRatio;
    static float maxAspectRatio;
    static Camera cam;
    //static Camera backgroundCam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam)
        {
            cam = Camera.main;
        }
        if (!cam)
        {
            Debug.LogError("No camera available");
            return;
        }
        targetAspectRatio = (float)targetWidth / targetHeight;
        maxAspectRatio = (float)maxWidth / maxHeight;
        SetCamera();
    }

    public static void SetCamera()
    {
        // if (!backgroundCam)
        // {
        //     //var prefab = AddressableManager.Instance.LoadAssetSync<GameObject>("BackgroundCam");
        //     var inst = AddressableManager.Instance.InstantiateSync("BackgroundCam", null);
        //     inst.layer = LayerSettings.Background;
        //     DontDestroyOnLoad(inst);
        //     backgroundCam = inst.GetComponentInChildren<Camera>();
        //     backgroundCam.depth = int.MinValue;
        //     backgroundCam.clearFlags = CameraClearFlags.SolidColor;
        //     backgroundCam.backgroundColor = Color.black;
        // }

        // SafeArea
        var safeArea = Screen.safeArea;
        if (safeArea.width < Screen.width)
            cam.rect = new Rect(safeArea.x / Screen.width, cam.rect.y, safeArea.width / Screen.width, cam.rect.height);

        if ((int)(currentAspectRatio * 100) / 100.0f == (int)(targetAspectRatio * 100) / 100.0f)
        {
            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            return;
        }

        // Pillarbox
        if (currentAspectRatio > maxAspectRatio)
        {
            float inset = 1.0f - maxAspectRatio / currentAspectRatio;
            //Debug.Log(new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f));
            cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
        }
        // Letterbox
        else if(currentAspectRatio < targetAspectRatio)
        {
            float inset = 1.0f - currentAspectRatio / targetAspectRatio;
            cam.rect = new Rect(cam.rect.x, inset / 2, cam.rect.width, 1.0f - inset);
            if(cam.orthographic)
                cam.orthographicSize = 1.0f - inset;
        }
    }

    public static float currentAspectRatio
    {
        get { return (float)Screen.width / Screen.height; }
    }

    public static float targetAspectRation
    {
        get { return (float)targetWidth / targetHeight; }
    }

    public static Vector3 mousePosition
    {
        get
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y -= (int)(cam.rect.y * Screen.height);
            mousePos.x -= (int)(cam.rect.x * Screen.width);
            return mousePos;
        }
    }

    public static Vector2 guiMousePosition
    {
        get
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Mathf.Clamp(mousePos.y, cam.rect.y * Screen.height, cam.rect.y * Screen.height + cam.rect.height * Screen.height);
            mousePos.x = Mathf.Clamp(mousePos.x, cam.rect.x * Screen.width, cam.rect.x * Screen.width + cam.rect.width * Screen.width);
            return mousePos;
        }
    }
}
