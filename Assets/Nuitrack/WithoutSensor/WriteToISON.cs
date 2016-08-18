using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class WriteToISON : MonoBehaviour {

    public bool addData;

    float timeBetweenFrame = 0.05f;

    JSONData frameData;
    List<JSONData> serializeData;

    NuitrackManager nuitrackManager;

    DepthSensor depthSensor;
    UserTracker userTracker;
    SkeletonTracker skeletonTracker;

    void Start()
    {
        serializeData = new List<JSONData>();

        text.text = "press to write";

        nuitrackManager = GameObject.FindObjectOfType<NuitrackManager>();

        userTracker = GameObject.FindObjectOfType<UserTracker>();
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        skeletonTracker = GameObject.FindObjectOfType<SkeletonTracker>();
    }



    int frame = 0;

    JSONData prevFrameData;

    void Update()
    {
        if (isWrite)
        {

            if (userTracker.frame != frame && depthSensor.DepthFrame != null && skeletonTracker.SkeletonData != null)
            {
                frame = userTracker.frame;
                Debug.Log("Writing frame " + frame.ToString());
                

                frameData = new JSONData();

                                                                                               
                frameData.YRes = depthSensor.DepthFrame.Rows;
                frameData.XRes = depthSensor.DepthFrame.Cols;

                frameData.depth = new int[depthSensor.DepthFrame.Rows * depthSensor.DepthFrame.Cols]; 
                frameData.userTracker = new int[depthSensor.DepthFrame.Rows * depthSensor.DepthFrame.Cols];
                for (int i = 0; i < frameData.YRes; ++i)
                    for (int j = 0; j < frameData.XRes; ++j)
                    {
                        frameData.depth[i * frameData.XRes + j] = depthSensor.DepthFrame[i, j];
                        frameData.userTracker[i * frameData.XRes + j] = userTracker.UserFrame[i, j];
                    }

                
                frameData.userID = nuitrackManager.GetUserID();


                frameData.skeletons = new Skeleton[skeletonTracker.SkeletonData.NumUsers];

                for (int i = 0; i < skeletonTracker.SkeletonData.NumUsers; ++i)
                {
                    frameData.skeletons[i] = new Skeleton();
                    frameData.skeletons[i].ID = frameData.userID[i];
                    frameData.skeletons[i].joints = new Vector3[3];


                    GetVector3Joint(skeletonTracker.SkeletonData.GetSkeletonByID(frameData.userID[i]).GetJoint(nuitrack.JointType.Head));
                    frameData.skeletons[i].joints[0] = GetVector3Joint(skeletonTracker.SkeletonData.GetSkeletonByID(frameData.userID[i]).GetJoint(nuitrack.JointType.Head));
                    frameData.skeletons[i].joints[1] = GetVector3Joint(skeletonTracker.SkeletonData.GetSkeletonByID(frameData.userID[i]).GetJoint(nuitrack.JointType.RightWrist));
                    frameData.skeletons[i].joints[2] = GetVector3Joint(skeletonTracker.SkeletonData.GetSkeletonByID(frameData.userID[i]).GetJoint(nuitrack.JointType.RightElbow));
                }

                serializeData.Add(frameData);



            }
        }
    }

    bool isWrite = false;
    public Text text;

    public void StartWrite()
    {
        if (!isWrite)
        {
            isWrite = true;
            text.text = "stop write";
        }
        else
        {
            isWrite = false;
            text.text = "press to write";
            writeToJson();
        }
    }

    //skeletonTracker.SkeletonData.GetSkeletonByID(NuitrackManager.currentUser)

    Vector3 GetVector3Joint(nuitrack.Joint joint)
    {
        Vector3 returnedJoint = new Vector3();

        returnedJoint.x = joint.Real.X;
        returnedJoint.y = joint.Real.Y;
        returnedJoint.z = joint.Real.Z;

        return returnedJoint;
    }

    void writeToJson()
    {
        SerializeData.SaveJSON(serializeData, "testDepthData");
    }
}
