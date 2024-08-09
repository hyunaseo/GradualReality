using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RenderPipeline : MonoBehaviour
{
    public enum InteractionState{
        Perceive,
        Grab,
        Move,
        Manipulate
    }

    //Default
    GameObject tracker;
    InteractionButton interactionButton;
    GameObject button;

    //Related Mesh Renderer
    MeshRenderer trackerRenderer;
    MeshRenderer buttonRenderer;

    //Leap Motion Interactable
    InteractionBehaviour passThroughNode;

    //Rendering
    GameObject virtualProxy;
    GameObject affordance;
    GameObject boundingBox;
    GameObject passThroughSphere;

    //Related Mesh Renderer
    MeshRenderer virtualProxyRenderer;
    MeshRenderer affordanceRenderer;
    MeshRenderer[] boundingBoxRenderers;
    MeshRenderer passThroughRenderer;

    //Interaction State 
    public InteractionState currenInteractionState = InteractionState.Perceive;

    //Booleans for Interaction State Tracking
    bool isButtonPressed = false;
    bool isHandManipulating = false;
    bool isHovered = false;
    public bool isMoving = false;

    //Variables for manipulation state
    public int handNotInCount = 0;
    int passThroughOffThreshold = 90;

    //Variables for move state
    List<GameObject> allTrackers = new List<GameObject>();
    float moveThreshold = 0.003f;
    Vector3 currentPos;
    Vector3 priorPos;
    public int notMoveCount = 0;
    int moveCountThreshould = 90;

    void Start(){
        //Default GO Setting
        tracker = transform.GetChild(0).GetChild(0).gameObject;
        interactionButton = tracker.transform.Find("Button").GetChild(0).GetComponent<InteractionButton>();
        button = interactionButton.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        //Default Mesh Renderer Setting
        trackerRenderer = tracker.GetComponent<MeshRenderer>();
        buttonRenderer = button.GetComponent<MeshRenderer>();

        //Leap Motion Interactable Setting
        passThroughNode = tracker.transform.Find("PassThroughSphere Node").GetComponent<InteractionBehaviour>();

        //Rendering
        foreach (Transform child in tracker.transform){
            if(child.tag == "VirtualProxy")
                virtualProxy = child.gameObject;
            else if(child.tag == "Affordance")
                affordance = child.gameObject;
            else if(child.tag == "BoundingBox")
                boundingBox = child.gameObject;
        }

        passThroughSphere = passThroughNode.transform.GetChild(0).gameObject;

        //Rendering Mesh Renderer Setting
        virtualProxyRenderer = virtualProxy.GetComponent<MeshRenderer>();
        affordanceRenderer = affordance.transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
        boundingBoxRenderers = boundingBox.GetComponentsInChildren<MeshRenderer>();
        passThroughRenderer = passThroughSphere.GetComponent<MeshRenderer>();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            allTrackers.Add(transform.parent.GetChild(i).gameObject);
        }

        priorPos = transform.position;
    }

    void Update(){
        isButtonPressed = interactionButton.isPressed;
        if(isButtonPressed) isHandManipulating = true;
        isHovered = passThroughNode.isHovered || passThroughNode.isPrimaryHovered;
        isMoving = isTrackerMoving();
        TrackInteractionState();
       
        switch(currenInteractionState){
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
    }

    void TrackInteractionState(){
        if(isButtonPressed || isHandManipulating){
            currenInteractionState = InteractionState.Manipulate;
        }
        else{
            if(isMoving){
                currenInteractionState = InteractionState.Move;
            }
            else if(!isMoving && isHovered){
                currenInteractionState = InteractionState.Grab;
            }
            else{
                currenInteractionState = InteractionState.Perceive;
            }
        }
    }

    void RenderPerceive(){
        trackerRenderer.enabled = true;
        virtualProxyRenderer.enabled = true;

        affordanceRenderer.enabled = false;
        buttonRenderer.enabled = false;
        for(int i=0; i<boundingBoxRenderers.Length; i++) {boundingBoxRenderers[i].enabled = false;}
        passThroughRenderer.enabled = false;
    }

    void RenderGrab(){
        trackerRenderer.enabled = true;
        virtualProxyRenderer.enabled = true;
        affordanceRenderer.enabled = true;
        buttonRenderer.enabled = true;

        for (int i = 0; i < boundingBoxRenderers.Length; i++) { boundingBoxRenderers[i].enabled = false; }
        passThroughRenderer.enabled = false;
    }

    void RenderMove(){
        trackerRenderer.enabled = true;
        virtualProxyRenderer.enabled = true;
        affordanceRenderer.enabled = true;
        buttonRenderer.enabled = true;
        for (int i = 0; i < boundingBoxRenderers.Length; i++) { boundingBoxRenderers[i].enabled = true; }
        
        passThroughRenderer.enabled = false;

        if(isTrackerMoving()) notMoveCount = 0;
        else notMoveCount++;

        if(notMoveCount > moveCountThreshould){
            isMoving = false;
            notMoveCount = 0;
        }
        else{
            isMoving = true;
        }
    }

    public void RenderManipulate(){
        //Set Renderer Enable Status
        passThroughRenderer.enabled = true;

        trackerRenderer.enabled = false;
        virtualProxyRenderer.enabled = false;
        affordanceRenderer.enabled = false;
        buttonRenderer.enabled = false;
        for (int i = 0; i < boundingBoxRenderers.Length; i++) { boundingBoxRenderers[i].enabled = false; }
    
        //Count Hand In Frame #
        if(isHandInSphere()) handNotInCount = 0;
        else handNotInCount++;

        //If Hand is not in, turn off PT
        if(handNotInCount > passThroughOffThreshold) {
            Debug.Log("Is manipulating: " + isButtonPressed);
            isHandManipulating = false;
            handNotInCount = 0;
        }
        else {
            isHandManipulating = true;
        }
    }

    bool isHandInSphere(){
        return isHovered;
    }

    bool isTrackerMoving(){
        currentPos = transform.position;
        if(Vector3.Distance(currentPos, priorPos) > moveThreshold){
            priorPos = currentPos;
            return true;
        }

        else {
            return false;
        }
    }
}
