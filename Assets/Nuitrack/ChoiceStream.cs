﻿using UnityEngine;
using System.Collections;
using System;


public class ChoiceStream : MonoBehaviour {
    [SerializeField]NuitrackManagerEmulation nuitrackManagerEmulation;

    DepthSensor depthSensor;
    UserTracker userTracker;
    SkeletonTracker skeletonTracker;

    public int[,] depthFrame;
    public int[,] userFrame;

    bool isSimulateStreamData = false;


    nuitrack.Joint zeroJoint;

    void Start()
    {

        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        userTracker = GameObject.FindObjectOfType<UserTracker>();
        skeletonTracker = GameObject.FindObjectOfType<SkeletonTracker>();

        depthFrame = new int[60, 80]; //TODO set correct size
        userFrame = new int[60, 80];

        isSimulateStreamData = Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer;

        zeroJoint = new nuitrack.Joint();
    }



    int[,] ConvertNuitrackObjToIntArray<T>(T array, int[,] resault) where T : nuitrack.Frame<ushort>
    {
        int YRes = depthSensor.DepthFrame.Rows;
        int XRes = depthSensor.DepthFrame.Cols;

        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                resault[i, j] = array[i * XRes + j];
            }

        return resault;
    }


    public int[,] GetDepthFrame()
    {
        if (isSimulateStreamData)
        {
            depthFrame = nuitrackManagerEmulation.DepthFrame;
            return depthFrame;
        }
        else
        {
            if (depthSensor.DepthFrame != null)
            {
                return ConvertNuitrackObjToIntArray<nuitrack.DepthFrame>(depthSensor.DepthFrame, depthFrame);
            }
            else
            {
                return null;
            }
        }
    }

    public int[,] GetUserFrame()
    {
        if (isSimulateStreamData)
        {
            userFrame = nuitrackManagerEmulation.UserFrame;
            return userFrame;
        }
        else
        {
            if (userTracker.UserFrame != null)
            {
                return ConvertNuitrackObjToIntArray<nuitrack.UserFrame>(userTracker.UserFrame, userFrame);
            }
            else
            {
                return null;
            }
        }
    }

    int[] userID;
    public int[] GetArrayIDSegmentation()
    {
        if (isSimulateStreamData)
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
    int GetValueForSpecificPlatform(int emulationValue, int sensorValue)
    {
        if (isSimulateStreamData)
        {
            return emulationValue;
        }
        else
        {
            return sensorValue;
        }
    }

    //public int Frame { get { return GetValueForSpecificPlatform(nuitrackManagerEmulation.Frame, depthSensor.frame); } }  
    //public int XRes { get { return GetValueForSpecificPlatform(nuitrackManagerEmulation.XRes, depthSensor.DepthFrame.Cols); } }
    //public int YRes { get { return GetValueForSpecificPlatform(nuitrackManagerEmulation.YRes, depthSensor.DepthFrame.Rows); } }

    public int Frame
    {
        get
        {
            if (isSimulateStreamData)
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
            if (isSimulateStreamData)
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
            if (isSimulateStreamData)
            {
                return nuitrackManagerEmulation.YRes;
            }
            else
            {
                return depthSensor.DepthFrame.Rows;
            }
        }
    }

    public nuitrack.SkeletonData GetSkeletonData()
    {
        if (isSimulateStreamData)
        {
            return nuitrackManagerEmulation.GetSkeletonData();
        }
        else
        {
            return skeletonTracker.SkeletonData;
        }

    }

    Vector3 GetVector3Joint(nuitrack.Joint joint)
    {
        Vector3 returnedJoint = new Vector3();

        returnedJoint.x = joint.Real.X;
        returnedJoint.y = joint.Real.Y;
        returnedJoint.z = joint.Real.Z;

        return returnedJoint;
    }

    public int GetUserID(int numberUser)
    {
        if (GetArrayIDSegmentation() != null && numberUser <= GetArrayIDSegmentation().Length)
            return GetArrayIDSegmentation()[numberUser - 1];
        else
            return 0;
    }

    public nuitrack.Skeleton[] GetSkeletons() //TODO: add simulation type
    {
        return skeletonTracker.SkeletonData.Skeletons;
    }

    public nuitrack.Joint GetJoint(nuitrack.JointType jointType, int numberUser) //number user >= 1
    {
        if (isSimulateStreamData)
        {
            if (GetArrayIDSegmentation() != null && numberUser <= GetArrayIDSegmentation().Length)
                return nuitrackManagerEmulation.GetJoint(jointType, numberUser - 1);
            else
                return zeroJoint;
        }
        else
        {
            if (skeletonTracker.SkeletonData != null
                && skeletonTracker.SkeletonData.NumUsers != 0
                && numberUser <= skeletonTracker.SkeletonData.NumUsers
                )
            {
                return skeletonTracker.SkeletonData.Skeletons[numberUser - 1].GetJoint(jointType);
            }
            else
            {
                return zeroJoint;
            }
        }
  
    }
}
