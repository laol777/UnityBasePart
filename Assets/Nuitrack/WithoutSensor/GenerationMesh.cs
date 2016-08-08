using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerationMesh : MonoBehaviour {

    public List<JSONData> data;

    Vector3[,] ToVector3Data(JSONData data)
    {
        Vector3 tmpPos;
        Vector3[,] dataToCoord = new Vector3[data.YRes, data.XRes];//60, 80

        for (int i = 0; i < data.YRes; ++i)
            for (int j = 0; j < data.XRes; ++j)
            {
                tmpPos.x = j;
                tmpPos.y = data.YRes - i;
                tmpPos.z = (float)(data.depth[i * data.XRes + j] / 100f);

                dataToCoord[i, j] = tmpPos;
                //cube[i, j].SetActive(tmpPos.z != 0f ? true : false);
                //cube[i, j].transform.position = tmpPos;
            }

        return dataToCoord;
    }

    public Vector3[,] coord;
    public GameObject[,] cube;
    public GameObject prefab;

    void Start () {



        data = SerializeData.LoadJSON<JSONData>("testDepthData");

        cube = new GameObject[data[0].YRes, data[0].XRes];
        for (int i = 0; i < data[0].YRes; ++i)
            for (int j = 0; j < data[0].XRes; ++j)
            {
                cube[i, j] = (GameObject)Instantiate(prefab);
            }
    }

    float time = 0f;

    void Update()
    {
        time += Time.deltaTime;

        if (time > 0.03f)
        {
            time = 0f;
            UpdatePosCube(GetCoord(), 80, 60);
        }
    }


    void UpdatePosCube(Vector3[,] coord, int xMax, int yMax)
    {
        for (int i = 0; i < yMax; ++i)
            for (int j = 0; j < xMax; ++j)
            {
                cube[i, j].transform.position = coord[i, j];
                cube[i, j].SetActive(coord[i, j].z != 0f ? true : false);
                //cube[i, j].transform. // (coord[i, j].z != 0f ? true : false);
                //cube[i, j].transform.
            }
    }

    int frame = -1;
    public Vector3[,] GetCoord()
    {
        ++frame;
        frame = frame >= data.Count ? 0: frame;

        return ToVector3Data(data[frame]);
    }
}
