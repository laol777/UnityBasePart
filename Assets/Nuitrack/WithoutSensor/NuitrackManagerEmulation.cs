﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NuitrackManagerEmulation : MonoBehaviour {

    public List<JSONData> data;

    int frame = 0;
    public int Frame { get { return frame; } }

    int[,] depthFrame;
    public int[,] DepthFrame { get { return depthFrame; } }

    int[,] userFrame;
    public int[,] UserFrame { get { return userFrame; } }

    public int XRes, YRes; //suppose that the size constant

    void Awake()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
            Destroy(gameObject);

    }

    void Start()
    {

        data = SerializeData.LoadJSON<JSONData>("testDepthData");

        XRes = data[0].XRes;
        YRes = data[0].YRes;

        depthFrame = new int[YRes, XRes];
        userFrame = new int[YRes, XRes];

        StartCoroutine(FrameCounter());
    }

    [SerializeField]
    float timeOnFrame = 0.03f;

    IEnumerator FrameCounter()
    {
        yield return new WaitForSeconds(timeOnFrame);
        ++frame;
        if (frame >= data.Count)
            frame = 0;

        UpdateDataFrame();

        StartCoroutine(FrameCounter());
    }

    int pastSentFrame = -1;

    void UpdateDataFrame()
    {
        depthFrame = UpdateDepthFrame();
        userFrame = UpdateUserTracker();
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
        return userFrame;
    }
}
