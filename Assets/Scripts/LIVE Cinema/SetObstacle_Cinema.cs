using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObstacle_Cinema : MonoBehaviour
{
    Dictionary<RenderPipeline, Vector3> posDict = new Dictionary<RenderPipeline, Vector3>();
    
    //Set Thresholds
    InitParams initParams;
    float obstacleDistThr = 0.3f;
    SelectMainStudyMode.BaselineMode mainStudyMode;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<transform.childCount; i++){
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf && child.GetComponent<RenderPipeline>()!=null) posDict.Add(child.GetComponent<RenderPipeline>(), child.position);
        }

        initParams = GameObject.FindObjectOfType<InitParams>();
        obstacleDistThr = initParams.obstacleDistThr;

        mainStudyMode = GameObject.Find("LIVE").GetComponent<SelectMainStudyMode>().baselineMode;
    }

    // Update is called once per frame
    void Update()
    {
        if(mainStudyMode != SelectMainStudyMode.BaselineMode.LIVE) return;
        
        int moveCount = 0;

        foreach(var pair in posDict){
            if(pair.Key.currenInteractionState == RenderPipeline.InteractionState.Move)
                moveCount++;
        }

        if(moveCount==0) {
            foreach (var pair in posDict)
            {
                pair.Key.isObstacle = false;
            }
            return;
        }
        
        foreach (var pair in posDict){
            foreach (var pair2 in posDict){
                
                if(pair.Key.currenInteractionState == RenderPipeline.InteractionState.Move 
                   && pair2.Key.currenInteractionState != RenderPipeline.InteractionState.Move
                   && pair2.Key.currenInteractionState != RenderPipeline.InteractionState.Manipulate) {
                    
                    // float distance = Vector3.Distance(pair.Key.boundingBox.transform.position, pair2.Key.boundingBox.transform.position);
                    float distance = MeasureShortestDistance(pair.Key.boundingBox, pair2.Key.boundingBox);
                    if(distance < obstacleDistThr){
                        pair2.Key.isObstacle = true;
                    }
                    else{
                        pair2.Key.isObstacle = false;
                    }
                } 
            }
        }
    }

    float MeasureShortestDistance(GameObject targetObject, GameObject obstacle){
        Vector3 targetCenter = targetObject.transform.position;
        Vector3 obstacleCenter = obstacle.transform.position;
        BoxCollider targetCollider = targetObject.GetComponent<BoxCollider>();
        BoxCollider obstacleCollider = obstacle.GetComponent<BoxCollider>();

        float centerToCenter = Vector3.Distance(targetCenter, obstacleCenter);
        Vector3 tmpPoint1 = obstacleCollider.ClosestPoint(targetCenter);
        Vector3 tmpPoint2 = targetCollider.ClosestPoint(obstacleCenter);

        float distance = Vector3.Distance(tmpPoint1, tmpPoint2);
        return distance;
    }
}
