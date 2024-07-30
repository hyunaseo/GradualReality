using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTask1 : MonoBehaviour
{
    public GameObject explanation;

    SelectMainStudyMode.TaskMode taskMode;

    // Start is called before the first frame update
    void Start()
    {
        explanation.SetActive(false);

        taskMode = GameObject.Find("LIVE").GetComponent<SelectMainStudyMode>().taskMode;
    }

    // Update is called once per frame
    void Update()
    {
        if(taskMode != SelectMainStudyMode.TaskMode.Task1) return;

        else{
            explanation.SetActive(true);
        }
    }
}
