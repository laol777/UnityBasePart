using UnityEngine;
using System.Collections;
using System;

public class ChoiceStream : MonoBehaviour {
    //when simultaneous transmission of data sensor is, out of sync when receiving the data.
    [SerializeField]NuitrackManagerEmulation nuitrackManagerEmulation;

    DepthSensor depthSensor;
    UserTracker userTracker;
    SkeletonTracker skeletonTracker;

    public int[,] depthFrame;
    public int[,] userFrame;

    //

    bool isSimulateStreamData = false;
    
    void Start()
    {
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        userTracker = GameObject.FindObjectOfType<UserTracker>();
        skeletonTracker = GameObject.FindObjectOfType<SkeletonTracker>();

        depthFrame = new int[60, 80]; //TODO set correct size
        userFrame = new int[60, 80];

        isSimulateStreamData = Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer;
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
        if (isSimulateStreamData)
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
        if (isSimulateStreamData)
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

    int[] userID;
    public int[] GetSegmentationID()
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
        if (GetSegmentationID() != null && numberUser <= GetSegmentationID().Length)
            return GetSegmentationID()[GetSegmentationID().Length - numberUser];
        else
            return 0;
    }

    public nuitrack.Skeleton[] GetSkeletons() //add to simulation
    {
        return skeletonTracker.SkeletonData.Skeletons;
    }

    public Vector3 GetJoint(nuitrack.JointType jointType, int numberUser) //number user >= 1
    {
        if (isSimulateStreamData)
        {
            if (GetSegmentationID() != null && numberUser <= GetSegmentationID().Length)
                return nuitrackManagerEmulation.GetJoint(jointType, GetSegmentationID().Length - numberUser);
            else
                return Vector3.zero;
        }
        else
        {
            if (skeletonTracker.SkeletonData != null
                && skeletonTracker.SkeletonData.NumUsers != 0
                && numberUser <= skeletonTracker.SkeletonData.NumUsers
                )
            {
                return GetVector3Joint(skeletonTracker.SkeletonData.Skeletons[skeletonTracker.SkeletonData.NumUsers - numberUser].GetJoint(jointType));
            }
            else
            {
                return Vector3.zero;
            }
        }
  
    }
}
