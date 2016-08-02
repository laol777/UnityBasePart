using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

public class TestJSON : MonoBehaviour {

    List<string> json;

    public bool addData;

    float timeBetweenFrame = 0.05f;

    JSONData frameData;
    List<JSONData> serializeData;

    DepthSensor depthSensor;

    int[] testData;
    FileInfo f;
    void Start()
    {
        f = new FileInfo(Application.persistentDataPath + "/playerSave.json");

        serializeData = new List<JSONData>();

        depthSensor = GameObject.FindObjectOfType<DepthSensor>();

        frameData = new JSONData();
        frameData.rows = 2;
        frameData.cols = 2;
        frameData.depth = new int[] { 1, 2, 3, 4 };
    }

    void Save(string data)
    {
        StreamWriter w;
        if (!f.Exists)
        {
            w = f.CreateText();
        }
        else
        {
            f.Delete();
            w = f.CreateText();
        }
        w.WriteLine(data);
        w.Close();
    }

    void SaveJSON<T>(List<T> serializeData)
    {
        string serialized = JsonConvert.SerializeObject(serializeData);
        Save(serialized);
    }

    string Load()
    {
        StreamReader r = File.OpenText(Application.persistentDataPath + "/playerSave.json");
        string info = r.ReadToEnd();
        r.Close();
        return info;
    }

    List<T> LoadJSON<T>()
    {
        return JsonConvert.DeserializeObject<List<T>>(Load());
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

            int a = 5;

            if (write < a)
            {
                serializeData.Add(frameData);
                Debug.Log("1");
            }
            if (write == a)
            {
                serializeData.Add(frameData);


                Debug.Log("testWrite");
                SaveJSON(serializeData);


                List<JSONData> test2 = LoadJSON<JSONData>();
                Debug.Log(test2);

              
                Debug.Log(Application.persistentDataPath);

            }

        }
    }
}
