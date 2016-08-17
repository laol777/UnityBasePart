using UnityEngine;
using System.Collections;
using System;

public class ChoiceStream : MonoBehaviour {

    [SerializeField]NuitrackManagerEmulation nuitrackManagerEmulation;

    DepthSensor depthSensor;
    UserTracker userTracker;

    public int[,] depthFrame;
    public int[,] userFrame;

    void Start()
    {
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        userTracker = GameObject.FindObjectOfType<UserTracker>();

        depthFrame = new int[60, 80]; //TODO set correct size
        userFrame = new int[60, 80];
    }

    void ConvertNuitrackObjToIntArray<T>(T array, ref int[,] resault) where T : nuitrack.Frame<ushort>
    {
        //return;
        //nuitrack.UserFrame tt = new nuitrack.UserFrame();

        int YRes = depthSensor.DepthFrame.Rows;
        int XRes = depthSensor.DepthFrame.Cols;

        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                resault[i, j] = array[i * XRes + j];
            }
    }


    public int[,] GetDepthFrame()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            depthFrame = nuitrackManagerEmulation.DepthFrame;
            return depthFrame;
        }
        else
        {
            if (depthSensor.DepthFrame != null)
            {
                ConvertNuitrackObjToIntArray<nuitrack.DepthFrame>(depthSensor.DepthFrame, ref depthFrame);
                return depthFrame;
            }
            else
            {
                return null; 
            }
        }
    }

    public int[,] GetUserFrame()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            userFrame = nuitrackManagerEmulation.UserFrame;
            return userFrame;
        }
        else
        {
            if (userTracker.UserFrame != null)
            {
                ConvertNuitrackObjToIntArray<nuitrack.UserFrame>(userTracker.UserFrame, ref userFrame);
                return userFrame;
            }
            else
            {
                return null;
            }
        }
    }

    public int[] GetUserID()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return nuitrackManagerEmulation.UsersID;
        }
        else if (userTracker.UserFrame != null)
        {
            int[] nuitrackId = new int[userTracker.UserFrame.NumUsers];

            for (int i = 0; i < userTracker.UserFrame.NumUsers; ++i)
                nuitrackId[i] = userTracker.UserFrame.Users[i].ID;

            return nuitrackId;
        }
        else
        {
            return null;
        }
        
    }



    public int Frame
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return nuitrackManagerEmulation.Frame;
            }
            else
            {
                return depthSensor.frame;
            }
        }
        
    }

    public int XRes
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return nuitrackManagerEmulation.XRes;
            }
            else
            {
                return depthSensor.DepthFrame.Cols;
            }
        }
    }
    public int YRes
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return nuitrackManagerEmulation.YRes;
            }
            else
            {
                return depthSensor.DepthFrame.Rows;
            }
        }
    }
}
