using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sl;

public class GrabZedFrame : MonoBehaviour
{

    public ZEDManager zedManager;
    sl.ZEDCamera zedCam;

    InitParameters init_params;
    RuntimeParameters runtimeParameters;

    uint imageWidth;
    uint ImageHeight;
    sl.ZEDMat image;

    int imageNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        zedCam = zedManager.zedCamera;
        imageWidth = (uint)zedCam.ImageWidth;
        ImageHeight = (uint)zedCam.ImageHeight;
    }

    // Update is called once per frame
    void Update()
    {
        runtimeParameters = new RuntimeParameters();
        image = new ZEDMat();
        image.Create(imageWidth, ImageHeight, ZEDMat.MAT_TYPE.MAT_8U_C4, ZEDMat.MEM.MEM_CPU);

        if(zedCam.Grab(ref runtimeParameters) == ERROR_CODE.SUCCESS){
            zedCam.RetrieveImage(image, VIEW.LEFT);
            ulong timestamp = zedCam.GetCameraTimeStamp();

            imageNum +=1;
            string fileName = "ZedFrameGrab_" + imageNum.ToString() + ".png"; 
            Debug.Log("ZEDFRAMEGRABBER: grab image success");
            image.Write("MYTESTImage.png", -1);
        }
    }

    
}