using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

public class MapTrackers_MagicGarden: MonoBehaviour
{
    SelectMainStudyMode.TaskMode taskMode; 

    Dictionary<string, int> serial_id_Dict = new Dictionary<string, int>();
    Dictionary<string, string> serial_object_Dict = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        taskMode = GameObject.Find("LIVE").GetComponent<SelectMainStudyMode>().taskMode;
        Debug.Log("task: " + taskMode);
        
        serial_object_Dict.Clear();
        serial_object_Dict.Add("LHR-B7B0A3B7", "Pot");
        serial_object_Dict.Add("LHR-D6811C64", "Spray");

        // serial_object_Dict.Add("LHR-FD7E5D75", "GreenBox");
        serial_object_Dict.Add("LHR-F70A47CF", "WateringCan");
        // serial_object_Dict.Add("LHR-B7B0A3B7", "BlueBox");
        // serial_object_Dict.Add("LHR-D6811C64", "MugComplex");
        // serial_object_Dict.Add("LHR-35A8CE9F", "Fan");
        // serial_object_Dict.Add("LHR-4A7CD0ED", "Cactus");

        ListDevices(serial_id_Dict);
        MapTrackerToObject(serial_id_Dict, serial_object_Dict);
    }


    void ListDevices(Dictionary<string, int> serial_id_Dict){

        for(int i=0; i < SteamVR.connected.Length; ++i){
            ETrackedPropertyError error = new ETrackedPropertyError();
            StringBuilder sb = new StringBuilder();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_SerialNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
            var SerialNumber = sb.ToString();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_ModelNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
            var ModelNumber = sb.ToString();

            Debug.Log("SerialNum type: " + SerialNumber.GetType().Name);
            if (SerialNumber.Length > 0 || ModelNumber.Length > 0){
                Debug.Log("Device " + i.ToString() + " = " + SerialNumber + " | " + ModelNumber);
                serial_id_Dict.Add(SerialNumber, i);
            }
        }
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
