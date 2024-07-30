using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera mainCamera;
    public float offsetX;
    public float offsetY;
    public float offsetZ;

    Vector3 parentGlobalPosition;
    void Start(){
        
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.parent == null) return;
        
        //Debug.Log("Parent Object Name: " + transform.parent.name);
        parentGlobalPosition = transform.parent.transform.position;
        // transform.position = parentGlobalPosition + new Vector3(offsetX, offsetY, offsetZ);

        // transform.LookAt(mainCamera.transform);
        transform.rotation = mainCamera.transform.rotation;
        // transform.rotation = transform.parent.transform.rotation;
    }
}
