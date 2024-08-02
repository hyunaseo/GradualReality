using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
/// <summary>
///  This script is attached to each object and tracks its interactin state.
/// </summary>
public class InteractionStateAwareBlending : MonoBehaviour
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

    //Booleans for Interaction State Tracking
    bool isApproachStateOn = false;

    bool isSimpleManipulateStateOn = false;
    int NoMovementDetectionFrame;
    int MovementDetectionFrameWindow;

    bool isComplexManipulateStateOn = false;
    int NoComplexManipulateStateFrame;
    int ComplexManipulateStateFrameWindow;
    bool isHandInPassThroughArea = false;

    [HideInInspector]
    public bool isNonTargetObject = false;

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


    #region Trackers and Virtual Buttons

    public bool isTrackerRenderingEnabled;
    GameObject Tracker; // VIVE Tracker 
    MeshRenderer TrackerRenderer;
    float TrackingErrorThreshold;

    InteractionButton interactionButton; // Interactable button provided by leap motion SDK 
    GameObject interactionButtonCore;
    MeshRenderer[] buttonRenderers = new MeshRenderer[2];

    #endregion


    Vector3 CurrentPosition;
    Vector3 PriorPosition;

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

        // Track interaction state and update CurrentInteractionState
        TrackInteractionState();

        // Select blending method for CurrentInteractionState
        switch (CurrenInteractionState)
        {
            case InteractionState.Perceive:
                RenderVirtualProxy();
                break;

            case InteractionState.Approach:
                RenderAffordanceContour_ApproachState();
                break;

            case InteractionState.SimpleManipulate:
                RenderAffordanceContour_SimpleManipulateState();
                break;

            case InteractionState.ComplexManipulate:
                RenderPassThrough();
                break;

            case InteractionState.Avoid:
                RenderBoundaryBox();
                break;
        }

        if (isTrackerRenderingEnabled)
            TrackerRenderer.enabled = true;
    }

    void TrackInteractionState()
    {
        // Update the boolean values for the interaction state based on the input data
        isApproachStateOn = PassThrough.isPrimaryHovered || PassThrough.isHovered;
        
        if (isTargetObjectMoving()) isSimpleManipulateStateOn = true;
        
        isComplexManipulateStateOn = interactionButton.isPressed;
        if (isComplexManipulateStateOn) isHandInPassThroughArea = true;
        
        // Set CurrentInteractionState 
        if (isComplexManipulateStateOn || isHandInPassThroughArea) CurrenInteractionState = InteractionState.ComplexManipulate;
        else if (isSimpleManipulateStateOn) CurrenInteractionState = InteractionState.SimpleManipulate;
        else if (isApproachStateOn) CurrenInteractionState = InteractionState.Approach;
        else if (isNonTargetObject) CurrenInteractionState = InteractionState.Avoid;
        else CurrenInteractionState = InteractionState.Perceive; 
    }

    void RenderVirtualProxy()
    {
        AffordanceContourRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        PassThroughRenderer.enabled = false;

        if (!isNonTargetObject)
        {
            BoundaryBoxRenderer.enabled = false;
            BoundaryBoxLineRenderer.enabled = false;
        }
    }

    void RenderAffordanceContour_ApproachState()
    {
        AffordanceContourRenderer.enabled = true;
        buttonRenderers[0].enabled = true;

        PassThroughRenderer.enabled = false;
    }

    void RenderAffordanceContour_SimpleManipulateState()
    {
        RenderAffordanceContour_ApproachState();

        if (isTargetObjectMoving()) NoMovementDetectionFrame = 0;
        else NoMovementDetectionFrame++;

        if (NoMovementDetectionFrame > MovementDetectionFrameWindow)
        {
            isSimpleManipulateStateOn = false;
            NoMovementDetectionFrame = 0;
        }
        else
        {
            isSimpleManipulateStateOn = true;
        }
    }

    public void RenderPassThrough()
    {
        PassThroughRenderer.enabled = true;

        AffordanceContourRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        
        if (PassThrough.isPrimaryHovered || PassThrough.isHovered) NoComplexManipulateStateFrame = 0;
        else NoComplexManipulateStateFrame++;

        if (NoComplexManipulateStateFrame > ComplexManipulateStateFrameWindow)
        {
            isHandInPassThroughArea = false;
            NoComplexManipulateStateFrame = 0;
        }
        else
        {
            isHandInPassThroughArea = true;
        }
    }
    
    public void RenderBoundaryBox(){
        BoundaryBoxRenderer.enabled = true;
        BoundaryBoxLineRenderer.enabled = true;
    }

    bool isTargetObjectMoving()
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