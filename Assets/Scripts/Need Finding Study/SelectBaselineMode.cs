using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBaselineMode : MonoBehaviour
{
    public enum BaselineMode{
        Baseline1, 
        Baseline2,
        Baseline3,
        Main
    }

    public BaselineMode baselineMode;
    
    List<GameObject> PassThroughSphereList;
    List<GameObject> ButtonList;

    InteractionManager interactionManager;

    public float baseline2HoverThreshold = 0.18f;
    public float baseline3HoverThreshold = 0.12f;
    
    void Start()
    {
        interactionManager = (InteractionManager)FindObjectOfType(typeof(InteractionManager));
        if(interactionManager == null) Debug.Log("interaction  manager is not found!");

        PassThroughSphereList = GetPassThroughShperes();
        ButtonList = GetButtons();
    }

    void Update()
    {
        switch(baselineMode){
            case BaselineMode.Baseline1:
                Debug.Log("Current Mode: Baseline1 (VP only)");
                OffVirtualButton(ButtonList);
                break;

            case BaselineMode.Baseline2:
                Debug.Log("Current Mode: Baseline2 (Manual adaptation)");
                OnVirtualButton(ButtonList);
                interactionManager.hoverActivationRadius = baseline2HoverThreshold;
                break;

            case BaselineMode.Baseline3:
                Debug.Log("Current Mode: Baseline3 (Auto apdaptation)");
                OffVirtualButton(ButtonList);
                interactionManager.hoverActivationRadius = baseline3HoverThreshold;
                break;

            case BaselineMode.Main:
                OnVirtualButton(ButtonList);
                break;
        }
    }

    void OnVirtualButton(List<GameObject> ButtonList){
        foreach(GameObject button in ButtonList){
            button.SetActive(true);
        }
    }

    void OffVirtualButton(List<GameObject> ButtonList)
    {
        foreach (GameObject button in ButtonList)
        {
            button.SetActive(false);
        }
    }

    List<GameObject> GetPassThroughShperes(){
        List<GameObject> PassThroughSphereList = new List<GameObject>();
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        
        foreach(GameObject go in gos){
            if(go.layer == 6){
                PassThroughSphereList.Add(go);
            }
        }

        return PassThroughSphereList;
    }

    List<GameObject> GetButtons(){
        List<GameObject> ButtonList = new List<GameObject>();
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        foreach(GameObject go in gos){
            if(go.name == "Button" || go.name == "Button (Box)"){
                ButtonList.Add(go);
                go.SetActive(false);
            }
        }

        return ButtonList;
    }
}
