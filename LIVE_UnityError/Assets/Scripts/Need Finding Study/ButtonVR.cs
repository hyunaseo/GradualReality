using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;

    GameObject presser;
    bool isPressed;

    void Start()
    {
        isPressed = false;    
    }

    private void OnTriggerEnter(Collider other){
        if(!isPressed){
            button.transform.localPosition = new Vector3(0, 0.005f, 0);
            presser = other.gameObject;
            onPress.Invoke();

            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other){
        if(other == presser){
            button.transform.localPosition = new Vector3(0, 0.01f, 0); //original position
            onRelease.Invoke();

            isPressed = false;
        }
    }

    public void SpawnSphere(){
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        sphere.transform.localPosition = new Vector3(0, 1, 2);
        sphere.AddComponent<Rigidbody>();
    }
}
