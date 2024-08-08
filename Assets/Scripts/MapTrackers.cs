using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.VR;

[System.Serializable]
public class TrackerInfo
{
    public string SerialNumber;   // Serial number of the tracker
    public GameObject Object; // The gameobject of Virtual Proxy associated with this tracker
}


public class MapTrackers : MonoBehaviour
{
    public List<TrackerInfo> TrackerVirtualProxyList = new List<TrackerInfo>();
    
    Dictionary<string, int> SerialNumberIDDictionary = new Dictionary<string, int>();
    GetTrackerSerialNumbers getTrackerSerialNumbers;

    void Start()
    {
        getTrackerSerialNumbers = FindObjectOfType<GetTrackerSerialNumbers>();
        getTrackerSerialNumbers.ListDeviceSerialNumbers(SerialNumberIDDictionary);

        MapTrackerToVirtualProxy(TrackerVirtualProxyList);
    }

    // Map tracker to Virtual Object. 
    void MapTrackerToVirtualProxy(List<TrackerInfo> TrackerVirtualProxyList){
        foreach (var mapping in TrackerVirtualProxyList){
            SteamVR_TrackedObject steamTrackedObject = mapping.Object.GetComponentInChildren<SteamVR_TrackedObject>();
            steamTrackedObject.index = (SteamVR_TrackedObject.EIndex)Enum.Parse(typeof(SteamVR_TrackedObject.EIndex), "Device" + SerialNumberIDDictionary[mapping.SerialNumber].ToString());
            Debug.Log($"Object Setting Step 2. Tracker Serial Number: {mapping.SerialNumber}, Assigned Object: {mapping.Object.name}");
        }
    }
}
