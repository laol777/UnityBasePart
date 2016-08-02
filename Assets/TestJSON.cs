using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

public class TestJSON : MonoBehaviour {

    public bool addData;

    float timeBetweenFrame = 0.05f;

    JSONData frameData;
    List<JSONData> serializeData;

    DepthSensor depthSensor;

    void Start()
    {
        serializeData = new List<JSONData>();

        depthSensor = GameObject.FindObjectOfType<DepthSensor>();

        frameData = new JSONData();
        frameData.rows = 2;
        frameData.cols = 2;
        frameData.depth = new int[] { 1, 2, 3, 4 };
    }

    int write = 0;

    void Update () {

        timeBetweenFrame += Time.deltaTime;

        if (timeBetweenFrame > 0.05f)
        {
            addData = true;
            timeBetweenFrame -= 0.05f;
        }


        if (addData && depthSensor != null && depthSensor.DepthFrame != null)
        {
            addData = false;

            if (frameData.rows != depthSensor.DepthFrame.Rows && frameData.cols != depthSensor.DepthFrame.Cols)
            {
                frameData.depth = new int[depthSensor.DepthFrame.Rows * depthSensor.DepthFrame.Cols];
            }

            frameData.rows = depthSensor.DepthFrame.Rows;
            frameData.cols = depthSensor.DepthFrame.Cols;

            for (int i = 0; i < frameData.rows; ++i)
                for (int j = 0; j < frameData.cols; ++j)
                {
                    frameData.depth[i * frameData.cols + j] = depthSensor.DepthFrame[i, j];
                }

            int a = 3;

            if (write < a)
            {
                serializeData.Add(frameData);
                Debug.Log("1");
            }
            if (write == a)
            {
                SerializeData.SaveJSON(serializeData, "testNewClass");

                List<JSONData> test2 = SerializeData.LoadJSON<JSONData>("testNewClass");
                Debug.Log(test2);

            }
            write++;

        }
    }
}
