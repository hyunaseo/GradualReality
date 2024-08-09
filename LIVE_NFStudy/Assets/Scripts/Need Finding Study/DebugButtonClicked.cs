using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtonClicked : MonoBehaviour
{
    private InteractionBehaviour _intObj;

    int timestampt = 0;

    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();

    }

    void Update(){
        if(_intObj.isHovered){
            Debug.Log("Hovered: " + timestampt);
            timestampt++;
        }

        if(_intObj is InteractionButton){
            Debug.Log("This is interacton button");
        }

        if(_intObj is InteractionButton && (_intObj as InteractionButton).isPressed){
            Debug.Log("Pressed: " + timestampt);
            timestampt++;
        }
    } 
}
