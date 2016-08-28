using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerBeahaviour : NetworkBehaviour
{

    Vector3 baseOffset;
    public Vector3 BaseOffset { get { return baseOffset; } }
    Vector3 offset;
    public Vector3 Offset { set { offset = value; }  get { return offset; } }
    Quaternion startRotation;

    [SerializeField]
    Transform camera;


    SkeletonTracker skeletonTracker;
    ChoiceStream choiceStream;

    SensorRotation sensorRotation;


    public Vector3 tmpPos;
    public int numberUser = 1;

    [SerializeField]
    Transform head;
    [SerializeField]
    Transform rightWrist;
    [SerializeField]
    Transform rightElbow;
    [SerializeField]
    Transform rotationPivot;

    [SerializeField]
    Transform cameraAim;

    BulletContainer bulletContainer;

    int[,] depthFrame;
    int[,] userFrame;
    int[,] detectIndicator;

    [SerializeField]
    GameObject shootEffectPlaceVisualisation;

    UserTrackerVisualization[] userTrackerVisualization;
    MiniCalibration miniCalibraion;
    void Start()
    {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
        startRotation = new Quaternion();

        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
        bulletContainer = GameObject.FindObjectOfType<BulletContainer>();
        handDeltas = new Vector3[2];
        userTrackerVisualization = GameObject.FindObjectsOfType<UserTrackerVisualization>();
        miniCalibraion = gameObject.GetComponent<MiniCalibration>();
    }



    IEnumerator RedactPlayerPrefabName()
    {
        yield return new WaitForSeconds(0.1f);
        if (((hasAuthority && isServer) || (!hasAuthority && !isServer)))
        {
            gameObject.name = "hostPlayer";
            baseOffset = new Vector3(0f, 0f, 5f);
            offset = baseOffset;
            startRotation = Quaternion.Euler(0f, 0f, 0f);
            sensorRotation.SetBaseRotation(startRotation);
            numberUser = 1;

            try
            {
                //offset.z -= 2.8f - choiceStream.GetJoint(nuitrack.JointType.LeftCollar, numberUser).z * 0.001f;
            }

            catch { }

            if (isServer)
            {
                //rotationPivot.rotation *= startRotation;
                transform.position += offset;
                sensorRotation.SetBaseRotation(Quaternion.Euler(0f, 180f, 0f));
            }
        }
        //else
        //{
        //    gameObject.name = "clientPlayer";
        //    offset = new Vector3(0f, 0f, -5f);
        //    startRotation = Quaternion.Euler(0f, 180f, 0f);
        //    sensorRotation.SetBaseRotation(startRotation);
        //    numberUser = 2;
        //    //rotationPivot.rotation *= startRotation;
        //    transform.position += offset;
        //    transform.rotation *= startRotation;
        //}

        //if(!hasAuthority) //disable, when is starting as remote player on current device
        //{
        //    camera.gameObject.SetActive(false);
        //    cursor.SetActive(false);
        //    cameraAim.gameObject.SetActive(false);
        //}
    }

    public GameObject bulletPrefab;
    public GameObject cursor;
    public GameObject quadCamera;
    public GameObject quadCursor;

    bool scalePositionCursor = false;
    bool prevScalePositionCursor = false;


    bool isStartFail;
    Vector3 tmpF;
    float timeFail = 5f;

    bool isHaveAbilityFail;

    float time = 0f;

    List<Transform> enemyBullets;

    public bool isEffectFail;

    public bool isShootPlayer;

    [SerializeField]
    Transform bulletIndicator;
    [SerializeField]
    Transform depthIndicator;

    bool isEffectFailProcess = false;

    Vector3[] handDeltas;

    public bool isStartDrop;
    float timeDrop;


    void Update()
    {
        if (choiceStream != null)
        {

            tmpPos = choiceStream.GetJoint(nuitrack.JointType.Head, numberUser) * 0.001f;
            head.localPosition = tmpPos;

            tmpPos = choiceStream.GetJoint(nuitrack.JointType.RightWrist, numberUser) * 0.001f;
            rightWrist.localPosition = tmpPos;

            tmpPos = choiceStream.GetJoint(nuitrack.JointType.RightElbow, numberUser) * 0.001f;
            rightElbow.localPosition = tmpPos;

        }

        if (miniCalibraion.isCalibrationComplite
           && (Mathf.Abs(miniCalibraion.positionCollarAfterCalibration.x - choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1).x) > 500f
           || Mathf.Abs(miniCalibraion.positionCollarAfterCalibration.z - choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1).z) > 500f) )
        {
            isStartDrop = true;
        }
        //Debug.Log(Mathf.Abs(miniCalibraion.positionCollarAfterCalibration.x - choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1).x));

        if (isStartDrop)
        {
            offset.y -= 9.8f * Time.deltaTime;
            timeDrop += Time.deltaTime;
            if (timeDrop > 5f)
            {
                Application.LoadLevel(Application.loadedLevel - 1);
            }
        }
        
        transform.position = offset; //change after calibration

        Vector3 vectorShoot = Vector3.Normalize(rightWrist.localPosition - rightElbow.localPosition);

        tmpF.x = vectorShoot.x * 2f;
        tmpF.y = vectorShoot.y * 2f;
        tmpF.z = - 10f;
        cursor.transform.localPosition = tmpF;
        time += Time.deltaTime;

        
        float angle = Vector3.Angle(vectorShoot, Vector3.back);

        if (Application.platform == RuntimePlatform.Android) //TODO: del it and add to serialize data all skeleton joint
        {
            handDeltas[0] = choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1) - choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1);
            handDeltas[1] = choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1) - choiceStream.GetJoint(nuitrack.JointType.RightShoulder, 1);
        }
        float angle2 = Vector3.Angle(handDeltas[0], handDeltas[1]);

        if (angle < 30f && angle2 < 30f)
        {
            scalePositionCursor = true;
        }
        else if ((angle < 60f) && (prevScalePositionCursor))
        {
            scalePositionCursor = true;
        }
        else
        {
            scalePositionCursor = false;
        }



        if (scalePositionCursor)
        {
            if (time > 1f && isShootPlayer)
            {
                time = 0f;
                GameObject tmp = (GameObject)Instantiate(bulletPrefab, rightWrist.position, Quaternion.identity);
                tmp.transform.parent = transform;
                tmp.GetComponent<MoveBullet>().vector = Vector3.Normalize(cursor.transform.localPosition - rightWrist.localPosition);
                tmp.GetComponent<MoveBullet>().velocity = 3f;
                if (!hasAuthority)
                {
                    tmp.GetComponent<MoveBullet>().IsLocal = false;
                    bulletContainer.AddBullet(tmp.transform);
                }
                Destroy(tmp, 10f);
                
            }
        }

        if (hasAuthority)
        {
            if (cursor != null)
            {

                vectorShoot.z = 0f;
                vectorShoot.x = -vectorShoot.x;

                quadCursor.transform.localPosition = vectorShoot / 2f;

                prevScalePositionCursor = scalePositionCursor;

            }

            if (quadCamera != null)
            {
                quadCamera.SetActive(scalePositionCursor);
            }

            enemyBullets = bulletContainer.GetBullet();
            for (int i = 0; i < enemyBullets.Count; ++i)
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


            //effectCollision

            depthFrame = choiceStream.GetDepthFrame();
            userFrame = choiceStream.GetUserFrame();
            int userID = choiceStream.GetUserID(numberUser);
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

                            if (numberUser == 1)
                            {
                                float fX = 0.5f / Mathf.Tan(0.5f);
                                float fY = fX * /*80 / 60*/ 1.33f;

                                tmpDepth.z = depthFrame[i, j] * 0.001f;
                                tmpDepth.x = tmpDepth.z * (float)((j - 40f) / (80f)) / fX;
                                tmpDepth.y = -tmpDepth.z * (float)((i - 30f) / (60f)) / fY;

                                tmpDepth.z += offset.z;
                                depthWithOffset = tmpDepth;
                            }
                            else
                            {
                                tmpDepth.x = -(float)((j - 40f) / (40f)) * 3f;
                                tmpDepth.y = (float)((i - 60f) / (60f)) * 2f;
                                tmpDepth.z = -depthFrame[i, j] * 0.001f + offset.z;
                                depthWithOffset = tmpDepth;
                            }
                            if (enemyBullets.Count != 0)
                            {
                                Transform bullet = enemyBullets[0];
                                {
                                    if (Vector3.Distance(depthWithOffset, bullet.position) < 0.05f)
                                    {
                                        if (!isEffectFailProcess)
                                        {
                                            SetIndicator(i, j);
                                            isBreak = true;
                                            StartCoroutine(EffectFail());
                                            //GameObject tmpShootEffectPlaceVisualisation = (GameObject)Instantiate(shootEffectPlaceVisualisation, depthWithOffset, Quaternion.identity);
                                            //Destroy(tmpShootEffectPlaceVisualisation, 1f);

                                        }
                                    }

                                }
                            }
                            foreach (Transform bullet in enemyBullets)
                            {
                                if (numberUser == 1)
                                {
                                    if (bullet.position.z > head.position.z + 0.2f)
                                        Destroy(bullet.gameObject);
                                }
                                else
                                {
                                    if (bullet.position.z < head.position.z)
                                        Destroy(bullet.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }




    }

    void SetIndicator(int a0, int b0)
    {
        
        foreach (UserTrackerVisualization utv in userTrackerVisualization)
        {
            int count = 0;
            int raduis = utv.radius;
            for (int i = 0; i < 60; ++i)
            {
                for (int j = 0; j < 80; ++j)
                {
                    if (Mathf.Sqrt((i - a0) * (i - a0) + (j - b0) * (j - b0)) < raduis)
                    {
                        utv.detectIndicator[i, j] = 1;
                    }
                }
            }
           
        }
        
    }


    void ClearIndicator()
    {
        foreach (UserTrackerVisualization utv in userTrackerVisualization)
        {
            for (int i = 0; i < 60; ++i)
            {
                for (int j = 0; j < 80; ++j)
                {
                    utv.detectIndicator[i, j] = 0;
                }
            }
        }
    }

    [SerializeField]
    GameObject quadFail;
    IEnumerator EffectFail()
    {
        isEffectFailProcess = true;
        if (hasAuthority) quadFail.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        if (hasAuthority) quadFail.SetActive(false);
        isEffectFailProcess = false;
        ClearIndicator();
    }
}
