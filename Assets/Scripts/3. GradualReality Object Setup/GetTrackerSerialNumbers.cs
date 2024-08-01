using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

/// <summary>
/// This script is to check your VIVE tracker 3.0's serial number. 
/// </summary>
public class GetTrackerSerialNumbers : MonoBehaviour
{
    public Dictionary<string, int> serial_id_Dict = new Dictionary<string, int>();
    
    void Start()
    {
        ListDeviceSerialNumbers(serial_id_Dict);
    }

    public void ListDeviceSerialNumbers(Dictionary<string, int> serial_id_Dict)
    {

        for (int i = 0; i < SteamVR.connected.Length; ++i)
        {
            ETrackedPropertyError error = new ETrackedPropertyError();
            StringBuilder sb = new StringBuilder();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_SerialNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
            var SerialNumber = sb.ToString();

            OpenVR.System.GetStringTrackedDeviceProperty((uint)i, ETrackedDeviceProperty.Prop_ModelNumber_String, sb, OpenVR.k_unMaxPropertyStringSize, ref error);
            var ModelNumber = sb.ToString();

            if (SerialNumber.Length > 0 || ModelNumber == "VIVE Tracker 3.0")
            {
                Debug.Log("Device " + i.ToString() + " = " + SerialNumber + " | " + ModelNumber);
                serial_id_Dict.Add(SerialNumber, i);
            }
        }
    }
}
