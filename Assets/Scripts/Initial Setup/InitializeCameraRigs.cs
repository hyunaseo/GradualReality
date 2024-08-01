using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sl;

public class InitializeCameraRigs : MonoBehaviour
{
    public ZEDManager zedManager;
    public GameObject mainCamera;
    public GameObject PTCanvas;
    
    bool mainCameraOn = false;
    bool canvasOn = false;
    
    sl.ZEDCamera zedCamera;

    [HideInInspector]
    public ZEDRenderingPlane zedRenderingPlane;

    void Start()
    {
        zedCamera = zedManager.zedCamera;
        zedRenderingPlane = zedManager.gameObject.GetComponentInChildren<ZEDRenderingPlane>();
    }

    // Update is called once per frame
    void Update()
    {
        // (1) turn on main camera
        if (zedCamera != null && zedCamera.IsCameraReady && !mainCameraOn)
        {
            mainCamera.SetActive(true);
            mainCameraOn = true;
        }


        // (2) turn on canvas
        if (zedCamera != null && zedCamera.IsCameraReady && zedRenderingPlane.TextureEye != null && !canvasOn && mainCameraOn )
        {
            PTCanvas.SetActive(true);
            canvasOn = true;
        }
    }
}
