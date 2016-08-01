using UnityEngine;
using System.Collections;

public class DepthSensor : MonoBehaviour{

    static nuitrack.DepthSensor depthSensor = null;

    static nuitrack.DepthFrame depthFrame = null;

    public void Init()
    {
        depthSensor = nuitrack.DepthSensor.Create();
        depthSensor.OnUpdateEvent += HandleOnDepthUpdateEvent;
    }

    static void HandleOnDepthUpdateEvent(nuitrack.DepthFrame _depthFrame)
    {
        depthFrame = _depthFrame;
    }
}
