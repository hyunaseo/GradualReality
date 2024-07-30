using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetScreenSize : MonoBehaviour
{
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Scren Width : " + Screen.width);
        Debug.Log("Scren Height : " + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
