using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

public class MapTrackers : MonoBehaviour
{
    SelectMainStudyMode.TaskMode taskMode; 
    GetTrackerSerialNumbers getTrackerSerialNumbers;
    Dictionary<string, int> serial_id_Dict = new Dictionary<string, int>();
    Dictionary<string, string> serial_object_Dict = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        getTrackerSerialNumbers = FindObjectOfType<GetTrackerSerialNumbers>();
        taskMode = GameObject.Find("GradualReality").GetComponent<SelectMainStudyMode>().taskMode;
        Debug.Log("task: " + taskMode);
        
        if(taskMode == SelectMainStudyMode.TaskMode.Tutorial){
            serial_object_Dict.Clear();
            serial_object_Dict.Add("LHR-FD7E5D75", "GreenBox");
            serial_object_Dict.Add("LHR-D6811C64", "MugComplex");
        }
        
        else if(taskMode == SelectMainStudyMode.TaskMode.Task1){
            serial_object_Dict.Clear();
            serial_object_Dict.Add("LHR-D6811C64", "MugSimple");
            serial_object_Dict.Add("LHR-FD7E5D75", "GreenBox");
            serial_object_Dict.Add("LHR-4A7CD0ED", "BlueBox");
            serial_object_Dict.Add("LHR-F70A47CF", "YellowBox");
        }

        else if(taskMode == SelectMainStudyMode.TaskMode.Task2){
            serial_object_Dict.Clear(); 
            serial_object_Dict.Add("LHR-D6811C64", "MugComplex");
            serial_object_Dict.Add("LHR-35A8CE9F", "Container1");
            serial_object_Dict.Add("LHR-4A7CD0ED", "Bottle");
        }

        else if (taskMode == SelectMainStudyMode.TaskMode.Task3)
        {
            serial_object_Dict.Clear();
            serial_object_Dict.Add("LHR-D6811C64", "MugComplex");
            serial_object_Dict.Add("LHR-4A7CD0ED", "Bottle");
        }

        else if(taskMode == SelectMainStudyMode.TaskMode.InTheWild){
            serial_object_Dict.Clear();
            serial_object_Dict.Add("LHR-FD7E5D75", "GreenBox");
            serial_object_Dict.Add("LHR-F70A47CF", "YellowBox");
            // serial_object_Dict.Add("LHR-B7B0A3B7", "BlueBox");
            serial_object_Dict.Add("LHR-D6811C64", "MugComplex");
            serial_object_Dict.Add("LHR-35A8CE9F", "Container1");
            serial_object_Dict.Add("LHR-4A7CD0ED", "Bottle");
        }

        getTrackerSerialNumbers.ListDeviceSerialNumbers(serial_id_Dict);
        MapTrackerToObject(serial_id_Dict, serial_object_Dict);
    }

    void MapTrackerToObject(Dictionary<string, int> serial_id_Dict, Dictionary<string, string> serial_object_Dict){
        foreach(string serialNum in serial_id_Dict.Keys){
            if(serial_object_Dict.ContainsKey(serialNum)){
                GameObject currentObject = GameObject.Find(serial_object_Dict[serialNum]);
                Debug.Log("Current object: " + currentObject.name);
                SteamVR_TrackedObject steamTrackedObject = currentObject.gameObject.GetComponentInChildren<SteamVR_TrackedObject>();
                steamTrackedObject.index = (SteamVR_TrackedObject.EIndex)Enum.Parse(typeof(SteamVR_TrackedObject.EIndex), "Device" + serial_id_Dict[serialNum].ToString());
            }
        }
    }
}
