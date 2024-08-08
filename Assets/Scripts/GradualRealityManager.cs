using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script is to set paramters of GradualReality. 
/// Originally, those are set following our paper, but you can modify it by yourself too!
/// </summary>

public class GradualRealityManager : MonoBehaviour
{
    [Header("Interaction State Triggering Thresholds")]
    
    [Tooltip("Hand-Object distance threshold to trigger Approach State (units: m)")]
    public float ApproachStateDistance = 0.12f;
    [Tooltip("Object-Object distance threshold to trigger Avoid State (units: m)")]
    public float AvoidStateDistance = 0.15f;
    [Tooltip("Time threshold to automatically turn off Pass-Through for Complex Manipulate State (units: sec)")]
    public int ComplexManipulateStateTime = 3;
    
    [HideInInspector]
    public int ComplexManipulateStateFrameWindow; // ComplexManipulateStateTime should be converted as frames 

    [Header("Blending Method Parameters")]

    [Tooltip("Material for Affordance Contours")]
    public Material AffordanceContourMaterial;
    [Tooltip("Color of Boundary Box's line")]
    public Color BoundaryBoxLineColor = new Color(0f, 1f, 0.8039216f);

    [HideInInspector]
    public float TrackingErrorThreshold = 0.005f;
    [HideInInspector]
    public int MovementDetectionFrameWindow = 30;

    // [Header("Others")]

    // [Tooltip("Check if you want to render the tracker")]
    // public bool TrackerRenderingEnabled;
    // public Material TrackerMaterial; 

    void Update()
    {
        // Convert ComplexManipulateStateTime to Frames 
        ComplexManipulateStateFrameWindow = ComplexManipulateStateTime*30;

        // Set Leap motion hands' hovering distance as Approach State Distance. 
        // HoverAcionRadius is used to trigger the virtual button rendering. 
        InteractionManager[] HandManagers = GameObject.FindObjectsOfType<InteractionManager>();
        for(int i=0; i<HandManagers.Length; i++){
            HandManagers[i].hoverActivationRadius = ApproachStateDistance;
        }
    }
}
