using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymBarState : MonoBehaviour
{
    public MeshRenderer barbellHoleRenderer;
    public MeshRenderer barRightRenderer;
    public MeshRenderer barHandleRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(barbellHoleRenderer.enabled == true)
            barRightRenderer.enabled = true;
        else    
            barRightRenderer.enabled = false;

        if(barHandleRenderer.enabled == true){
            barRightRenderer.enabled = false;
            barbellHoleRenderer.enabled = false;            
        }
    }
}
