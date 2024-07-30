using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sl;
using System;


public class CropZedTexture : MonoBehaviour
{
    public ZEDManager zedManager;
    public GameObject mainQuad;
    public GameObject testQuad;

    // public Camera mainCamera;
    // private sl.ZEDCamera zedCamera = null;
    // private float distance = 3.0f;
    private float aspect = 16.0f / 9.0f;
    private sl.ZEDCamera zedCamera = null;

    public Material blendMaterial;
    public Material rotateMaterial;
    public RenderTexture VERenderTexture;
    public RenderTexture FullRenderTexture;

    Shader cropShader; // blend render texture & zed texture
    Shader rotateShader; 

    [HideInInspector]
    public Texture2D zedTexture;
    [HideInInspector]
    public ZEDRenderingPlane zedRenderingPlane;

    void Start()
    {
        // set shader
        cropShader = Shader.Find("Unlit/PassThroughCropShader");
        rotateShader = Shader.Find("Unlit/RotationShader");
        
        // set zed rendering plane object
        zedRenderingPlane = zedManager.gameObject.GetComponentInChildren<ZEDRenderingPlane>();
        zedCamera = zedManager.zedCamera;
        mainQuad.SetActive(true);

        // Debug.Log("Steam Cam Aspect: " + mainCamera.aspect);
        // Debug.Log("Screen size: " + Screen.height + " " + Screen.width);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(zedCamera == null || !zedCamera.IsCameraReady || zedRenderingPlane.TextureEye == null)
            return;
            
        // get zed texture - automatically updated
        zedTexture = zedRenderingPlane.TextureEye;
        Texture2D zedLeftTexture = zedCamera.CreateTextureImageType(sl.VIEW.LEFT);
        Texture2D zedRightTexture = zedCamera.CreateTextureImageType(sl.VIEW.RIGHT);


        if(zedTexture != null){
            
            Texture2D newVETexture = RenderTextureToTexture2D(VERenderTexture);
            Texture2D newFullTexture = RenderTextureToTexture2D(FullRenderTexture);
            
            blendMaterial.SetTexture("_MainTex", zedTexture);
            blendMaterial.SetTexture("_VERenderTex", newVETexture);
            blendMaterial.SetTexture("_FullRenderTex", newFullTexture);

            /// zedTexture test: apply it to the quad material
            // testQuad.GetComponent<Renderer>().material.mainTexture = zedTexture;
            ///

            // Texture2D tmpTexture2D = (Texture2D) mainMaterial.GetTexture("_MainTex");
            // RenderTexture newRenderTexture = Texture2DToRenderTexture(tmpTexture2D, FullRenderTexture);
        }
        
    }

    public static Texture2D ChangeFormat(Texture2D oldTex, TextureFormat newFormat){
        Texture2D newTex = new Texture2D(oldTex.width, oldTex.height, newFormat, false);
        newTex.SetPixels(oldTex.GetPixels());
        newTex.Apply();
        return newTex;
    }

    public static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture){
        // Debug.Log("Original RT format: " + renderTexture.format);
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0,0,renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public static RenderTexture Texture2DToRenderTexture(Texture2D textuer2D, RenderTexture originalRenderTexture){
        RenderTexture newRenderTexture = new RenderTexture(originalRenderTexture.width, originalRenderTexture.height, 0);
        RenderTexture.active = newRenderTexture;

        Graphics.Blit(textuer2D, newRenderTexture);

        return newRenderTexture;
    }

    private void scale(GameObject screen, float fov)
    {
        float height = Mathf.Tan(0.5f * fov) * Vector3.Distance(screen.transform.localPosition, Vector3.zero) * 2;
        screen.transform.localScale = new Vector3((height * aspect), height, 1);
    }

    float GetFOVXFromProjectionMatrix(Matrix4x4 projection)
    {
        return Mathf.Atan(1 / projection[0, 0]) * 2.0f;
    }
}

