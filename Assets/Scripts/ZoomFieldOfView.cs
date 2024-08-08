using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// This script handles the FoV (Field of View) adjustments. 
/// The Zed Mini's FoV is 118° max, smaller than VR HMDs. This causes Pass-Through clipping when triggered outside the Zed Mini's FoV.
/// To fix this, the VR HMD's FoV zoom factor is increased to match the Zed Mini.
/// Users can set changeFoV to false if this adjustment is not needed.
/// </summary>
public class ZoomFieldOfView : MonoBehaviour
{
    [Tooltip("Set true to change the FoV.")]
    public bool ZoomFOV = false;

    [Tooltip("Ratio to apply when adjust the FoV. Default value for HTC VIVE PRO 2 is 1.1f.")]
    public float FOVRatio = 1.1f;

    void Update()
    {
        if(ZoomFOV)
        {
            if (XRDevice.fovZoomFactor != FOVRatio){
                XRDevice.fovZoomFactor = FOVRatio;   
                Debug.Log("XRDevice fovZoomFactor update: " + XRDevice.fovZoomFactor);
            }
        }
    }
}
