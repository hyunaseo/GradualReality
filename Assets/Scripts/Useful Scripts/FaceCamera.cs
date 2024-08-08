using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this script to the virtual button, so that you can press it easily. 
/// </summary>
public class FaceCamera : MonoBehaviour
{
    Camera mainCamera;
    bool isCameraFound = false;
    Vector3 parentGlobalPosition;
    
    void Update()
    {
        if(!isCameraFound){
            var gameObject = GameObject.Find("Main Camera");
            if (gameObject != null){
                mainCamera = gameObject.GetComponent<Camera>();

                if(mainCamera != null){
                    isCameraFound = true;
                }
            }
        }
        

        if (isCameraFound)
        {
            parentGlobalPosition = transform.parent.transform.position;
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
