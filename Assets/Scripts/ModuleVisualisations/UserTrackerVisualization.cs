using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using nuitrack.issues;

public class UserTrackerVisualization : MonoBehaviour
{
    #region Fields
    [SerializeField, Range(1, 2)]
    int numberPlayer = 1;

    Vector3 offset;
    public Vector3 baseRotation;

    DepthSensor depthSensor;
    UserTracker userTracker;

    nuitrack.DepthFrame depthFrame = null;
    nuitrack.UserFrame userFrame = null;

    [SerializeField]int hRes;
    int frameStep;
    float depthToScale;

    //visualization fields
    [SerializeField]Color[] userCols;
    Color[] occludedUserCols;

    [SerializeField]Mesh sampleMesh;
    [SerializeField]float meshScaling = 1f;
    [SerializeField]Material visualizationMaterial;

    int pointsPerVis, parts;

    int vertsPerMesh, trisPerMesh;
    int[] sampleTriangles;
    Vector3[] sampleVertices;
    Vector3[] sampleNormals;
    Vector2[] sampleUvs;

    List<int[]> triangles;
    List<Vector3[]> vertices;
    List<Vector3[]> normals;
    List<Vector2[]> uvs;
    List<Vector2[]> uv2s;
    List<Vector2[]> uv3s;
    List<Vector2[]> uv4s;
    List<Color[]> colors;

    Color[] userCurrentCols;

    GameObject[] visualizationParts;
    Mesh[] visualizationMeshes;

    Texture2D depthTexture, segmentationTexture;
    Color[] depthColors;
    Color[] segmentationColors;
    #endregion


    ChoiceStream choiceStream;

    Color transparentColor;
    Color ones;

    public int radius = 7;

    public int[,] detectIndicator;
    public PlayerBeahaviour playerBehaviour;

    void Start()
    {
        transparentColor = new Color(0f, 0f, 0f, 0f);
        ones = new Color(1f, 1f, 1f, 1f);
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        occludedUserCols = new Color[userCols.Length];
        userCurrentCols = new Color[userCols.Length];
        for (int i = 0; i < userCols.Length; i++)
        {
            userCurrentCols[i] = userCols[i];
            float[] hsv = new float[3];
            Color.RGBToHSV(userCols[i], out hsv[0], out hsv[1], out hsv[2]);
            hsv[2] *= 0.25f;
            occludedUserCols[i] = Color.HSVToRGB(hsv[0], hsv[1], hsv[2]);
            occludedUserCols[i].a = userCols[i].a;
        }

        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
        userTracker = GameObject.FindObjectOfType<UserTracker>();
        detectIndicator = new int[60, 80];

        //nuitrack.OutputMode mode = DepthSensor.GetDepthSensor.GetOutputMode();
        //frameStep = mode.XRes / hRes;
        //if (frameStep <= 0) frameStep = 1; // frameStep should be greater then 0
        //hRes = mode.XRes / frameStep;

        //depthToScale = meshScaling * 2f * Mathf.Tan (0.5f * mode.HFOV) / hRes;

        //InitMeshes( 
        //    ((mode.XRes / frameStep) + (mode.XRes % frameStep == 0 ? 0 : 1)),
        //    ((mode.YRes / frameStep) + (mode.YRes % frameStep == 0 ? 0 : 1)),
        //    mode.HFOV
        //    );

        int XRes = 80;
        int YRes = 60;
        int HFOV = 1;

        frameStep = XRes / hRes;
        if (frameStep <= 0) frameStep = 1; // frameStep should be greater then 0
        hRes = XRes / frameStep;

        depthToScale = meshScaling * 2f * Mathf.Tan(0.5f * HFOV) / hRes;

        InitMeshes(
            ((XRes / frameStep) + (XRes % frameStep == 0 ? 0 : 1)),
            ((YRes / frameStep) + (YRes % frameStep == 0 ? 0 : 1)),
            HFOV
            );

        playerBehaviour = GameObject.FindObjectOfType<PlayerBeahaviour>();
    }

    #region Mesh generation and mesh update methods
    void InitMeshes(int cols, int rows, float hfov)
    {
        //depthTexture, segmentationTexture

        depthColors = new Color[cols * rows];
        segmentationColors = new Color[cols * rows];

        depthTexture = new Texture2D(cols, rows, TextureFormat.RFloat, false);
        segmentationTexture = new Texture2D(cols, rows, TextureFormat.ARGB32, false);

        depthTexture.filterMode = FilterMode.Point;
        depthTexture.wrapMode = TextureWrapMode.Clamp;

        segmentationTexture.filterMode = FilterMode.Point;
        segmentationTexture.wrapMode = TextureWrapMode.Clamp;

        depthTexture.Apply();
        segmentationTexture.Apply();


        visualizationMaterial.SetTexture("_DepthTex", depthTexture);
        visualizationMaterial.SetTexture("_SegmentationTex", segmentationTexture);

        int numPoints = cols * rows;

        vertsPerMesh = sampleMesh.vertices.Length;
        trisPerMesh = sampleMesh.triangles.Length;

        sampleVertices = sampleMesh.vertices;

        for (int i = 0; i < sampleVertices.Length; i++)
        {
            sampleVertices[i] *= depthToScale;
            visualizationMaterial.SetVector("_Offsets" + i.ToString(), sampleVertices[i]);
        }

        sampleTriangles = sampleMesh.triangles;
        sampleNormals = sampleMesh.normals;
        sampleUvs = sampleMesh.uv;

        vertices = new List<Vector3[]>();
        triangles = new List<int[]>();
        normals = new List<Vector3[]>();
        uvs = new List<Vector2[]>();
        uv2s = new List<Vector2[]>();
        uv3s = new List<Vector2[]>();

        colors = new List<Color[]>();

        pointsPerVis = 64000 / vertsPerMesh; //can't go over the limit for number of mesh vertices in one mesh
        parts = numPoints / pointsPerVis + (((numPoints % pointsPerVis) != 0) ? 1 : 0);

        visualizationParts = new GameObject[parts];
        visualizationMeshes = new Mesh[parts];

        float fX, fY;
        fX = 0.5f / Mathf.Tan(0.5f * hfov);
        fY = fX * cols / rows;

        visualizationMaterial.SetFloat("fX", fX);
        visualizationMaterial.SetFloat("fY", fY);

        //generation of triangle indexes, vertices, uvs and normals for all visualization parts

        for (int i = 0, row = 0, col = 0; i < parts; i++)
        {
            int numPartPoints = Mathf.Min(pointsPerVis, numPoints - i * pointsPerVis);

            int[] partTriangles = new int[numPartPoints * trisPerMesh];
            Vector3[] partVertices = new Vector3[numPartPoints * vertsPerMesh];
            Vector3[] partNormals = new Vector3[numPartPoints * vertsPerMesh];
            Vector2[] partUvs = new Vector2[numPartPoints * vertsPerMesh];
            Vector2[] partUv2s = new Vector2[numPartPoints * vertsPerMesh];
            Vector2[] partUv3s = new Vector2[numPartPoints * vertsPerMesh];
            Color[] partColors = new Color[numPartPoints * vertsPerMesh];

            for (int j = 0; j < numPartPoints; j++)
            {
                for (int k = 0; k < trisPerMesh; k++)
                {
                    partTriangles[j * trisPerMesh + k] = sampleTriangles[k] + j * vertsPerMesh;
                }
                Vector2 depthTextureUV = new Vector2(((float)col + 0.5f) / cols, ((float)row + 0.5f) / rows);
                for (int k = 0; k < vertsPerMesh; k++)
                {
                    partUv2s[j * vertsPerMesh + k] = depthTextureUV;
                    partUv3s[j * vertsPerMesh + k] = new Vector2(k, 0);
                }
                System.Array.Copy(sampleVertices, 0, partVertices, j * vertsPerMesh, vertsPerMesh);
                System.Array.Copy(sampleNormals, 0, partNormals, j * vertsPerMesh, vertsPerMesh);
                System.Array.Copy(sampleUvs, 0, partUvs, j * vertsPerMesh, vertsPerMesh);

                col++;
                if (col == cols)
                {
                    row++;
                    col = 0;
                }
            }

            triangles.Add(partTriangles);
            vertices.Add(partVertices);
            normals.Add(partNormals);
            uvs.Add(partUvs);
            uv2s.Add(partUv2s);
            uv3s.Add(partUv3s);
            colors.Add(partColors);

            visualizationMeshes[i] = new Mesh();
            visualizationMeshes[i].vertices = vertices[i];
            visualizationMeshes[i].triangles = triangles[i];
            visualizationMeshes[i].normals = normals[i];
            visualizationMeshes[i].uv = uvs[i];
            visualizationMeshes[i].uv2 = uv2s[i];
            visualizationMeshes[i].uv3 = uv3s[i];
            visualizationMeshes[i].colors = colors[i];

            Bounds meshBounds = new Bounds(500f * Vector3.one, 1000f * Vector3.one);
            visualizationMeshes[i].bounds = meshBounds;
            visualizationMeshes[i].MarkDynamic();

            visualizationParts[i] = new GameObject();
            visualizationParts[i].name = "Visualization_" + i.ToString();
            visualizationParts[i].transform.position = Vector3.zero;
            visualizationParts[i].transform.rotation = Quaternion.identity;

            visualizationParts[i].transform.position += offset;
            visualizationParts[i].transform.rotation *= Quaternion.Euler(baseRotation);

            visualizationParts[i].AddComponent<MeshFilter>();
            visualizationParts[i].GetComponent<MeshFilter>().mesh = visualizationMeshes[i];
            visualizationParts[i].AddComponent<MeshRenderer>();
            visualizationParts[i].GetComponent<Renderer>().sharedMaterial = visualizationMaterial;
        }
    }
    #endregion

    int frame = -1;

    void Test()
    {
        int one = 0;
        for (int i = 0; i < 60; ++i)
        {
            for (int j = 0; j < 80; ++j)
            {
                if (detectIndicator[i, j] == 1)
                {
                    one++;
                }
            }
        }
        if (one != 0)
        {
            Debug.Log(one);
        }
    }

    void Update()
    {
        bool haveNewFrame = false;
        if (choiceStream.GetDepthFrame() != null)
        {        
            if (frame != choiceStream.Frame)
            {
                haveNewFrame = true;
                frame = choiceStream.Frame;
            }

            if (haveNewFrame)
            {
                //Test();
                ProcessFrame(choiceStream.GetDepthFrame(), choiceStream.GetUserFrame(), 80, 60);
            }
        }
        else
        {
            HideVisualization();
        }
    }

    void HideVisualization()
    {
        for (int i = 0; i < parts; i++)
        {
            if (visualizationParts[i].activeSelf) visualizationParts[i].SetActive(false);
        }
    }

    int[,] testArray;
    Color pointColor = Color.white;
    void ProcessFrame(int[,] depthFrame, int[,] userFrame, int Cols, int Rows)
    {

        for (int i = 0; i < parts; i++)
        {
            if (!visualizationParts[i].activeSelf) visualizationParts[i].SetActive(true);
            if (playerBehaviour != null)
            {
                visualizationParts[i].transform.position = playerBehaviour.Offset;
            }
            else
            {
                playerBehaviour = GameObject.FindObjectOfType<PlayerBeahaviour>();
            }
        }

        int visPartInd = 0;
        int pointInd = 0;
        int pointsPerVisTotal = pointsPerVis * vertsPerMesh;

        int[] userID = choiceStream.GetSegmentationID();

        for (int i = 0, pointIndex = 0; i < Rows; i += frameStep)
        {
            for (int j = 0; j < Cols; j += frameStep, ++pointIndex)
            {
                pointColor = transparentColor;
                depthColors[pointIndex].r = depthFrame[i, j] / 16384f;

                uint userId = 0u; 
                if (userFrame != null) 
                {
                    userId = (uint)userFrame[i * Rows / Rows,
                    j * Cols / Cols];
                }

                #region RGB coloring
                int rgbOffset = 3 * (i * Cols + j);
                //Color rgbCol = new Color32(depthFrame.rgb[rgbOffset + 2], depthFrame.rgb[rgbOffset + 1], depthFrame.rgb[rgbOffset + 0], 255);
                //pointColor = rgbCol;
                //Debug.Log(j.ToString() + ", " + i.ToString() + " : " + rgbCol);
                #endregion

                //pointColor = userCurrentCols[userId]; //user segmentation coloring
                //if (userId != 0)
                //{
                //    pointColor = Color.white;
                //}

                //try
                //{
                //    if (userFrame[i, j] == userID[userID.Length - numberPlayer])
                //        pointColor = Color.white;
                //}
                //catch (Exception ex) { }

                if (userID != null && numberPlayer <= userID.Length)
                {
                    
                    if (userFrame[i, j] == userID[userID.Length - numberPlayer])
                    {
                        pointColor = ones;
                    }
                }
                //pointColor = Color.white;
                //pointColor = ones;
                if (detectIndicator[i, j] == 1 && userFrame[i, j] != 0)
                {
                    pointColor = Color.red;
                }

                segmentationColors[pointIndex] = pointColor;
            }
        }
        depthTexture.SetPixels(depthColors);
        segmentationTexture.SetPixels(segmentationColors);

        depthTexture.Apply();
        segmentationTexture.Apply();
    }
	
    void OnDestroy()
    {
        if (depthTexture != null) Destroy(depthTexture);
        if (segmentationTexture != null) Destroy(segmentationTexture);


        if (visualizationParts != null)
        {
            for (int i = 0; i < visualizationParts.Length; i++)
            {
                Destroy(visualizationParts[i]);
            }
        }
    }
}