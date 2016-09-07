//#define DEDUG_TEST
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerCollisionDetect : MonoBehaviour {

    [SerializeField]
    int countLife = 5;
    [SerializeField]AudioSource soundHit;

    PlayerBeahaviour playerBeahaviour;

    List<Transform> enemyBullets;

    public BulletContainer bulletContainerBot;

    [SerializeField]Transform head;

    int[,] depthFrame;
    int[,] userFrame;
    [SerializeField]Text countLifeText;
    ChoiceStream choiceStream;


    bool isEffectFailProcess = false;


    Vector3 offset;
    public Vector3 Offset { set { offset = value; } get { return offset; } }


    [SerializeField]
    GameObject prefab;
#if DEDUG_TEST
    GameObject[,] cubeVisualization;


    int X = 80;
    int Y = 60;
    float widht = 4;
    float height = 3;
#endif
    void Start () {
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        offset = new Vector3(0f, 0f, -5f);

        playerBeahaviour = gameObject.GetComponent<PlayerBeahaviour>();

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
#endif
    }

    void Update () {


        enemyBullets = bulletContainerBot.GetBullet();
        for (int i = 0; i < enemyBullets.Count; ++i)
        {
            if (enemyBullets[i] != null)
            {
                if (Vector3.Distance(enemyBullets[i].position, head.position) < 4f)
                {

                    float speed = Vector3.Distance(enemyBullets[i].position, head.position);
                    enemyBullets[i].gameObject.GetComponent<MoveBullet>().velocity =
                        Mathf.Clamp(Mathf.Lerp(enemyBullets[i].gameObject.GetComponent<MoveBullet>().velocity, speed / 2f, 0.1f), 0.5f, 3f);
                }
                else
                {
                    enemyBullets[i].gameObject.GetComponent<MoveBullet>().velocity = 3f;
                }
            }
        }


        //effectCollision

        depthFrame = choiceStream.GetDepthFrame();
        userFrame = choiceStream.GetUserFrame();
        int userID = choiceStream.GetUserID(1);
        Vector3 depthWithOffset = Vector3.zero;
        Vector3 tmpDepth;

        Vector3 bulletCoord = Vector3.zero;
        Vector3 depthCoord = Vector3.zero;
        int a = 0;


        bool isBreak = false;

        if (depthFrame != null && userFrame != null)
        {
            for (int i = 0; i < choiceStream.YRes; ++i)
            {
                if (isBreak) break;
                for (int j = 0; j < choiceStream.XRes; ++j)
                {
                    if (isBreak) break;
                    if (userFrame[i, j] != 0) //check it
                    {
                        a++;

         
                        float fX = 0.5f / Mathf.Tan(0.5f);
                        float fY = fX * /*80 / 60*/ 1.33f;

                        tmpDepth.z = -depthFrame[i, j] * 0.001f;
                        tmpDepth.x = tmpDepth.z * (float)((j - 40f) / (80f)) / fX;
                        tmpDepth.y = tmpDepth.z * (float)((i - 30f) / (60f)) / fY;

                        //tmpDepth.z += offset.z;
                        tmpDepth.z += playerBeahaviour.Offset.z;
                        depthWithOffset = tmpDepth;
                        
                        if (enemyBullets.Count != 0)
                        {
                            Transform bullet = enemyBullets[0];
                            if (bullet != null)
                            {
                                if (Vector3.Distance(depthWithOffset, bullet.position) < 0.05f)
                                {
                                    if (!isEffectFailProcess)
                                    {
                                        isBreak = true;
                                        StartCoroutine(EffectFail());
                                        soundHit.Play();
                                        --countLife;
                                        countLifeText.text = "countLife " + countLife.ToString();
                                        if (countLife < 0)
                                        {
                                            Application.LoadLevel(Application.loadedLevel - 1);
                                        }

                                        //GameObject tmpShootEffectPlaceVisualisation = (GameObject)Instantiate(shootEffectPlaceVisualisation, depthWithOffset, Quaternion.identity);
                                        //Destroy(tmpShootEffectPlaceVisualisation, 1f);

                                    }
                                }

                            }
                        }
                        foreach (Transform bullet in enemyBullets)
                        {
                            if (bullet != null)
                            {
            
                                if (bullet.position.z < head.position.z - 0.2f)
                                    Destroy(bullet.gameObject);
                             
                            }
                        }
                    }
                }
            }
        }


#if DEDUG_TEST
        depthFrame = choiceStream.GetDepthFrame();
        userFrame = choiceStream.GetUserFrame();
        userID = choiceStream.GetUserID(1);
        depthWithOffset = Vector3.zero;

        float minDist = 100f;
        bulletCoord = Vector3.zero;
        depthCoord = Vector3.zero;

        //List<Transform> enemyBullets = bulletContainer.GetBullet();

        if (depthFrame != null && userFrame != null)
        {
            for (int i = 0; i < choiceStream.YRes; ++i)
                for (int j = 0; j < choiceStream.XRes; ++j)
                {
                    if (userFrame[i, j] == userID)
                    {
                        float fX = 0.5f / Mathf.Tan(0.5f);
                        float fY = fX * /*80 / 60*/ 1.33f;
                        
                        tmpDepth.z = -depthFrame[i, j] * 0.001f;
                        tmpDepth.x = tmpDepth.z * (float)((j - 40f) / (80f)) / fX;
                        tmpDepth.y = tmpDepth.z * (float)((i - 30f) / (60f)) / fY;

                        //tmpDepth.z += offset.z;
                        tmpDepth.z += playerBeahaviour.Offset.z;
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
    }

    [SerializeField]
    GameObject quadFail;
    IEnumerator EffectFail()
    {
        isEffectFailProcess = true;
        //if (hasAuthority)
        quadFail.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        //if (hasAuthority)
        quadFail.SetActive(false);
        isEffectFailProcess = false;
    }

}
