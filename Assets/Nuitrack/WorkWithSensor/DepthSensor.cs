using UnityEngine;
using System.Collections;

public class DepthSensor : MonoBehaviour{

    static nuitrack.DepthSensor depthSensor = null;

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

        //bool test = true;
        //int ii = 0, jj = 0;
        //if (frame % 100 == 99)
        //{
        //    for (int i = 0; i < _depthFrame.Rows; ++i)
        //    {
        //        if (ii != 0 && jj != 0 && !test)
        //            break;
        //        for (int j = 0; j < _depthFrame.Cols; ++j)
        //        {
        //            if (_depthFrame[i, j] != depthFrame[i, j])
        //            {
        //                test = false;
        //                ii = i;
        //                jj = j;
        //                break;
        //            }
        //        }
        //    }
        //    Debug.Log(test + "_" + ii.ToString() + "_" + jj.ToString() + "_|_" + depthFrame[ii, jj] + "_" + _depthFrame[ii, jj]);
        //}
        depthFrame = _depthFrame;
        ++frame;
    }
}
