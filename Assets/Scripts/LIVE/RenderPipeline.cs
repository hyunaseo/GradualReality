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
    public enum InteractionState
    {
        Perceive,
        Approach,
        ComplexManipulate, 
        Avoid
    }

    public bool isCinema = false;
    public bool isBar = false;
    public bool isBarBell = false;

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
    GradualRealityManager gradualRealityManager;

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

    void Start()
    {
        //Initial game object settings 
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
            else if (child.tag == "Affordance")
                affordance = child.gameObject;
            else if (child.tag == "BoundingBox")
                boundingBox = child.gameObject;

            if (gameObject.name == "CokeCan"){
                virtualProxy = GameObject.Find("SciFi_Battery");
            }
        }

        passThroughSphere = passThroughNode.transform.GetChild(0).gameObject;

        //Rendering Mesh Renderer Setting
        // virtualProxyRenderer = virtualProxy.GetComponent<MeshRenderer>();
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

        gradualRealityManager = GameObject.FindObjectOfType<GradualRealityManager>();
        trackerMovingErrorThr = gradualRealityManager.TrackingErrorThreshold;
        moveWindow = gradualRealityManager.MovementDetectionFrameWindow;
        manipulateWindow = gradualRealityManager.ComplexManipulateStateFrameWindow;
        Debug.Log("HYUNA: Manipulate Window" + gradualRealityManager.ComplexManipulateStateFrameWindow);
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
            if(isHovered) currenInteractionState = InteractionState.ComplexManipulate;
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

        switch (currenInteractionState)
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

        if(isObstacle) {
            boundingBoxRenderer.enabled = true;
            lineRenderer.enabled = true;
        }
        // else if (isBarBell){
        //     boundingBoxRenderer.enabled = false;
        //     lineRenderer.enabled = false;
        //     trackerRenderer.enabled = false;
        //     buttonRenderers[0].enabled = false;
        // }
        // else if (isBar){
        //     trackerRenderer.enabled = false;
        //     buttonRenderers[0].enabled = false;
        // }
        // else if (!isObstacle){
        //     boundingBoxRenderer.enabled = false;
        //     lineRenderer.enabled = false;
        // }

        // if (isBarBell && currenInteractionState != InteractionState.Grab)
        // {
        //     buttonRenderers[0].enabled = false;
        //     boundingBoxRenderer.enabled = false;
        // }

        // if (isCinema) {
        //     trackerRenderer.enabled = false;
        //     lineRenderer.enabled = false;
        //     boundingBoxRenderer.enabled = false;

        //     // if(currenInteractionState == InteractionState.Manipulate){
        //     //     virtualProxyRenderer.enabled = false;
        //     // }
        // }

        trackerRenderer.enabled = true;
    }

    void TrackInteractionState()
    {
        if (isButtonPressed || isHandManipulating)
        {
            currenInteractionState = InteractionState.ComplexManipulate;
        }
        else
        {
            if (isMoving)
            {
                currenInteractionState = InteractionState.Avoid;
            }
            else if (isHovered)
            {
                currenInteractionState = InteractionState.Approach;
            }
            else
            {
                currenInteractionState = InteractionState.Perceive;
            }
        }
    }

    void RenderPerceive()
    {
        trackerRenderer.enabled = false;
        // virtualProxyRenderer.enabled = true;

        affordanceRenderer.enabled = false;
        buttonRenderers[0].enabled = false;
        passThroughRenderer.enabled = false;

        if (!isObstacle)
        {
            boundingBoxRenderer.enabled = false;
            lineRenderer.enabled = false;
        }
    }

    void RenderGrab()
    {
        trackerRenderer.enabled = false;
        // virtualProxyRenderer.enabled = true;

        if (mainStudyMode == SelectMainStudyMode.BaselineMode.LIVE){
            if (taskMode == SelectMainStudyMode.TaskMode.Tutorial && transform.name == "GreenBox")
            {
                Debug.Log("Green out");
                affordanceRenderer.enabled = false;
            }
            else affordanceRenderer.enabled = true;
        }
        else
            affordanceRenderer.enabled = false;


        buttonRenderers[0].enabled = true;

        passThroughRenderer.enabled = false;
    }

    void RenderMove()
    {
        //Set Renderer Enable Status
        trackerRenderer.enabled = false;
        // virtualProxyRenderer.enabled = true;

        if (mainStudyMode == SelectMainStudyMode.BaselineMode.LIVE)
            affordanceRenderer.enabled = true;
        else
            affordanceRenderer.enabled = false;

        if (taskMode == SelectMainStudyMode.TaskMode.Tutorial && transform.name == "GreenBox"){
            affordanceRenderer.enabled = false;
        }

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
        // virtualProxyRenderer.enabled = false;
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