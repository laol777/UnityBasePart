//#define DEDUG_TEST
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotControl : MonoBehaviour {

    float timeBetweenShoot = 0.4f;
    Vector3 nextPosition;

    public bool startShoot = false;

    Vector3 basePos;
    [SerializeField]GameObject bullet;

    [SerializeField]
    BulletContainer bulletContainerBot;
    [SerializeField]
    BulletContainer bulletContainerPlayer;

    int X = 80;
    int Y = 60;

    int[,] depthFrame;
    int[,] userFrame;

    ChoiceStream choiceStream;
    Vector3 offset;

    float widht = 4;
    float height = 3;
    [SerializeField]
    GameObject prefab;

    MiniCalibration miniCalibration;
    GameObject[,] cubeVisualization;
    void Start()
    {
        
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        basePos = transform.position;
        miniCalibration = GameObject.FindObjectOfType<MiniCalibration>();
#if DEDUG_TEST
        cubeVisualization = new GameObject[60, 80];
        for (int i = 0; i < 60; ++i)
        {
            for (int j = 0; j < 80; ++j)
            {
                cubeVisualization[i, j] = (GameObject)Instantiate(prefab);
                cubeVisualization[i, j].transform.localScale = new Vector3((float)(widht / X), (float)(height / Y), 0.05f);
            }

        }

        offset = new Vector3(0f, 0f, -5f);
#endif
    }

    float vel = 1f;

    public int iter = 0;

    void Update()
    {
#if DEDUG_TEST
        depthFrame = choiceStream.GetDepthFrame();
        userFrame = choiceStream.GetUserFrame();
        int userID = choiceStream.GetUserID(1);
        Vector3 depthWithOffset = Vector3.zero;
        Vector3 tmpDepth;

        float minDist = 100f;
        Vector3 bulletCoord = Vector3.zero;
        Vector3 depthCoord = Vector3.zero;
        int a = 0;

        //List<Transform> enemyBullets = bulletContainer.GetBullet();

        if (depthFrame != null && userFrame != null)
        {
            for (int i = 0; i < choiceStream.YRes; ++i)
                for (int j = 0; j < choiceStream.XRes; ++j)
                {
                    if (userFrame[i, j] == userID)
                    {
                        a++;

                        float fX = 0.5f / Mathf.Tan(0.5f);
                        float fY = fX * /*80 / 60*/ 1.33f;
                        
                        tmpDepth.z = -depthFrame[i, j] * 0.001f;
                        tmpDepth.x = tmpDepth.z * (float)((j - 40f) / (80f)) / fX;
                        tmpDepth.y = tmpDepth.z * (float)((i - 30f) / (60f)) / fY;

                        tmpDepth.z += offset.z;
                        depthWithOffset = tmpDepth;

                        cubeVisualization[i, j].SetActive(true);
                        cubeVisualization[i, j].transform.position = depthWithOffset;
                    }
                    else
                    {
                        cubeVisualization[i, j].SetActive(false);
                    }
                }
        }
#endif
        if (miniCalibration == null)
        {
            miniCalibration = GameObject.FindObjectOfType<MiniCalibration>();
        }

        if (miniCalibration != null && miniCalibration.isCalibrationComplite)
        {
            ++iter;
        }

        if (startShoot && iter == 1 )
        {
            startShoot = false;
            StartCoroutine(ShootBehaviour());
        }

        target.position = Vector3.Lerp(target.position, targetNextPose, Time.deltaTime * 5f);
    }

    [SerializeField]
    Transform target;
    Vector3 targetNextPose;
    [SerializeField]
    Transform rightWrist;

    IEnumerator ShootBehaviour()
    {

        Vector3 rndSize = new Vector3(Random.Range(-0.03f, 0.03f), Random.Range(-0.05f, 0.05f), 0f);
        targetNextPose = new Vector3(rndSize.x * 33f, rndSize.y * 33f, 0f);

        yield return new WaitForSeconds(0.5f);

        GameObject tmpBullet = (GameObject)Instantiate(bullet, rightWrist.position, Quaternion.identity);
        tmpBullet.GetComponent<AudioSource>().Play();
        bulletContainerBot.AddBullet(tmpBullet.transform);


        tmpBullet.GetComponent<MoveBullet>().velocity = 6f;
        tmpBullet.GetComponent<MoveBullet>().vector = Vector3.back + rndSize;

        Destroy(tmpBullet, 12f);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ShootBehaviour());
    }

}
