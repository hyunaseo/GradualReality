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

    // Start is called before the first frame update
    void Start()
    {
        // set shader
        rotateShader = Shader.Find("Unlit/RotationShader");

        // set zed rendering plane object
        zedRenderingPlane = zedManager.gameObject.GetComponentInChildren<ZEDRenderingPlane>();
    }

    // Update is called once per frame
    void Update()
    {
        // get zed texture - automatically updated
        zedTexture = zedRenderingPlane.TextureEye;

        /// zedTexture test: apply it to the quad material
        // testQuad.GetComponent<Renderer>().material.mainTexture = zedTexture;
        ///

        if(zedTexture != null){
            mainMaterial.SetTexture("_MainTex", zedTexture);
        }
    }
}
