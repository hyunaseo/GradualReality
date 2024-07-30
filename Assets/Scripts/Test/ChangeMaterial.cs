using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Material SphereMaterial;
    int tick;

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Material from the renderer of the gameobject
        SphereMaterial = GetComponent<Renderer>().material;
        Debug.Log("Materials: " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);

        tick = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (tick > 300)
        {
            SphereMaterial.color = Color.green;
        }

        else
        {
            SphereMaterial.color = Color.blue;
        }

        tick++;

    }
}
