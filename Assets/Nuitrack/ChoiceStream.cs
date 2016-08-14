using UnityEngine;
using System.Collections;
using System;

public class ChoiceStream : MonoBehaviour {

    [SerializeField]NuitrackManagerEmulation nuitrackManagerEmulation;

    DepthSensor depthSensor;
    UserTracker userTracker;


    void Start()
    {
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        userTracker = GameObject.FindObjectOfType<UserTracker>();
    }

    int[,] ConvertNuitrackObjToIntArray<T>(T array) where T : nuitrack.Frame<ushort>
    {

        nuitrack.UserFrame tt = new nuitrack.UserFrame();

        int YRes = DepthSensor.DepthFrame.Rows;
        int XRes = DepthSensor.DepthFrame.Cols;
        int[,] resault = new int[YRes, XRes];
        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                //resault[i, j] = (int)Convert.ChangeType(array[i, j], typeof(int));
                //resault[i, j] = array[i * XRes + j];
                resault[i, j] = array[i * XRes + j];
            }
        return resault;
    }

    public int[,] GetDepthFrame()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return nuitrackManagerEmulation.DepthFrame;
        }
        else
        {
            if (DepthSensor.DepthFrame != null)
            {
                return ConvertNuitrackObjToIntArray<nuitrack.DepthFrame>(DepthSensor.DepthFrame);
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
            return nuitrackManagerEmulation.UserFrame;
        }
        else
        {
            if (UserTracker.UserFrame != null)
            {
                return ConvertNuitrackObjToIntArray<nuitrack.UserFrame>(UserTracker.UserFrame);
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
