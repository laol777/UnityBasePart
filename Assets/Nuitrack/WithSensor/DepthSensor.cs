using UnityEngine;
using System.Collections;

public class DepthSensor : MonoBehaviour{

    static nuitrack.DepthSensor depthSensor = null;
    public static nuitrack.DepthSensor GetDepthSensor { get { return depthSensor; } }

    static nuitrack.DepthFrame depthFrame = null;
    public static nuitrack.DepthFrame DepthFrame { get { return depthFrame; } }

    DepthSensor()
    {
        depthSensor = nuitrack.DepthSensor.Create();
        depthSensor.OnUpdateEvent += HandleOnDepthUpdateEvent;
        Debug.Log("___DepthSensor.Init() success.");
    }

    public static int frame = 0;

    static void HandleOnDepthUpdateEvent(nuitrack.DepthFrame _depthFrame)
    {

        depthFrame = _depthFrame;
        ++frame;
    }
}
