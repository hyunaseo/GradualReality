using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sl;
using System;

/// <summary>
/// This script is for rendering a Pass-Through view of a physical object within a virtual environment. This process involves several key steps:
/// 
/// 1. Continuously reading the camera feed from the ZED camera.
/// 2. Determining the ellipsoid region that encloses the physical object.
/// 3. Cropping the ellipsoid region from the camera feed.
/// 4. Blending the cropped region into the main camera render texture. 
/// 5. Rendering the final render texture.
/// </summary>

public class RenderPassThrough : MonoBehaviour
{
    [Tooltip("ZED Manager of ZED Camera Rig")]
    public ZEDManager zedManager;
    private sl.ZEDCamera zedCamera = null;

    [Tooltip("Raw image that applies a material with Pass-Through effect")]
    public GameObject RenderingRawImage;

    [Tooltip("Material that generates render texture of blending Virtual Environment and Pass-Through")]
    public Material BlendMaterial;
    [Tooltip("Render texture from Pass-Through Camera Rig/Rendering Camaera. It makes Pass-Through area transparent.")]
    public RenderTexture PassThroughMaskRenderTexture;

    [HideInInspector]
    public Texture2D zedTexture;
    [HideInInspector]
    public ZEDRenderingPlane zedRenderingPlane;

    void Start()
    {
        if (zedManager == null)
        {
            zedManager = FindObjectOfType<ZEDManager>();
        }
        
        // Set ZED rendering plane and camera
        zedRenderingPlane = zedManager.gameObject.GetComponentInChildren<ZEDRenderingPlane>();
        zedCamera = zedManager.zedCamera;
        
        // This should be set active in the run-time
        RenderingRawImage.SetActive(true);
    }

    void Update()
    {
        // Check the ready status of the ZED Camera
        if (!RenderingRawImage.activeSelf)
            RenderingRawImage.SetActive(true);

        // Check the ready status of the ZED Camera
        if (zedCamera == null || !zedCamera.IsCameraReady || zedRenderingPlane.TextureEye == null)
            return;
        
        // Retrieve ZED Camera feed. ZED SDK automatically update it. 
        zedTexture = zedRenderingPlane.TextureEye;

        if(zedTexture != null){
            Texture2D PassThroughMask2DTexture = RenderTextureToTexture2D(PassThroughMaskRenderTexture);
            
            BlendMaterial.SetTexture("_RealWorldTex", zedTexture);
            BlendMaterial.SetTexture("_PassThroughMaskTex", PassThroughMask2DTexture);
        }
        
    }

    // Convert render texture type to 2D texture 
    public static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture){
        
        // Create a new Texture2D with the same dimensions and format as the RenderTexture
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

        // Set the active RenderTexture to ensure ReadPixels reads from this specific texture
        RenderTexture.active = renderTexture;

        // Read the pixel data from the RenderTexture into the Texture2D
        tex.ReadPixels(new Rect(0,0,renderTexture.width, renderTexture.height), 0, 0);
        // Apply the changes to the Texture2D to finalize the data copy
        tex.Apply();

        return tex;
    }
}

