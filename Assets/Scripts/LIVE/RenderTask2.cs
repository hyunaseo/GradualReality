using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTask2 : MonoBehaviour
{
    public enum CurrentTask{
        trial1, 
        trial2
    }

    public CurrentTask currentTask;

    public GameObject trial1;
    public GameObject trial2;
    public GameObject explanation;

    public GameObject wildExplanation;
    public GameObject wildTrial2;

    SelectMainStudyMode.TaskMode taskMode;

    // Start is called before the first frame update
    void Start()
    {
        trial1.SetActive(false);
        trial2.SetActive(false);
        explanation.SetActive(false);

        taskMode = GameObject.Find("LIVE").GetComponent<SelectMainStudyMode>().taskMode;
    }

    // Update is called once per frame
    void Update()
    {
        if(taskMode != SelectMainStudyMode.TaskMode.Task2) return;

        if(currentTask == CurrentTask.trial1){
            trial1.SetActive(true);
            trial2.SetActive(false);
            explanation.SetActive(true);
        }

        else if(currentTask == CurrentTask.trial2){
            trial1.SetActive(false);
            trial2.SetActive(true);
            explanation.SetActive(true);
        }

        if(taskMode == SelectMainStudyMode.TaskMode.InTheWild){
            trial1.SetActive(true);
            wildTrial2.SetActive(true);
            wildExplanation.SetActive(true);
        }
    }
}
