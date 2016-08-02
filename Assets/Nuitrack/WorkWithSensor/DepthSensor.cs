using UnityEngine;
using System.Collections;

public class DepthSensor : MonoBehaviour{

    static nuitrack.DepthSensor depthSensor = null;

    static nuitrack.DepthFrame depthFrame = null;
    public nuitrack.DepthFrame DepthFrame { get { return depthFrame; } set { depthFrame = value; } }


    public void Init()
    {
        depthSensor = nuitrack.DepthSensor.Create();
        depthSensor.OnUpdateEvent += HandleOnDepthUpdateEvent;
        Debug.Log("DepthSensor.Init() success.");
    }

    static void HandleOnDepthUpdateEvent(nuitrack.DepthFrame _depthFrame)
    {
        depthFrame = _depthFrame;
    }
}
