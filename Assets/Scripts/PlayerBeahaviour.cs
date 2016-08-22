﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerBeahaviour : NetworkBehaviour
{
    Vector3 offset;
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
    void Start()
    {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
        startRotation = new Quaternion();

        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
        bulletContainer = GameObject.FindObjectOfType<BulletContainer>();
    }

    IEnumerator RedactPlayerPrefabName()
    {
        yield return new WaitForSeconds(0.1f);
        if (((hasAuthority && isServer) || (!hasAuthority && !isServer)))
        {
            gameObject.name = "hostPlayer";
            offset = new Vector3(0f, 0f, 5f);
            startRotation = Quaternion.Euler(0f, 0f, 0f);
            sensorRotation.SetBaseRotation(startRotation);
            numberUser = 1;
            if (isServer)
            {
                //rotationPivot.rotation *= startRotation;
                transform.position += offset;
                sensorRotation.SetBaseRotation(Quaternion.Euler(0f, 180f, 0f));
            }
        }
        else
        {
            gameObject.name = "clientPlayer";
            offset = new Vector3(0f, 0f, -5f);
            startRotation = Quaternion.Euler(0f, 180f, 0f);
            sensorRotation.SetBaseRotation(startRotation);
            //numberUser = 2;
            //rotationPivot.rotation *= startRotation;
            transform.position += offset;
            transform.rotation *= startRotation;
        }

        if(!hasAuthority) //disable, when is starting as remote player on current device
        {
            camera.gameObject.SetActive(false);
            cursor.SetActive(false);
            cameraAim.gameObject.SetActive(false);
        }
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

        Vector3 vectorShoot = Vector3.Normalize(rightWrist.localPosition - rightElbow.localPosition);
        cursor.transform.localPosition = new Vector3(vectorShoot.x * 2f, vectorShoot.y * 2f, -10f);
        time += Time.deltaTime;


        float angle = Vector3.Angle(vectorShoot, new Vector3(0f, 0f, -1f));

        if (angle < 30f)
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
            if (time > 1f)
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
               
                if (Vector3.Distance(enemyBullets[i].position, head.position) < 30f)
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
    
    

    
    }
}
