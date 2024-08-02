using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RenderPipeline_SpaceHouse : MonoBehaviour
{
    public enum InteractionState
    {
        Perceive,
        Grab,
        Move,
        Manipulate
    }

    //Default
    SelectMainStudyMode.BaselineMode mainStudyMode;
    GameObject tracker;
    InteractionButton interactionButton;
    GameObject buttonCore;
    GameObject buttonBody;

    //Related Mesh Renderer
    MeshRenderer trackerRenderer;
    MeshRenderer[] buttonRenderers = new MeshRenderer[2];

    //Leap Motion Interactable
    InteractionBehaviour passThroughNode;

    //Rendering
    GameObject virtualProxy;
    GameObject affordance;
    [HideInInspector]
    public GameObject boundingBox;
    GameObject passThroughSphere;

    //Related Mesh Renderer
    MeshRenderer virtualProxyRenderer;
    MeshRenderer affordanceRenderer;
    MeshRenderer boundingBoxRenderer;
    MeshRenderer passThroughRenderer;

    //Interaction State 
    public InteractionState currenInteractionState = InteractionState.Perceive;

    //Booleans for Interaction State Tracking
    bool isButtonPressed = false;
    bool isHandManipulating = false;
    bool isHovered = false;
    public bool isContact = false;
    bool isTrackerPosChanged = false;
    public bool isMoving = false;
    public bool isObstacle = false;

    //Set Thresholds
    GradualRealityManager initParams;

    //Variables for manipulation state
    public int handNotInCount = 0;
    int manipulateWindow = 90;

    //Variables for move state
    List<GameObject> allTrackers = new List<GameObject>();
    float trackerMovingErrorThr = 0.005f;
    Vector3 currentPos;
    Vector3 priorPos;
    public int notMoveCount = 0;
    int moveWindow = 30;
    LineRenderer lineRenderer = new LineRenderer();

    //For tutorial mode
    SelectMainStudyMode.TaskMode taskMode;

    //For Cinema Scenario
    GameObject popcornContainer;
    RenderPipeline.InteractionState containerState;

    void Start()
    {
        //Default GO Setting
        mainStudyMode = GameObject.Find("GradualReality").GetComponent<SelectMainStudyMode>().baselineMode;
        taskMode = GameObject.Find("GradualReality").GetComponent<SelectMainStudyMode>().taskMode;
        tracker = transform.GetChild(0).GetChild(0).gameObject;
        interactionButton = tracker.transform.Find("Button").Find("Cube UI Button").GetComponent<InteractionButton>();
        //buttonBody = interactionButton.transform.GetChild(0).gameObject;
        buttonCore = interactionButton.transform.GetChild(0).GetChild(0).gameObject;

        //Default Mesh Renderer Setting
        trackerRenderer = tracker.GetComponent<MeshRenderer>();
        buttonRenderers[0] = buttonCore.GetComponent<MeshRenderer>();
        //buttonRenderers[1] = buttonBody.GetComponent<MeshRenderer>();

        //Leap Motion Interactable Setting
        passThroughNode = tracker.transform.Find("PassThroughSphere Node").GetComponent<InteractionBehaviour>();

        //Rendering
        foreach (Transform child in tracker.transform)
        {
            if (child.tag == "VirtualProxy")
                virtualProxy = child.gameObject;
            else if (child.tag == "AffordanceContour")
                affordance = child.gameObject;
            else if (child.tag == "BoundaryBox")
                boundingBox = child.gameObject;
        }

        passThroughSphere = passThroughNode.transform.GetChild(0).gameObject;

        //Rendering Mesh Renderer Setting
        virtualProxyRenderer = virtualProxy.GetComponent<MeshRenderer>();
        affordanceRenderer = affordance.transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
        boundingBoxRenderer = boundingBox.GetComponent<MeshRenderer>();
        lineRenderer = boundingBox.GetComponent<LineRenderer>();
        passThroughRenderer = passThroughSphere.GetComponent<MeshRenderer>();

        boundingBoxRenderer.enabled = false;
        lineRenderer.enabled = false;

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            allTrackers.Add(transform.parent.GetChild(i).gameObject);
        }

        priorPos = transform.position;

        initParams = GameObject.FindObjectOfType<GradualRealityManager>();
        trackerMovingErrorThr = initParams.TrackingErrorThreshold;
        moveWindow = initParams.MovementDetectionFrameWindow;
        manipulateWindow = initParams.ComplexManipulateStateFrameWindow;

        popcornContainer = GameObject.Find("PopcornContainer");
        containerState = popcornContainer.GetComponent<RenderPipeline>().CurrenInteractionState;

    }

    void Update()
    {
        // Mode: Always-on Virtual Proxy
        if(mainStudyMode == SelectMainStudyMode.BaselineMode.VirtualProxy){
            currenInteractionState = InteractionState.Perceive;
        }

        // Mode: Automatic Trigger 
        else if(mainStudyMode == SelectMainStudyMode.BaselineMode.BaselineAuto){
            isHovered = passThroughNode.isPrimaryHovered || passThroughNode.isHovered;
            if(isHovered) currenInteractionState = InteractionState.Manipulate;
            else currenInteractionState = InteractionState.Perceive;
        }

        // Mode: LIVE or Manual Trigger
        else {
            isButtonPressed = interactionButton.isPressed;
            if (isButtonPressed) isHandManipulating = true;
            isHovered = passThroughNode.isPrimaryHovered || passThroughNode.isHovered;
            isTrackerPosChanged = isTrackerMoving();
            if (isTrackerPosChanged) isMoving = true;
            TrackInteractionState();
        }

        containerState = popcornContainer.GetComponent<RenderPipeline>().CurrenInteractionState;

        if(containerState == RenderPipeline.InteractionState.Perceive)
            currenInteractionState = InteractionState.Perceive;
        else if(containerState == RenderPipeline.InteractionState.Approach)
            currenInteractionState = InteractionState.Grab;
        else if(containerState == RenderPipeline.InteractionState.Avoid)
            currenInteractionState = InteractionState.Grab;
        else currenInteractionState = InteractionState.Perceive;

        switch (currenInteractionState)
        {
            case InteractionState.Perceive:
                RenderPerceive();
                break;

            case InteractionState.Grab:
                RenderGrab();
                break;

            case InteractionState.Move:
                RenderMove();
                break;

            case InteractionState.Manipulate:
                RenderManipulate();
                break;
        }

        if(isObstacle) {
            boundingBoxRenderer.enabled = true;
            lineRenderer.enabled = true;
        }
        else{
            boundingBoxRenderer.enabled = false;
            lineRenderer.enabled = false;
        }

        trackerRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
    }

    void TrackInteractionState()
    {
        if (isButtonPressed || isHandManipulating)
        {
            currenInteractionState = InteractionState.Manipulate;
        }
        else
        {
            if (isMoving)
            {
                currenInteractionState = InteractionState.Move;
            }
            else if (isHovered)
            {
                currenInteractionState = InteractionState.Grab;
            }
            else
            {
                currenInteractionState = InteractionState.Perceive;
            }
        }

        // if(this.gameObject.name == "GreenBox" | this.gameObject.name == "YellowBox" || this.gameObject.name == "Container1"){
        //     affordanceRenderer.enabled = false;
        //     buttonRenderers[0].enabled = false;
        // }
    }

    void RenderPerceive()
    {
        trackerRenderer.enabled = true;
        
        virtualProxyRenderer.enabled = false;
        affordanceRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        passThroughRenderer.enabled = false;
    }

    void RenderGrab()
    {
        trackerRenderer.enabled = true;
        virtualProxyRenderer.enabled = false;
        affordanceRenderer.enabled = true;
        buttonRenderers[0].enabled = true;
        passThroughRenderer.enabled = false;
    }

    void RenderMove()
    {
        //Set Renderer Enable Status
        trackerRenderer.enabled = true;
        virtualProxyRenderer.enabled = true;

        if (mainStudyMode == SelectMainStudyMode.BaselineMode.LIVE)
            affordanceRenderer.enabled = true;
        else
            affordanceRenderer.enabled = false;

        buttonRenderers[0].enabled = true;
        passThroughRenderer.enabled = false;

        //Count Moving Frame
        if (isTrackerMoving()) notMoveCount = 0;
        else notMoveCount++;

        //If not moving, turn off Box
        if (notMoveCount > moveWindow)
        {
            isMoving = false;
            notMoveCount = 0;
        }
        else
        {
            isMoving = true;
        }
    }

    public void RenderManipulate()
    {
        //Set Renderer Enable Status
        passThroughRenderer.enabled = true;

        trackerRenderer.enabled = false;
        virtualProxyRenderer.enabled = false;
        affordanceRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        
        //Count Hand In Frame #
        if (isHandInSphere()) handNotInCount = 0;
        else handNotInCount++;

        //If Hand is not in, turn off PT
        if (handNotInCount > manipulateWindow)
        {
            isHandManipulating = false;
            handNotInCount = 0;
        }
        else
        {
            isHandManipulating = true;
        }
    }

    bool isHandInSphere()
    {
        return isHovered;
    }

    bool isTrackerMoving()
    {
        currentPos = transform.position;
        if (Vector3.Distance(currentPos, priorPos) > trackerMovingErrorThr)
        {
            priorPos = currentPos;
            return true;
        }

        else
        {
            return false;
        }
    }
}