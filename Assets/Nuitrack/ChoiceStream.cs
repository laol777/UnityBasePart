using UnityEngine;
using System.Collections;


public class ChoiceStream : MonoBehaviour {

    [SerializeField]NuitrackManagerEmulation nuitrackManagerEmulation;

    DepthSensor depthSensor;
    UserTracker userTracker;


    void Start()
    {
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        userTracker = GameObject.FindObjectOfType<UserTracker>();
    }

    public int[,] GetDepthFrame()
    {
        //if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return nuitrackManagerEmulation.GetDepthFrame();
        }
        //else
        //{
        //    return DepthSensor.DepthFrame;
        //}
    }

    public int[,] GetUserFrame()
    {
        return nuitrackManagerEmulation.GetUserFrame();
    }

    public int Frame { get { return nuitrackManagerEmulation.Frame; } }

    public int XRes { get { return nuitrackManagerEmulation.XRes; } }
    public int YRes { get { return nuitrackManagerEmulation.YRes; } }
}
