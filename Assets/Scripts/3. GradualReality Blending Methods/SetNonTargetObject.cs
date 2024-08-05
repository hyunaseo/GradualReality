using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNonTargetObject : MonoBehaviour
{
    Dictionary<InteractionStateAwareBlending, Vector3> ObjectPositionDictionary = new Dictionary<InteractionStateAwareBlending, Vector3>();

    GradualRealityManager GRManager;
    float AvoidStateDistance;

    void Start()
    {
        for(int i=0; i<transform.childCount; i++){
            Transform child = transform.GetChild(i);
            if(child.gameObject.activeSelf) ObjectPositionDictionary.Add(child.GetComponent<InteractionStateAwareBlending>(), child.position);
        }

        GRManager = GameObject.FindObjectOfType<GradualRealityManager>();
        AvoidStateDistance = GRManager.AvoidStateDistance;
    }

    // Update is called once per frame
    void Update()
    {
        int SimpleManipulateStateFrame = 0;

        foreach(var pair in ObjectPositionDictionary){
            if(pair.Key.currenInteractionState == InteractionStateAwareBlending.InteractionState.SimpleManipulate)
                SimpleManipulateStateFrame++;
        }

        if(SimpleManipulateStateFrame==0) {
            foreach (var pair in ObjectPositionDictionary)
            {
                pair.Key.isNonTargetObject = false;
            }
            return;
        }

        foreach (var pair in ObjectPositionDictionary){
            foreach (var pair2 in ObjectPositionDictionary){
                
                if(pair.Key.currenInteractionState == InteractionStateAwareBlending.InteractionState.SimpleManipulate
                   && pair2.Key.currenInteractionState != InteractionStateAwareBlending.InteractionState.SimpleManipulate
                   && pair2.Key.currenInteractionState != InteractionStateAwareBlending.InteractionState.ComplexManipulate) {
                    
                    float distance = MeasureShortestDistance(pair.Key.boundaryBox, pair2.Key.boundaryBox);
                    if(distance < AvoidStateDistance){
                        pair2.Key.isNonTargetObject = true;
                    }
                    else{
                        pair2.Key.isNonTargetObject = false;
                    }
                } 
            }
        }
    }

    float MeasureShortestDistance(GameObject targetObject, GameObject nonTargetObject){
        Vector3 targetCenter = targetObject.transform.position;
        Vector3 obstacleCenter = nonTargetObject.transform.position;
        BoxCollider targetCollider = targetObject.GetComponent<BoxCollider>();
        BoxCollider obstacleCollider = nonTargetObject.GetComponent<BoxCollider>();

        float centerToCenter = Vector3.Distance(targetCenter, obstacleCenter);
        Vector3 tmpPoint1 = obstacleCollider.ClosestPoint(targetCenter);
        Vector3 tmpPoint2 = targetCollider.ClosestPoint(obstacleCenter);

        float distance = Vector3.Distance(tmpPoint1, tmpPoint2);
        return distance;
    }
}
