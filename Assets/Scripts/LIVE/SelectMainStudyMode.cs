using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMainStudyMode : MonoBehaviour
{
    public enum TaskMode{
        Tutorial,
        Task1,
        Task2,
        Task3,
        InTheWild
    }

    public enum BaselineMode{
        LIVE,
        VirtualProxy,
        BaselineManual,
        BaselineAuto
    }

    public BaselineMode baselineMode;
    public TaskMode taskMode;
    
    List<GameObject> PassThroughSphereList;
    List<GameObject> ButtonList;

    InteractionManager interactionManager;
    
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
            case BaselineMode.LIVE:
                OnVirtualButton(ButtonList);
                break;

            case BaselineMode.VirtualProxy:
                OffVirtualButton(ButtonList);
                break;
            
            case BaselineMode.BaselineManual:
                OnVirtualButton(ButtonList);
                break;

            case BaselineMode.BaselineAuto:
                OffVirtualButton(ButtonList);
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
