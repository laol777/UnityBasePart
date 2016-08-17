using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class TestJSON : MonoBehaviour {

    public bool addData;

    float timeBetweenFrame = 0.05f;

    JSONData frameData;
    List<JSONData> serializeData;

    DepthSensor depthSensor;
    UserTracker userTracker;

    void Start()
    {
        //Debug.Log(Directory.GetCurrentDirectory());

        serializeData = new List<JSONData>();


        text.text = "press to write";

        userTracker = GameObject.FindObjectOfType<UserTracker>();
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
    }
    

    int frame = 0;

    JSONData prevFrameData;

    void Update () {
        if (isWrite)
        {

            if (userTracker.frame != frame && depthSensor.DepthFrame != null)
            {
                Debug.Log(frame);
                frame = userTracker.frame;
                
                frameData = new JSONData();
                //if (frameData.rows != depthSensor.DepthFrame.Rows && frameData.cols != depthSensor.DepthFrame.Cols)
                {
                    frameData.depth = new int[depthSensor.DepthFrame.Rows * depthSensor.DepthFrame.Cols];
                    frameData.userTracker = new int[depthSensor.DepthFrame.Rows * depthSensor.DepthFrame.Cols];
                }

                frameData.YRes = depthSensor.DepthFrame.Rows;
                frameData.XRes = depthSensor.DepthFrame.Cols;

                for (int j = 0; j < frameData.XRes; ++j)
                    for (int i = 0; i < frameData.YRes; ++i) 
                    {
                        //try
                        {
                            frameData.depth[i * frameData.XRes + j] = depthSensor.DepthFrame[i, j];
                            frameData.userTracker[i * frameData.XRes + j] = userTracker.UserFrame[i, j];
                        }
                        //catch
                        //{
                        //    Debug.Log(i.ToString() + " " + j.ToString());
                        //}
                    }


                //try
                //{
                //    bool test = true;
                //    for (int i = 0; i < frameData.depth.Length; ++i)
                //    {
                //        if (frameData.depth[i] != prevFrameData.depth[i])
                //            test = false;
                //    }
                //    Debug.Log(test);
                //}
                //catch
                //{ }

                //prevFrameData = frameData;

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

    void writeToJson()
    {
        SerializeData.SaveJSON(serializeData, "testDepthData");
    }

}
