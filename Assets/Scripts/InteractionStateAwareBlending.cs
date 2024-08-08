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
    private GradualRealityManager GRManager; // GradualReality Manager to obtain parameters

    #region Interaction States

    // Enum to represent interaction states of GradualReailty 
    public enum InteractionState
    {
        Perceive,
        Approach,
        SimpleManipulate,
        ComplexManipulate,
        Avoid
    }

    public InteractionState currenInteractionState = InteractionState.Perceive;

    //Booleans for Interaction State Tracking
    bool isApproachStateOn = false;

    bool isSimpleManipulateStateOn = false;
    int noMovementDetectionFrame;
    int movementDetectionFrameWindow;

    bool isComplexManipulateStateOn = false;
    int noComplexManipulateStateFrame;
    int complexManipulateStateFrameWindow;
    bool isHandInPassThroughArea = false;

    [HideInInspector]
    public bool isNonTargetObject = false;

    #endregion


    #region Blending Methods

    // Affordance Contour 
    GameObject affordanceContour;
    MeshRenderer affordanceContourRenderer;

    // Boundary Box
    [HideInInspector]
    public GameObject boundaryBox;
    MeshRenderer boundaryBoxRenderer;
    LineRenderer boundaryBoxLineRenderer = new LineRenderer();

    // Pass-Through 
    InteractionBehaviour passThrough;
    GameObject passThroughEllipsoid;
    MeshRenderer passThroughRenderer;

    #endregion


    #region Trackers and Virtual Buttons

    GameObject tracker; // VIVE Tracker 
    MeshRenderer trackerRenderer;
    float trackingErrorThreshold; 

    InteractionButton interactionButton; // Interactable button provided by leap motion SDK 
    GameObject interactionButtonCore;
    MeshRenderer[] buttonRenderers = new MeshRenderer[2];

    #endregion


    Vector3 currentPosition;
    Vector3 priorPosition;

    void Start()
    {
        InitializeReferences();
        InitializeComponents();
        priorPosition = transform.position;
    }

    void Update()
    {

        UpdateThresholds();
        TrackInteractionState();
        RenderBlendingMethods();
        EnableTrackerRendering();
    }

    private void InitializeReferences()
    {
        GRManager = GameObject.FindObjectOfType<GradualRealityManager>();
        tracker = transform.GetChild(0).gameObject;
    }

    private void InitializeComponents()
    {
        // Find the corresponding blending methods 
        foreach (Transform child in tracker.transform)
        {
            if (child.tag == "AffordanceContour")
                affordanceContour = child.gameObject;
            if (child.tag == "BoundaryBox")
                boundaryBox = child.gameObject;
            if (child.tag == "PassThrough")
                passThrough = child.gameObject.GetComponent<InteractionBehaviour>();
        }

        // Affordance Contour setting
        affordanceContourRenderer = affordanceContour.transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
        affordanceContourRenderer.material = GRManager.AffordanceContourMaterial;

        // Boundary Box setting 
        boundaryBoxRenderer = boundaryBox.GetComponent<MeshRenderer>();
        boundaryBoxLineRenderer = boundaryBox.GetComponent<LineRenderer>();
        boundaryBoxRenderer.enabled = false;
        boundaryBoxLineRenderer.enabled = false;

        // Pass-Through setting 
        passThroughEllipsoid = passThrough.transform.GetChild(0).gameObject;
        passThroughRenderer = passThroughEllipsoid.GetComponent<MeshRenderer>();

        // Default mesh renderer Setting
        trackerRenderer = tracker.GetComponent<MeshRenderer>();
        // trackerRenderer.material = GRManager.TrackerMaterial;
        interactionButton = tracker.transform.Find("Button").Find("Cube UI Button").GetComponent<InteractionButton>();
        interactionButtonCore = interactionButton.transform.GetChild(0).GetChild(0).gameObject;
        buttonRenderers[0] = interactionButtonCore.GetComponent<MeshRenderer>();
    }

    private void UpdateThresholds()
    {
        // Retrieve threholds 
        trackingErrorThreshold = GRManager.TrackingErrorThreshold;
        movementDetectionFrameWindow = GRManager.MovementDetectionFrameWindow;
        complexManipulateStateFrameWindow = GRManager.ComplexManipulateStateFrameWindow;
    }

    private void TrackInteractionState()
    {
        // Update the boolean values for the interaction state based on the input data
        isApproachStateOn = passThrough.isPrimaryHovered || passThrough.isHovered;

        if (isTargetObjectMoving()) isSimpleManipulateStateOn = true;

        isComplexManipulateStateOn = interactionButton.isPressed;
        if (isComplexManipulateStateOn) isHandInPassThroughArea = true;

        // Set CurrentInteractionState 
        if (isComplexManipulateStateOn || isHandInPassThroughArea) currenInteractionState = InteractionState.ComplexManipulate;
        else if (isSimpleManipulateStateOn) currenInteractionState = InteractionState.SimpleManipulate;
        else if (isApproachStateOn) currenInteractionState = InteractionState.Approach;
        else if (isNonTargetObject) currenInteractionState = InteractionState.Avoid;
        else currenInteractionState = InteractionState.Perceive;
    }

    private void RenderBlendingMethods()
    {
        // Select blending method for CurrentInteractionState
        switch (currenInteractionState)
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
    }

    private void EnableTrackerRendering()
    {
        // if (GRManager.TrackerRenderingEnabled)
        // {
        //     trackerRenderer.enabled = true;
        // }
    }

    void RenderVirtualProxy()
    {
        affordanceContourRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        passThroughRenderer.enabled = false;

        if (!isNonTargetObject)
        {
            boundaryBoxRenderer.enabled = false;
            boundaryBoxLineRenderer.enabled = false;
        }
    }

    void RenderAffordanceContour_ApproachState()
    {
        affordanceContourRenderer.enabled = true;
        buttonRenderers[0].enabled = true;

        passThroughRenderer.enabled = false;
    }

    void RenderAffordanceContour_SimpleManipulateState()
    {
        RenderAffordanceContour_ApproachState();

        if (isTargetObjectMoving()) noMovementDetectionFrame = 0;
        else noMovementDetectionFrame++;

        if (noMovementDetectionFrame > movementDetectionFrameWindow)
        {
            isSimpleManipulateStateOn = false;
            noMovementDetectionFrame = 0;
        }
        else
        {
            isSimpleManipulateStateOn = true;
        }
    }

    public void RenderPassThrough()
    {
        passThroughRenderer.enabled = true;

        affordanceContourRenderer.enabled = false;
        buttonRenderers[0].enabled = false;

        if (passThrough.isPrimaryHovered || passThrough.isHovered)
        {
            noComplexManipulateStateFrame = 0;
        }
        else
        {
            noComplexManipulateStateFrame++;
        }

        if (noComplexManipulateStateFrame > complexManipulateStateFrameWindow)
        {
            isHandInPassThroughArea = false;
            noComplexManipulateStateFrame = 0;
        }
        else
        {
            isHandInPassThroughArea = true;
        }
    }

    public void RenderBoundaryBox()
    {
        boundaryBoxRenderer.enabled = true;
        boundaryBoxLineRenderer.enabled = true;
    }

    bool isTargetObjectMoving()
    {
        currentPosition = transform.position;

        if (Vector3.Distance(currentPosition, priorPosition) > trackingErrorThreshold)
        {
            priorPosition = currentPosition;
            return true;
        }

        else
        {
            return false;
        }
    }
}