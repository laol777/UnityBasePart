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

        frameData = new JSONData();
        frameData.rows = 2;
        frameData.cols = 2;
        frameData.depth = new int[] { 1, 2, 3, 4 };

        text.text = "press to write";

        userTracker = GameObject.FindObjectOfType<UserTracker>();
    }
    

    int frame = 0;

    JSONData prevFrameData;

    void Update () {
        if (isWrite)
        {

            if (UserTracker.frame != frame && DepthSensor.DepthFrame != null)
            {
                Debug.Log(frame);
                frame = UserTracker.frame;
                
                frameData = new JSONData();
                //if (frameData.rows != depthSensor.DepthFrame.Rows && frameData.cols != depthSensor.DepthFrame.Cols)
                {
                    frameData.depth = new int[DepthSensor.DepthFrame.Rows * DepthSensor.DepthFrame.Cols];
                    frameData.userTracker = new int[DepthSensor.DepthFrame.Rows * DepthSensor.DepthFrame.Cols];
                }

                frameData.rows = DepthSensor.DepthFrame.Rows;
                frameData.cols = DepthSensor.DepthFrame.Cols;

                for (int i = 0; i < frameData.rows; ++i)
                    for (int j = 0; j < frameData.cols; ++j)
                    {
                        //try
                        {
                            frameData.depth[i * frameData.cols + j] = DepthSensor.DepthFrame[i, j];
                            frameData.userTracker[i * frameData.cols + j] = userTracker.UserFrame[i, j];
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
