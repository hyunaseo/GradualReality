  using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class RenderBaseline2 : MonoBehaviour
{
    public GameObject virtualProxy;
    
    GameObject needFindingStudyManager;
    SelectBaselineMode selectBaselineMode;
    SelectBaselineMode.BaselineMode currentMode;

    GameObject tracker;
    GameObject button;
    InteractionButton interactionButton;
    InteractionBehaviour passThroughNode;
    GameObject passThroughSphere;

    private InteractionBehaviour _intObj;

    MeshRenderer trackerRenderer;
    MeshRenderer buttonRenderer;
    MeshRenderer virtualProxyRenderer;
    MeshRenderer passThroughRenderer;

    public bool isPassThroughOn = false;
    // bool isMoving = true;
    bool isHovered = false;

    Vector3 previousPosition;
    
    void Start()
    {
        // initialize the game objects
        needFindingStudyManager = GameObject.Find("Need Finding Study Manager");
        button = transform.GetChild(0).gameObject;
        interactionButton = transform.parent.parent.gameObject.GetComponent<InteractionButton>();
        tracker = interactionButton.transform.parent.parent.gameObject;
        passThroughSphere = tracker.transform.Find("PassThroughSphere Node").GetChild(0).gameObject;
        passThroughNode = tracker.transform.Find("PassThroughSphere Node").gameObject.GetComponent<InteractionBehaviour>();

        _intObj = GetComponent<InteractionBehaviour>();

        // initialize the renderers
        virtualProxyRenderer = virtualProxy.GetComponent<MeshRenderer>();
        virtualProxyRenderer.enabled = true;
        trackerRenderer = tracker.GetComponent<MeshRenderer>();
        trackerRenderer.enabled = true;
        buttonRenderer = button.GetComponent<MeshRenderer>();
        buttonRenderer.enabled = true;

        passThroughRenderer = passThroughSphere.GetComponent<MeshRenderer>();
        passThroughRenderer.enabled = false;

        selectBaselineMode = needFindingStudyManager.gameObject.GetComponentInChildren<SelectBaselineMode>();

        previousPosition = virtualProxy.transform.position;
    }   


    void Update(){
        isHovered = passThroughNode.isHovered || passThroughNode.isPrimaryHovered;

        if(isHovered || isPassThroughOn){
            buttonRenderer.enabled = true;
        }
        else{
            buttonRenderer.enabled = false;
        }
    }

    public void TriggerPassThrough()
    {
        currentMode = selectBaselineMode.baselineMode;

        if(currentMode != SelectBaselineMode.BaselineMode.Baseline2)
            return;

        // pressed -> pass-through on & virtual proxy off
        if (!isPassThroughOn)
        {
            passThroughRenderer.enabled = true;
            virtualProxyRenderer.enabled = false;
            trackerRenderer.enabled = false;

            isPassThroughOn = true;
            Debug.Log("PassThrough On");
        }

        // pressed -> pass-through off & virtual proxy on
        else if (isPassThroughOn)
        {
            passThroughRenderer.enabled = false;
            virtualProxyRenderer.enabled = true;
            trackerRenderer.enabled = true;

            isPassThroughOn = false;
            Debug.Log("PassThrough Off");
        }
    }
}
