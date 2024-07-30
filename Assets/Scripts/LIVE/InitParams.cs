using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InitParams : MonoBehaviour
{
    public float affordanceHandThr = 0.12f;
    public float obstacleDistThr = 0.30f;
    public float trackerMovingErrorThr = 0.005f;
    public int moveWindow = 30;
    public int manipulateWindow = 90;
    public Color lineColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractionManager[] HandManagers = GameObject.FindObjectsOfType<InteractionManager>();
        for(int i=0; i<HandManagers.Length; i++){
            HandManagers[i].hoverActivationRadius = affordanceHandThr;
        }
    }
}
