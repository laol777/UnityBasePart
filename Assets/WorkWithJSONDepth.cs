using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkWithJSONDepth : MonoBehaviour {

    public List<JSONData> data;


    public JSONData oneFrame;
    public GameObject[,] cube;

    public GameObject prefab;
    Vector3 tmpPos;
    void Start () {

        tmpPos = new Vector3();

        data = SerializeData.LoadJSON<JSONData>("testDepthData");

        Debug.Log(data.Count);

        oneFrame = data[0];

        cube = new GameObject[oneFrame.rows, oneFrame.cols];
        for (int i = 0; i < oneFrame.rows; ++i)
            for (int j = 0; j < oneFrame.cols; ++j)
            {
                //cube[i, j] = new GameObject(i.ToString() + "_" + j.ToString());
                cube[i, j] = (GameObject)Instantiate(prefab);
            }

        UpdatePositionCube(oneFrame);

    }


    void UpdatePositionCube(JSONData data)
    {
        for (int i = 0; i < data.rows; ++i)
            for (int j = 0; j < data.cols; ++j)
            {
                tmpPos.x = j;
                tmpPos.y = data.rows - i;
                tmpPos.z = (float)(data.depth[i * data.cols + j] / 100f);

                cube[i, j].SetActive(tmpPos.z != 0f  && data.userTracker[i * data.cols + j] != 0 ? true : false);
                cube[i, j].transform.position = tmpPos;
            }
    }


    float time = 0f;
    bool newFrame;

    float timeOnFrame = 0.03f;

    int frameNumber = 0;

    bool Compare(JSONData a, JSONData b)
    {
        return a == b;
    }

    void Update()
    {
        Debug.Log(frameNumber);
        time += Time.deltaTime;

        if (time > timeOnFrame)
        {
            time = 0f;
            newFrame = true;
        }

        if (newFrame)
        {
            newFrame = false;
            UpdatePositionCube(data[frameNumber]);
            frameNumber++;
            if (frameNumber == data.Count)
            {
                frameNumber = 0;
            }
        }
        //Debug.Log(Compare(data[0], data[1]));
    }
}
