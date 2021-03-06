﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NuitrackManagerEmulation : MonoBehaviour {


    int frame = 0;
    public int Frame { get { return frame; } }

    public List<JSONData> data;

    int[,] depthFrame;
    public int[,] DepthFrame { get { return depthFrame; } }

    int[,] userFrame;
    public int[,] UserFrame { get { return userFrame; } }

    int[] usersID;
    public int[] UsersID { get { return usersID; } }

    public int XRes, YRes; //suppose that this size constant

    bool isSimulateStreamData = false;

    void Awake()
    {
        isSimulateStreamData = Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer;

        if (!isSimulateStreamData)
            Destroy(gameObject);

    }
    Vector3 tmp;
    void Start()
    {

        data = SerializeData.LoadJSON<JSONData>("testDepthData");

        XRes = data[0].XRes;
        YRes = data[0].YRes;

        depthFrame = new int[YRes, XRes];
        userFrame = new int[YRes, XRes];

        StartCoroutine(FrameCounter());

        Vector3 tmp = new Vector3();
    }

    [SerializeField]
    float timeOnFrame = 0.03f;

    IEnumerator FrameCounter()
    {
        yield return new WaitForSeconds(timeOnFrame);
        ++frame;
        if (frame >= data.Count)
            frame = 0;
        //frame = 7;
        UpdateDataFrame();

        StartCoroutine(FrameCounter());
    }

    int pastSentFrame = -1;

    void UpdateDataFrame()
    {
        depthFrame = UpdateDepthFrame();
        userFrame = UpdateUserTracker();
    }

    public nuitrack.SkeletonData GetSkeletonData()
    {
        return data[frame].skeletonData;
    }

    public nuitrack.Joint GetJoint(nuitrack.JointType jointType, int numberUser) //TODO add other type joint
    {
        //switch (jointType)
        //{
        //    case nuitrack.JointType.Head:
        //        return data[frame].skeletons[numberUser].joints[0];
        //    case nuitrack.JointType.RightWrist:
        //        return data[frame].skeletons[numberUser].joints[1];
        //    case nuitrack.JointType.RightElbow:
        //        return data[frame].skeletons[numberUser].joints[2];
        //}
        //return Vector3.zero;

        
        //tmp.x = data[frame].skeletonData.Skeletons[numberUser].GetJoint(jointType).Real.X;
        //tmp.y = data[frame].skeletonData.Skeletons[numberUser].GetJoint(jointType).Real.Y;
        //tmp.z = data[frame].skeletonData.Skeletons[numberUser].GetJoint(jointType).Real.Z;

        return data[frame].skeletonData.Skeletons[numberUser].GetJoint(jointType);
    } 

    public int[,] UpdateDepthFrame()
    {
        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                depthFrame[i, j] = data[frame].depth[i * XRes + j];
            }
        
        return depthFrame;
    }

    public int[,] UpdateUserTracker()
    {
        for (int i = 0; i < YRes; ++i)
            for (int j = 0; j < XRes; ++j)
            {
                userFrame[i, j] = data[frame].userTracker[i * XRes + j];
            }
        usersID = data[frame].userID;

        return userFrame;
    }



}
