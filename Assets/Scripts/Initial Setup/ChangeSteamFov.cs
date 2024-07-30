using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ChangeSteamFov : MonoBehaviour
{

    public float changeFov;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("XRDevice fovZoomFactor: " + XRDevice.fovZoomFactor);
        
    }

    // Update is called once per frame
    void Update()
    {
        XRDevice.fovZoomFactor = changeFov;
        // if (changeFov != 1.1)
        //     Debug.Log("XRDevice fovZoomFactor update: " + XRDevice.fovZoomFactor);
    }
}
