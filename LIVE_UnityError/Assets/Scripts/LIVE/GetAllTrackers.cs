using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAllTrackers : MonoBehaviour
{
    public List<GameObject> allTrackers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<transform.childCount; i++){
            allTrackers.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
