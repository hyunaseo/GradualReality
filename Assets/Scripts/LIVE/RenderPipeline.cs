using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
/// <summary>
///  This script is attached to each object and tracks its interactin state.
/// </summary>
public class RenderPipeline : MonoBehaviour
{
    GradualRealityManager GRManager; // GradualReality Manager to obtain parameters

    #region Interaction States
    
    public enum InteractionState
    {
        Perceive,
        Approach,
        SimpleManipulate,
        ComplexManipulate,
        Avoid
    }

    public InteractionState CurrenInteractionState = InteractionState.Perceive;
    
    #endregion


    #region Blending Methods

    // Affordance Contour 
    GameObject AffordanceContour;
    MeshRenderer AffordanceContourRenderer;

    // Boundary Box
    [HideInInspector]
    public GameObject BoundaryBox;
    MeshRenderer BoundaryBoxRenderer;
    LineRenderer BoundaryBoxLineRenderer = new LineRenderer();

    // Pass-Through 
    InteractionBehaviour PassThrough;
    GameObject PassThroughEllipsoid;
    MeshRenderer PassThroughRenderer;

    #endregion


    GameObject Tracker; // VIVE Tracker 
    MeshRenderer TrackerRenderer;

    InteractionButton interactionButton; // Interactable button provided by leap motion SDK 
    GameObject interactionButtonCore;
    MeshRenderer[] buttonRenderers = new MeshRenderer[2];

    //Booleans for Interaction State Tracking
    bool isComplexManipulateStateOn = false;
    bool isHandInPassThroughArea = false;
    bool isApproachStateOn = false;
    bool isTrackerPosChanged = false;
    public bool isAvoidStateOn = false;
    public bool isNonTargetObject = false;

    //Variables for manipulation state
    public int handNotInCount = 0;
    int ComplexManipulateStateFrameWindow;

    //Variables for move state
    List<GameObject> allTrackers = new List<GameObject>();
    float TrackingErrorThreshold;
    Vector3 CurrentPosition;
    Vector3 PriorPosition;
    public int notMoveCount = 0;
    int MovementDetectionFrameWindow = 30;

    void Start()
    {
        GRManager = GameObject.FindObjectOfType<GradualRealityManager>();
        Tracker = transform.GetChild(0).GetChild(0).gameObject;
        PriorPosition = transform.position;

        // Find the corresponding blending methods 
        foreach (Transform child in Tracker.transform)
        {
            if (child.tag == "AffordanceContour")
                AffordanceContour = child.gameObject;
            if (child.tag == "BoundaryBox")
                BoundaryBox = child.gameObject;
        }


        // Affordance Contour setting
        AffordanceContourRenderer = AffordanceContour.transform.GetChild(0).GetComponentInChildren<MeshRenderer>();

        // Boundary Box setting 
        BoundaryBoxRenderer = BoundaryBox.GetComponent<MeshRenderer>();
        BoundaryBoxLineRenderer = BoundaryBox.GetComponent<LineRenderer>();
        BoundaryBoxRenderer.enabled = false;
        BoundaryBoxLineRenderer.enabled = false;

        // Pass-Through setting 
        PassThrough = Tracker.transform.Find("PassThroughSphere Node").GetComponent<InteractionBehaviour>();
        PassThroughEllipsoid = PassThrough.transform.GetChild(0).gameObject;
        PassThroughRenderer = PassThroughEllipsoid.GetComponent<MeshRenderer>();

        // Default mesh renderer Setting
        TrackerRenderer = Tracker.GetComponent<MeshRenderer>();
        interactionButton = Tracker.transform.Find("Button").Find("Cube UI Button").GetComponent<InteractionButton>();
        interactionButtonCore = interactionButton.transform.GetChild(0).GetChild(0).gameObject;
        buttonRenderers[0] = interactionButtonCore.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // Retrieve threholds 
        TrackingErrorThreshold = GRManager.TrackingErrorThreshold;
        MovementDetectionFrameWindow = GRManager.MovementDetectionFrameWindow;
        ComplexManipulateStateFrameWindow = GRManager.ComplexManipulateStateFrameWindow;

        TrackInteractionState();

        switch (CurrenInteractionState)
        {
            case InteractionState.Perceive:
                RenderPerceive();
                break;

            case InteractionState.Approach:
                RenderGrab();
                break;

            case InteractionState.Avoid:
                RenderMove();
                break;

            case InteractionState.ComplexManipulate:
                RenderManipulate();
                break;
        }

        if(isNonTargetObject) {
            BoundaryBoxRenderer.enabled = true;
            BoundaryBoxLineRenderer.enabled = true;
        }

        TrackerRenderer.enabled = true;
    }

    void TrackInteractionState()
    {
        // Update the boolean values for the interaction state based on the input data
        isApproachStateOn = PassThrough.isPrimaryHovered || PassThrough.isHovered;
        
        isTrackerPosChanged = isTrackerMoving();
        if (isTrackerPosChanged) isAvoidStateOn = true;
        
        isComplexManipulateStateOn = interactionButton.isPressed;
        if (isComplexManipulateStateOn) isHandInPassThroughArea = true;
        
        // Set CurrentInteractionState 
        if (isComplexManipulateStateOn || isHandInPassThroughArea) CurrenInteractionState = InteractionState.ComplexManipulate;
        else if (isAvoidStateOn) CurrenInteractionState = InteractionState.Avoid;
        else if (isApproachStateOn) CurrenInteractionState = InteractionState.Approach;
        else CurrenInteractionState = InteractionState.Perceive; 
    }

    void RenderPerceive()
    {
        TrackerRenderer.enabled = false;
        // virtualProxyRenderer.enabled = true;

        AffordanceContourRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        PassThroughRenderer.enabled = false;

        if (!isNonTargetObject)
        {
            BoundaryBoxRenderer.enabled = false;
            BoundaryBoxLineRenderer.enabled = false;
        }
    }

    void RenderGrab()
    {
        TrackerRenderer.enabled = false;
        AffordanceContourRenderer.enabled = true;
        buttonRenderers[0].enabled = true;

        PassThroughRenderer.enabled = false;
    }

    void RenderMove()
    {
        //Set Renderer Enable Status
        TrackerRenderer.enabled = false;
        // virtualProxyRenderer.enabled = true;
        AffordanceContourRenderer.enabled = true;

        buttonRenderers[0].enabled = true;
        PassThroughRenderer.enabled = false;

        //Count Moving Frame
        if (isTrackerMoving()) notMoveCount = 0;
        else notMoveCount++;

        //If not moving, turn off Box
        if (notMoveCount > MovementDetectionFrameWindow)
        {
            isAvoidStateOn = false;
            notMoveCount = 0;
        }
        else
        {
            isAvoidStateOn = true;
        }
    }

    public void RenderManipulate()
    {
        //Set Renderer Enable Status
        PassThroughRenderer.enabled = true;

        TrackerRenderer.enabled = false;
        // virtualProxyRenderer.enabled = false;
        AffordanceContourRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        
        //Count Hand In Frame #
        if (isHandInSphere()) handNotInCount = 0;
        else handNotInCount++;

        //If Hand is not in, turn off PT
        if (handNotInCount > ComplexManipulateStateFrameWindow)
        {
            isHandInPassThroughArea = false;
            handNotInCount = 0;
        }
        else
        {
            isHandInPassThroughArea = true;
        }
    }
    

    bool isHandInSphere()
    {
        return isApproachStateOn;
    }

    bool isTrackerMoving()
    {
        CurrentPosition = transform.position;
        if (Vector3.Distance(CurrentPosition, PriorPosition) > TrackingErrorThreshold)
        {
            PriorPosition = CurrentPosition;
            return true;
        }

        else
        {
            return false;
        }
    }
}