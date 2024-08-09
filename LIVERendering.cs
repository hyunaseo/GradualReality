using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sl;
public class LIVERendering : MonoBehaviour
{
    enum RenderingState
    {
        VirtualProxy,
        VirtualIndicator,
        PassThrough
    }

    public InputMaster inputMaster;
    public GameObject hand3D;
    public float Threshold1;
    
    GameObject leftWrist;
    GameObject leftPalm;
    GameObject rightWrist;
    GameObject rightPalm;

    GameObject tracker2;
    GameObject virtualProxy2;

    private RenderingState currentRenderingState = RenderingState.VirtualProxy;
    
    [Tooltip("ZEDManager in the scene used to grab the image." +
        "Note this script isn't currently designed for multiple ZEDs.")]
    public ZEDManager zedManager;
    ZEDCamera zedCam;
    CameraParameters cameraParams;
    RuntimeParameters runtimeParameters;

    private void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.Enable();

        leftWrist = hand3D.transform.Find("ObjModelHandLeft_26").gameObject.transform.Find("Wrist").gameObject;
        leftPalm = leftWrist.transform.Find("Palm").gameObject;
        rightWrist = hand3D.transform.Find("ObjModelHandRight_26").gameObject.transform.Find("Wrist").gameObject;
        rightPalm = rightWrist.transform.Find("Palm").gameObject;

        tracker2 = GameObject.Find("Tracker2").gameObject;
        virtualProxy2 = tracker2.transform.Find("Model").gameObject;
        
        zedCam = zedManager.zedCamera;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = getHandObjectDistance();

        // Decide the rendering state
        if(currentDistance < Threshold1){
            currentRenderingState = RenderingState.PassThrough;
        }

        else{
            currentRenderingState = RenderingState.VirtualProxy;
        }

        Debug.Log("Current VisualState: " + currentRenderingState + " , Dst: " + currentDistance);

        switch (currentRenderingState){
            case RenderingState.VirtualProxy:
                virtualProxy2.SetActive(true);
                break;

            case RenderingState.PassThrough:
                virtualProxy2.SetActive(false);

                // Get the zed camear raw frame
                uint mWidth = (uint)zedCam.ImageWidth;
                uint mHeight = (uint)zedCam.ImageHeight;
                
                // Initialize the Mat that will contain the left image
                ZEDMat image = new ZEDMat();
                image.Create(mWidth, mHeight, ZEDMat.MAT_TYPE.MAT_8U_C4, ZEDMat.MEM.MEM_CPU); // Mat needs to be created before use.

                runtimeParameters = new sl.RuntimeParameters();
                if(zedCam.Grab(ref runtimeParameters)== sl.ERROR_CODE.SUCCESS){
                    // A new image is available if Grab() returns ERROR_CODE.SUCCESS
                    zedCam.RetrieveImage(image, VIEW.LEFT); // Get the left image
                    ulong timestamp = zedCam.GetCameraTimeStamp(); // Get image timestamp
                    Debug.Log("Image resolution: " + image.GetWidth() + "x" + image.GetHeight() + " || Image timestamp: " + timestamp);
                }
                break;

            case RenderingState.VirtualIndicator:
                break;
        }
    }

    float getHandObjectDistance(){
        // Get trackers' distances
        Vector3 tracker2Pos = tracker2.transform.position;

        // Get palms' positions
        Vector3 leftPalmPos = leftPalm.transform.position;
        Vector3 rightPalmPos = rightPalm.transform.position;

        // Compare distance
        float leftHandDistance = Vector3.Distance(leftPalmPos, tracker2Pos);
        float rightHandDistance = Vector3.Distance(rightPalmPos, tracker2Pos);

        float currentDistance = leftHandDistance < rightHandDistance ? leftHandDistance : rightHandDistance;

        return currentDistance;
    }
}
