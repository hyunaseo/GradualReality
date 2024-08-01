using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZedTexture : MonoBehaviour
{
    public ZEDManager zedManager;
    public Material mainMaterial;
    public GameObject testQuad; 
    
    Shader rotateShader;

    [HideInInspector]
    public Texture2D zedTexture;
    [HideInInspector]
    public ZEDRenderingPlane zedRenderingPlane;

    void Start()
    {
        rotateShader = Shader.Find("Unlit/RotationShader");

        zedRenderingPlane = zedManager.gameObject.GetComponentInChildren<ZEDRenderingPlane>();
    }

    void Update()
    {
        zedTexture = zedRenderingPlane.TextureEye;

        if(zedTexture != null){
            mainMaterial.SetTexture("_MainTex", zedTexture);
        }
    }
}
