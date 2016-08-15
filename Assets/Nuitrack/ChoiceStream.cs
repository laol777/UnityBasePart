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

        int YRes = DepthSensor.DepthFrame.Rows;
        int XRes = DepthSensor.DepthFrame.Cols;

        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                resault[i, j] = array[i * XRes + j];
            }
    }

    void TestConvertNuitrackObjToIntArray<T>(T[,] array, ref T[,] resault) where T : struct
    {

        //nuitrack.UserFrame tt = new nuitrack.UserFrame();
        //return;
        int YRes = 60;
        int XRes = 80;

        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                resault[i, j] = array[i, j];
            }
    }

    public int[,] GetDepthFrame()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            //depthFrame = nuitrackManagerEmulation.DepthFrame;
            TestConvertNuitrackObjToIntArray<int>(nuitrackManagerEmulation.DepthFrame, ref depthFrame);
            return depthFrame;
        }
        else
        {
            if (DepthSensor.DepthFrame != null)
            {
                ConvertNuitrackObjToIntArray<nuitrack.DepthFrame>(DepthSensor.DepthFrame, ref depthFrame);
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
            if (UserTracker.UserFrame != null)
            {
                ConvertNuitrackObjToIntArray<nuitrack.UserFrame>(UserTracker.UserFrame, ref userFrame);
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
        else if (UserTracker.UserFrame != null)
        {
            int[] nuitrackId = new int[UserTracker.UserFrame.NumUsers];

            for (int i = 0; i < UserTracker.UserFrame.NumUsers; ++i)
                nuitrackId[i] = UserTracker.UserFrame.Users[i].ID;

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
                return DepthSensor.frame;
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
                return DepthSensor.DepthFrame.Cols;
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
                return DepthSensor.DepthFrame.Rows;
            }
        }
    }
}
