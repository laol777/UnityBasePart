using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TestJSON : MonoBehaviour {

    List<string> json;

    public bool addData;

    float timeBetweenFrame = 0.05f;

    JSONData frameData;

    DepthSensor depthSensor;

    int[] testData;

    void Start()
    {
        frameData = new JSONData();

        depthSensor = GameObject.FindObjectOfType<DepthSensor>();


        frameData = new JSONData();
        frameData.cols = 2;
        frameData.rows = 2;
        frameData.depth = new int[4] {1, 2, 3, 4 };
        //Debug.Log(JsonUtility.ToJson(frameData));

        //json = new List<string>();

        //json.Add(JsonUtility.ToJson(frameData));
        //json.Add(JsonUtility.ToJson(frameData));

        //Debug.Log(json);
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
            write++;

            frameData.rows = depthSensor.DepthFrame.Rows;
            frameData.cols = depthSensor.DepthFrame.Cols;

            if (frameData.rows != (frameData.depth.Length / frameData.cols) && frameData.cols != (frameData.depth.Length / frameData.rows))
            {
                Debug.Log(frameData.depth.Length);
                frameData.depth = new int[frameData.rows * frameData.cols];
                Debug.Log(frameData.depth.Length);
            }
            float mean = 0f;
            for (int i = 0; i < frameData.rows; ++i)
                for (int j = 0; j < frameData.cols; ++j)
                {
                    frameData.depth[i * frameData.cols + j] = depthSensor.DepthFrame[i, j];
                    mean += depthSensor.DepthFrame[i, j];
                }

            mean /= 4800;

            Debug.Log(mean);

            if (write == 50)
            {
                Debug.Log(frameData.depth[30 * 80 + 40]);
            }
            //json.Add(JsonUtility.ToJson(frameData));
        }
    }
}
