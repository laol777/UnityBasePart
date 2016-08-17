using UnityEngine;
using System.Collections;

public class DepthSensor : MonoBehaviour{

    nuitrack.DepthSensor depthSensor = null;
    public nuitrack.DepthSensor GetDepthSensor { get { return depthSensor; } }

    nuitrack.DepthFrame depthFrame = null;
    public nuitrack.DepthFrame DepthFrame { get { return depthFrame; } }

    DepthSensor()
    {
        depthSensor = nuitrack.DepthSensor.Create();
        depthSensor.OnUpdateEvent += HandleOnDepthUpdateEvent;
        Debug.Log("___DepthSensor.Init() success.");
    }

    public int frame = 0;

    void HandleOnDepthUpdateEvent(nuitrack.DepthFrame _depthFrame)
    {
        depthFrame = _depthFrame;
        ++frame;
    }
}
