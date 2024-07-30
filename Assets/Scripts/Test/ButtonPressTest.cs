using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ButtonPressTest : MonoBehaviour
{
    private InteractionButton _intObj;
    int timestampt = 0; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(_intObj.isPressed){
        //     Debug.Log("Pressed!");
        //     var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     sphere.transform.position = new Vector3(0, 0.5f + 0.1f*timestampt, 0);
        //     timestampt++;
        // }
    }

    public void DrawSphere(){
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.transform.position = new Vector3(0.5f + 0.1f * timestampt, 0, 0.0f);
        timestampt++;
    }
}
