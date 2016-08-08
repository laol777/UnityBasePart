using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NuitrackManagerEmulation : MonoBehaviour {

    public List<JSONData> data;

    int frame = 0;
    public int Frame { get { return frame; } }

    int[,] depthFrame;
    int[,] userFrame;

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

        StartCoroutine(FrameCounter());
    }

    int pastSentFrame = -1;

    public int[,] GetDepthFrame()
    {
        if (pastSentFrame != frame)
        {
            for (int i = 0; i < YRes; ++i)
                for (int j = 0; j < XRes; ++j)
                {
                    //try
                    {
                        //Debug.Log(i.ToString() + " " + j.ToString());
                        depthFrame[i, j] = data[frame].depth[i * XRes + j];
                    }
                    //catch (System.Exception ex)
                    {
                        //Debug.Log(i.ToString() + "_" + j.ToString() + " " + ex);
                        //Application.Quit();
                    }
                }

            pastSentFrame = frame;
        }
        return depthFrame;
    }

    public int[,] GetUserFrame()
    {
        if (pastSentFrame != frame)
        {
            for (int i = 0; i < YRes; ++i)
                for (int j = 0; j < XRes; ++j)
                {
                    //try
                    {
                        userFrame[i, j] = data[frame].userTracker[i * XRes + j];
                    }
                    //catch (System.Exception ex)
                    {
                        //Debug.Log(i.ToString() + "_" + j.ToString() + " " + ex);
                    }
                }
            pastSentFrame = frame;
        }
        return userFrame;
    }
}
