using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoContours : MonoBehaviour
{
    public MeshRenderer palmRenderer;
    public MeshRenderer buttonRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (palmRenderer.enabled) buttonRenderer.enabled=true;
        else buttonRenderer.enabled=false;
    }
}
