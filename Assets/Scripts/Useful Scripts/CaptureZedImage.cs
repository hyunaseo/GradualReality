using System.Collections;
using System.Collections.Generic;

using System.IO;
using UnityEngine;
using sl;

public class CaptureZedImage : MonoBehaviour
{
    public ZEDManager zedManager;
    sl.ZEDCamera zedCam;

    InitParameters init_params;
    RuntimeParameters runtimeParameters;

    private string rootFolderName = "Figures";
    private int iterationNumber;
    private int imageNumber=0;
    private string folderPath;

    uint imageWidth;
    uint ImageHeight;
    sl.ZEDMat image;
    private List<(sl.ZEDMat, string)> imageStringList = new List<(sl.ZEDMat, string)>();

    private void Start()
    {
        iterationNumber = PlayerPrefs.GetInt("IterationNumber", 0) + 1;
        folderPath = Path.Combine(Application.dataPath, rootFolderName, iterationNumber.ToString());

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Folder created: " + folderPath);

            PlayerPrefs.SetInt("IterationNumber", iterationNumber);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Folder already exists: " + folderPath);
        }
    
        zedCam = zedManager.zedCamera;
        imageWidth = (uint)zedCam.ImageWidth;
        ImageHeight = (uint)zedCam.ImageHeight;


    }

    private void Update()
    {
        runtimeParameters = new RuntimeParameters();
        image = new ZEDMat();
        image.Create(imageWidth, ImageHeight, ZEDMat.MAT_TYPE.MAT_8U_C4, ZEDMat.MEM.MEM_CPU);

        if(zedCam.Grab(ref runtimeParameters) == ERROR_CODE.SUCCESS){
            // Incremental filename
            string filename = folderPath + "/" + imageNumber.ToString() + ".png";

            zedCam.RetrieveImage(image, VIEW.LEFT);
            var tuple = (image, filename);
            imageStringList.Add(tuple);
            
            // Increment the iteration number for the next frame
            imageNumber++;
        }
    }

    private void OnApplicationQuit()
    {
        
        foreach (var pair in imageStringList)
        {
            sl.ZEDMat image = pair.Item1;
            string filename = pair.Item2;

            // Use image and text as needed
            Debug.Log($"Image String: {filename}");

            image.Write(filename, -1);
        }
    }
}
