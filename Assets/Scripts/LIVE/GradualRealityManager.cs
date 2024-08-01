using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GradualRealityManager : MonoBehaviour
{
    [Header("Interaction State Triggering Thresholds")]
    public float ApproachStateDistance = 0.12f;
    public float AvoidStateDistance = 0.30f;
    public int ComplexManipulateStateTimeWindow = 90;

    [Header("Blending Method Parameters")]
    public Color BoundaryBoxLineColor;

    [HideInInspector]
    public float TrackingErrorThreshold = 0.005f;
    [HideInInspector]
    public int MovementDetectionTimeWindow = 30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractionManager[] HandManagers = GameObject.FindObjectsOfType<InteractionManager>();
        for(int i=0; i<HandManagers.Length; i++){
            HandManagers[i].hoverActivationRadius = ApproachStateDistance;
        }
    }
}
