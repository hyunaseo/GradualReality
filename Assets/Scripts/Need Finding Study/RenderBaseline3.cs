using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RenderBaseline3 : MonoBehaviour
{
    GameObject needFindingStudyManager;
    SelectBaselineMode selectBaselineMode;
    SelectBaselineMode.BaselineMode currentMode;
    
    // Hands Related
    InteractionManager interactionManager;
    InteractionHand leftHand;
    InteractionHand rightHand;

    GameObject tracker;
    public GameObject virtualProxy;
    GameObject passThroughSphere;

    private InteractionBehaviour _intObj;
    
    MeshRenderer trackerRenderer;
    MeshRenderer virtualProxyRenderer;
    MeshRenderer passThroughRenderer;

    bool isPassThroughOn = false;

    bool isTrackedLeft = false;
    bool isTrackedRight = false;

    bool interactionWithLeft = false;
    bool interactionWithRight = false;

    void Start()
    {
        // initialize the game objects
        needFindingStudyManager = GameObject.Find("Need Finding Study Manager");

        interactionManager = GameObject.Find("Interaction Manager").GetComponent<InteractionManager>();
        leftHand = GameObject.Find("Interaction Hand (Left)").GetComponent<InteractionHand>();
        rightHand = GameObject.Find("Interaction Hand (Right)").GetComponent<InteractionHand>();

        tracker = transform.parent.parent.gameObject;
        passThroughSphere = transform.gameObject;

        _intObj = GetComponent<InteractionBehaviour>();

        // initialize renderers
        virtualProxyRenderer = virtualProxy.GetComponent<MeshRenderer>();
        virtualProxyRenderer.enabled = true;
        trackerRenderer = tracker.GetComponent<MeshRenderer>();
        trackerRenderer.enabled = true;
        
        passThroughRenderer = passThroughSphere.GetComponent<MeshRenderer>();
        passThroughRenderer.enabled = false;

        selectBaselineMode = needFindingStudyManager.gameObject.GetComponentInChildren<SelectBaselineMode>();
    }

    public void PassThroughOn()
    {
        if(!CheckBaselineMode()) return;

        // hovering -> pass-through on & virtual proxy off
        if (!isPassThroughOn)
        {
            passThroughRenderer.enabled = true;
            virtualProxyRenderer.enabled = false;
            trackerRenderer.enabled = false;

            isPassThroughOn = true;
            CheckHoveringSide();

            Debug.Log("PassThrough On");
        }
    }

    public void PassThroughOff()
    {
        if (!CheckBaselineMode()) return;

        CheckTracking();

        // hovering X -> pass-through off & virtual proxy on
        if (interactionWithLeft && isTrackedLeft && isPassThroughOn)
        {
            passThroughRenderer.enabled = false;
            virtualProxyRenderer.enabled = true;
            trackerRenderer.enabled = true;

            isPassThroughOn = false;
            Debug.Log("PassThrough Off");
        }

        else if (interactionWithRight && isTrackedRight && isPassThroughOn)
        {
            passThroughRenderer.enabled = false;
            virtualProxyRenderer.enabled = true;
            trackerRenderer.enabled = true;

            isPassThroughOn = false;
            Debug.Log("PassThrough Off");
        }
    }

    public void PassThroughStay()
    {
        if (!CheckBaselineMode()) return;

        if (isPassThroughOn){
            passThroughRenderer.enabled = true;
            virtualProxyRenderer.enabled = false;
            trackerRenderer.enabled = false;

            isPassThroughOn = true;
            Debug.Log("PassThrough Stay");
        }
    }

    bool CheckBaselineMode(){
        currentMode = selectBaselineMode.baselineMode;

        if (currentMode != SelectBaselineMode.BaselineMode.Baseline3)
            return false;

        return true;
    }

    void CheckHoveringSide(){
        if (leftHand.hoverEnabled && leftHand.hoveredObjects.Count > 0) 
            interactionWithLeft = true;
        else interactionWithLeft = false;

        if (rightHand.hoverEnabled && rightHand.hoveredObjects.Count > 0) 
            interactionWithRight = true;
        else interactionWithRight = false;
    }

    void CheckTracking(){
        if (leftHand.isBeingMoved) isTrackedLeft = true;
        else isTrackedLeft = false;

        if (rightHand.isBeingMoved) isTrackedRight = true;
        else isTrackedRight = false;
    }
}
